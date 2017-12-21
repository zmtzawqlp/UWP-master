using BodyNamed.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace BodyNamed.Utils
{
    public class BodyNamesHelper
    {
        public static BodyNamesHelper Instance;
        public const string FileName = "BodyNames.txt";
        static BodyNamesHelper()
        {
            Instance = new BodyNamesHelper();
        }
        private object sync = new object();
        private StorageFile file;
        private StreamWriter _streamWriter;
        public BodyNames BodyNames { get; private set; }
        public BodyNamesHelper()
        {
            BodyNames = new BodyNames();
        }
        public async Task Initialize()
        {
            try
            {
                var localFolder = ApplicationData.Current.LocalFolder;
                file = await localFolder.CreateFileAsync(FileName, CreationCollisionOption.OpenIfExists);
                if (file != null)
                {
                    using (Stream stream = await file.OpenStreamForReadAsync())
                    {
                        byte[] result = new byte[stream.Length];
                        await stream.ReadAsync(result, 0, (int)stream.Length);
                        var content = Encoding.UTF8.GetString(result, 0, result.Length);

                        var shape = JsonConvert.DeserializeObject<BodyNames>(content);
                        if (shape != null)
                        {
                            this.BodyNames = shape;
                        }
                        else
                        {
                            this.BodyNames.Clear();
                        }
                    }
                    _streamWriter?.Dispose();
                    var stream1 = await file.OpenStreamForWriteAsync();
                    stream1.Seek(0, SeekOrigin.Begin);
                    _streamWriter = new StreamWriter(stream1);
                }

            }
            catch (Exception e)
            {
            }
        }

        public async Task CopyFile()
        {
            if (!await StorageHelper.FileExistsAsync(FileName))
            {
                var source = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Assets/{FileName}"));
                await source.CopyAsync(ApplicationData.Current.LocalFolder);
            }
        }

        string preJson = "";
        public void Save()
        {
            lock (sync)
            {
                try
                {
                    var json = JsonConvert.SerializeObject(BodyNames);
                    if (json != preJson)
                    {
                        if (_streamWriter != null)
                        {
                            _streamWriter.BaseStream?.SetLength(0);
                            _streamWriter.BaseStream?.Seek(0, SeekOrigin.Begin);
                            _streamWriter.Write(json);
                            _streamWriter.Flush();
                        }
                        preJson = json;
                    }
                }
                catch (Exception e)
                {
                }
            }
        }
    }
}
