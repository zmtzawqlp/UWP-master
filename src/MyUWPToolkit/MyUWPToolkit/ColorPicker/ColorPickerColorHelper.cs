using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI;
using MyUWPToolkit.Common;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace MyUWPToolkit
{
    internal static class ColorPickerColorHelper
    {
        const string ColorPickerRecentColorsKey = "ColorPickerRecentColors.json";
        private static ObservableCollection<Color> RecentColors;
        //private static List<Color> systemColors;
        //private static List<Color> basicColors;
        private static bool hasLoadedRecentColors;

        //public static List<Color> BasicColors
        //{
        //    get
        //    {
        //        return basicColors;
        //    }
        //}

        static ColorPickerColorHelper()
        {
            //basicColors = new List<Color>();
            RecentColors = new ObservableCollection<Color>();
            //systemColors = new List<Color>();
            //foreach (var color in typeof(Colors).GetRuntimeProperties())
            //{
            //    basicColors.Add((Color)color.GetValue(null));
            //}

        }

        public static async Task<ObservableCollection<Color>> GetRecentColorsAsync()
        {
            if (!hasLoadedRecentColors)
            {
                hasLoadedRecentColors = true;
                RecentColors = await GetRecentColorsAsyncInternal();
                var temp = await GetRecentColorsAsyncInternal();
                if (temp != null)
                {
                    RecentColors = temp;
                }
            }
            return RecentColors;
        }

        public async static Task SetRecentColorsAsync(Color color)
        {
            if (RecentColors != null)
            {
                if (RecentColors.LastOrDefault() == color)
                {
                    return;
                }
                RecentColors.Add(color);
                if (RecentColors.Count > 8)
                {
                    RecentColors.RemoveAt(0);
                }
                await SaveRecentColorsAsync();
            }
        }

        private static async Task<ObservableCollection<Color>> GetRecentColorsAsyncInternal()
        {
            var jsonText = await StorageHelper.ReadFileAsync(ColorPickerRecentColorsKey);
            return JsonConvert.DeserializeObject<ObservableCollection<Color>>(jsonText);
        }

        private static async Task SaveRecentColorsAsync()
        {
            string jsonText = "";

            if (RecentColors.Count > 0)
            {
                jsonText = JsonConvert.SerializeObject(RecentColors);
            }
            await StorageHelper.WriteFileAsync(ColorPickerRecentColorsKey, jsonText);
        }
    }
}
