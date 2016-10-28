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

namespace MyUWPToolkit
{
    internal static class ColorPickerColorHelper
    {
        const string ColorPickerCustomColorsKey = "ColorPickerCustomColors.json";
        private static List<Color> customColors;
        private static List<Color> systemColors;
        private static List<Color> basicColors;
        private static bool hasLoadedCustomColors;

        public static List<Color> BasicColors
        {
            get
            {
                return basicColors;
            }
        }

        static ColorPickerColorHelper()
        {
            basicColors = new List<Color>();
            customColors = new List<Color>();
            systemColors = new List<Color>();
            foreach (var color in typeof(Colors).GetRuntimeProperties())
            {
                basicColors.Add((Color)color.GetValue(null));
            }
          
        }

        public static async Task<List<Color>> GetCustomColorsAsync()
        {
            if (!hasLoadedCustomColors)
            {
                hasLoadedCustomColors = true;
                customColors = await GetCustomColorsAsyncInternal();
            }
            return customColors;
        }

        public async static Task SetCustomColorsAsync(Color color)
        {
            if (customColors != null)
            {
                customColors.Add(color);
                if (customColors.Count > 20)
                {
                    customColors.RemoveAt(0);
                }
                await SaveCustomColorsAsync();
            }
        }

        private static async Task<List<Color>> GetCustomColorsAsyncInternal()
        {
            var jsonText = await StorageHelper.ReadFileAsync(ColorPickerCustomColorsKey);
            return JsonConvert.DeserializeObject<List<Color>>(jsonText);
        }

        private static async Task SaveCustomColorsAsync()
        {
            string jsonText = "";

            if (customColors.Count > 0)
            {
                jsonText = JsonConvert.SerializeObject(customColors);
            }
            await StorageHelper.WriteFileAsync(ColorPickerCustomColorsKey, jsonText);
        }
    }
}
