using BodyNamed.Models;
using MyUWPToolkit.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace BodyNamed.Utils
{
    public class AppSettings
    {
        private static readonly ApplicationDataContainer Settings = ApplicationData.Current.LocalSettings;

        public static Gender BodyGender
        {
            get { return Settings.GetValue(defaultValue: Gender.Male); }
            set { Settings.SetValue((int)value); }
        }
    }
}
