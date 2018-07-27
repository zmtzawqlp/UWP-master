using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Web;

namespace MyUWPToolkit.Util
{
    public static class HttpClientHelper
    {
        private static readonly object SyncRoot = new object();
        private static volatile HttpClient _client;

        /// <summary>
        /// HTTP客户端单例
        /// </summary>
        public static HttpClient Client
        {
            get
            {
                if (_client == null)
                {
                    lock (SyncRoot)
                    {
                        if (_client == null)
                        {
                            var handler = new HttpClientHandler();
                            if (handler.SupportsAutomaticDecompression)
                            {
                                handler.AutomaticDecompression = DecompressionMethods.GZip |
                                                                 DecompressionMethods.Deflate;
                            }
                            _client = new HttpClient(handler);
                        }
                    }
                }

                return _client;
            }
        }

        #region GET

        private static string GetQueryString(IEnumerable<KeyValuePair<string, string>> content) => string.Join("&", content.Select(p => p.Key + "=" + p.Value));

        public static Task<string> GetStringAsync(Uri uri, IEnumerable<KeyValuePair<string, string>> content, CancellationToken ct = default(CancellationToken), bool noCache = false)
            => GetStringAsync((new UriBuilder(uri) { Query = GetQueryString(content) }).Uri, ct, noCache);

        public static Task<string> GetStringAsync(Uri uri, CancellationToken ct = default(CancellationToken), bool noCache = false)
            => GetAsync(uri, response => response.Content.ReadAsStringAsync(), ct, noCache);

        public static Task<IBuffer> GetBufferAsync(Uri uri, CancellationToken ct = default(CancellationToken), bool noCache = false)
            => GetAsync(uri, async response => (await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false)).AsBuffer(), ct, noCache);

        private static async Task<TResult> GetAsync<TResult>(
            Uri uri,
            Func<HttpResponseMessage, Task<TResult>> readResponseContent,
            CancellationToken ct,
            bool noCache)
        {
            ct.ThrowIfCancellationRequested();

            using (var request = new HttpRequestMessage(HttpMethod.Get, uri))
            {
                if (noCache)
                    request.Headers.IfModifiedSince = DateTimeOffset.Now;

                return await RetryHelper.RunWithDelayAsync(
                    async () =>
                    {
                        HttpResponseMessage response = await Client.SendAsync(request, ct).ConfigureAwait(false);
                        response.EnsureSuccessStatusCode();
                        return await readResponseContent(response).WithCancellation(ct).ConfigureAwait(false);
                    },
                    ct,
                    ex => WebError.GetStatus(ex.HResult) == WebErrorStatus.CannotConnect && !NetworkInfo.IsNetworkAvailable).ConfigureAwait(false);
            }
        }

        public static async Task<BitmapImage> GetImageAsync(string url)
        {
            try
            {
                byte[] data = (await GetBufferAsync(new Uri(url), noCache: true)).ToArray();
                var bmp = new BitmapImage();
                var stream = new InMemoryRandomAccessStream();
                stream.AsStreamForWrite().Write(data, 0, data.Length);
                stream.AsStreamForWrite().Flush();
                stream.Seek(0);
                bmp.SetSource(stream);
                return bmp;
            }
            catch
            { }

            return null;
        }
        #endregion

        #region POST

        public static Task<string> PostStringAsync(Uri uri, IEnumerable<KeyValuePair<string, string>> content, CancellationToken ct = default(CancellationToken))
            => PostStringAsync(uri, new FormUrlEncodedContent(content), ct);

        public static Task<string> PostStringAsync(Uri uri, HttpContent content, CancellationToken ct = default(CancellationToken), Action<HttpRequestMessage> requestAction = null)
            => PostAsync(uri, content, response => response.Content.ReadAsStringAsync(), ct, requestAction);

        public static Task<IBuffer> PostBufferAsync(Uri uri, HttpContent content, CancellationToken ct = default(CancellationToken))
            => PostAsync(uri, content, async response => (await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false)).AsBuffer(), ct);

        private static async Task<TResult> PostAsync<TResult>(
            Uri uri,
            HttpContent content,
            Func<HttpResponseMessage, Task<TResult>> readResponseContent,
            CancellationToken ct,
            Action<HttpRequestMessage> requestAction = null)
        {
            ct.ThrowIfCancellationRequested();

            ValidationHelper.ArgumentNotNull(content, nameof(content));

            return await RetryHelper.RunWithDelayAsync(
                async () =>
                {
                    using (var request = new HttpRequestMessage(HttpMethod.Post, uri))
                    {
                        request.Content = content;
                        requestAction?.Invoke(request);

                        HttpResponseMessage response = await Client.SendAsync(request, ct).ConfigureAwait(false);
                        response.EnsureSuccessStatusCode();
                        return await readResponseContent(response).WithCancellation(ct).ConfigureAwait(false);
                    }
                },
                ct,
                ex => WebError.GetStatus(ex.HResult) == WebErrorStatus.CannotConnect && !NetworkInfo.IsNetworkAvailable).ConfigureAwait(false);
        }

        #endregion
    }
}
