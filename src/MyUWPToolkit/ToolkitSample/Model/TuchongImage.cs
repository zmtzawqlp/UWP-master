using MyUWPToolkit;
using MyUWPToolkit.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace ToolkitSample.Model
{
    public class TuchongImage: IResizable
    {
        
        public int post_id { get; set; }

        public string url { get; set; }

       
        public int image_count { get; set; }
        public string ImageUrl { get; set; }

        //public BitmapSource ImageSource { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int comments { get; internal set; }
        public int favorites { get; internal set; }

        public override bool Equals(object obj)
        {
            return (obj as TuchongImage).post_id == this.post_id;
            return base.Equals(obj);
        }
    }

    public class TuchongImageSource : IncrementalLoadingBase<TuchongImage>
    {
        int PageIndex = 1;
        protected override bool HasMoreItemsOverride()
        {
            return true;
        }

        protected async override Task<IEnumerable<TuchongImage>> LoadMoreItemsOverrideAsync(uint count, CancellationToken ct)
        {
            string url = "";
            if (this.Count == 0)
            {
                url = "https://api.tuchong.com/feed-app";
            }
            else
            {
                int lastPostId = this.LastOrDefault().post_id;
                url = "https://api.tuchong.com/feed-app?post_id=" + lastPostId + "&page=" + PageIndex + "&type=loadmore";
            }

            var temp = await TuchongHelper.GetTuchongImage(url);
            List<TuchongImage> result = new List<TuchongImage>();
            PageIndex++;
            foreach (var item in temp)
            {
                if (!this.Contains(item))
                {
                    result.Add(item);
                }
            }
            return result;
        }

        public void Reset()
        {
            PageIndex = 1;
            Clear();
        }
    }

    public class TuchongHelper
    {
        public async static Task<IEnumerable<TuchongImage>> GetTuchongImage(string url)
        {
            var json = await HttpClientHelper.GetStringAsync(new Uri(url));
            var res = JsonConvert.DeserializeObject<TuchongImageStandard>(json);

            List<TuchongImage> MineItems = new List<TuchongImage>();
            foreach (var item in res.feedList)
            {
                TuchongImage mine = new TuchongImage();
                try
                {
                    mine.post_id = item.post_id;
                    mine.url = item.url;
                    mine.image_count = item.image_count;
                    mine.favorites = item.favorites;
                    mine.comments = item.comments;
                    if (item.image_count <= 0)
                        continue;
                    //List<TuchongImageMine.Images> mineImages = new List<TuchongImageMine.Images>();
                    //foreach (var item0 in item.images)
                    //{
                    //    TuchongImageMine.Images ii = new TuchongImageMine.Images();
                    //    ii.img_realurl = "https://photo.tuchong.com/" + item0.user_id.ToString() + "/f/" + item0.img_id + ".jpg";
                    //    ii.img_id = item0.img_id;
                    //    ii.user_id = item0.user_id;
                    //    ii.title = item0.title;
                    //    ii.excerpt = item0.excerpt;
                    //    ii.width = item0.width;
                    //    ii.height = item0.height;
                    //    ii.description = item0.description;

                    //    mineImages.Add(ii);
                    //}
                    var tempurl = "https://photo.tuchong.com/" + item.images[0].user_id.ToString() + "/f/" + item.images[0].img_id + ".jpg";
                    mine.Width = item.images[0].width;
                    mine.Height = item.images[0].height;
                    mine.ImageUrl = tempurl;
                   // mine.ImageSource = await HttpClientHelper.GetImageAsync(tempurl);

                    mine.post_id = item.post_id;

                    mine.url = item.url;

                    MineItems.Add(mine);
                }
                catch (Exception ex)
                {

                }
            }

            return MineItems;
        }

    }

    public class TuchongImageStandard
    {
        public string is_history { get; set; }

        public int counts { get; set; }

        public List<FeedList> feedList { get; set; }

        public string message { get; set; }

        public string more { get; set; }

        public string result { get; set; }

        public class FeedList
        {

            public int post_id { get; set; }

            public string type { get; set; }

            public string url { get; set; }

            public string site_id { get; set; }

            public string author_id { get; set; }

            public string published_at { get; set; }

            public string passed_time { get; set; }

            public string excerpt { get; set; }

            public int favorites { get; set; }

            public int comments { get; set; }

            public string rewardable { get; set; }

            public string parent_comments { get; set; }

            public string rewards { get; set; }

            public int views { get; set; }

            public string collected { get; set; }

            public string delete { get; set; }

            public string update { get; set; }

            public string content { get; set; }

            public string title { get; set; }

            public int image_count { get; set; }

            public List<Images> images { get; set; }

            //public string title_image { get; set; }

            public List<string> tags { get; set; }

            public List<string> event_tags { get; set; }

            public List<string> favorite_list_prefix { get; set; }

            public List<string> reward_list_prefix { get; set; }

            public List<string> comment_list_prefix { get; set; }

            public string data_type { get; set; }

            public string created_at { get; set; }

            public List<string> sites { get; set; }

            public Site site { get; set; }

            public string recom_type { get; set; }

            public string rqt_id { get; set; }

            public string is_favorite { get; set; }
        }

        public class Images
        {

            public int img_id { get; set; }

            public int user_id { get; set; }

            public string title { get; set; }

            public string excerpt { get; set; }

            public int width { get; set; }

            public int height { get; set; }

            public string description { get; set; }
        }

        public class Site
        {

            public string site_id { get; set; }

            public string type { get; set; }

            public string name { get; set; }

            public string domain { get; set; }

            public string description { get; set; }

            public int followers { get; set; }

            public string url { get; set; }

            public string icon { get; set; }

            public string verified { get; set; }

            public int? verified_type { get; set; }

            public string verified_reason { get; set; }

            public int? verifications { get; set; }

            //public List<string> verification_list { get; set; }

            public string is_following { get; set; }
        }

    }
}
