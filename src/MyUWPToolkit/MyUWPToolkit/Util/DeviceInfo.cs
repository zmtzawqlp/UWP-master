using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.System.Profile;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace MyUWPToolkit.Util
{
    /// <summary>
    /// 设备信息
    /// </summary>
    public static class DeviceInfo
    {
        /// <summary>
        /// 设备ID
        /// </summary>
        public static readonly string DeviceId;

        /// <summary>
        /// 用户代理
        /// </summary>
        public static readonly string UserAgent;

        /// <summary>
        /// 操作系统版本
        /// </summary>
        public static readonly string OsVersion;

        /// <summary>
        /// 设备分辨率
        /// </summary>
        public static readonly Size DeviceResolution;

        /// <summary>
        /// 设备时区名字
        /// </summary>
        public static readonly string Timezone;

        /// <summary>
        /// 设备语言
        /// </summary>
        public static readonly string Language;

        /// <summary>
        /// 设备家族
        /// </summary>
        public static readonly WindowsDeviceFamily DeviceFamily;

        /// <summary>
        /// 设备屏幕大小
        /// </summary>
        public static readonly Size DeviceScreenSize;

        public static readonly double WideViewMinWidth;

        public static UserInteractionMode InteractionMode
        {
            get
            {
                return UIViewSettings.GetForCurrentView().UserInteractionMode;
            }
        }


        public static bool IsNarrowSrceen
        {
            get
            {
                //https://msdn.microsoft.com/zh-cn/library/windows/hardware/dn917883(v=vs.85).aspx
                switch (UIViewSettings.GetForCurrentView().UserInteractionMode)
                {
                    case UserInteractionMode.Mouse:
                        return false;
                    case UserInteractionMode.Touch:
                        return DeviceFamily == WindowsDeviceFamily.Mobile;
                    default:
                        return (Window.Current.Bounds.Width < WideViewMinWidth);
                }
            }
        }

        static DeviceInfo()
        {
            DeviceId = GetDeviceId();
            UserAgent = GetUserAgent();
            OsVersion = GetOsVersion();
            DeviceScreenSize = GetDeviceScreenSize();
            DeviceResolution = GetDeviceResolution();
            Timezone = GetTimezone();
            Language = GetLanguage();
            DeviceFamily = GetDeviceFamily();
            WideViewMinWidth = GetWideViewMinWidth();
        }

        private static double GetWideViewMinWidth()
        {
            double wideViewMinWidth = 640;
            if (Application.Current.Resources.ContainsKey("WideViewMinWidth"))
            {
                wideViewMinWidth = (double)Application.Current.Resources["WideViewMinWidth"];
            }
            return wideViewMinWidth;
        }

        /// <summary>
        /// 获取设备屏幕大小
        /// </summary>
        /// <returns></returns>
        private static Size GetDeviceScreenSize()
        {
            Size resolution = Size.Empty;
            foreach (var item in PointerDevice.GetPointerDevices())
            {
                resolution.Width = item.ScreenRect.Width;
                resolution.Height = item.ScreenRect.Height;
                break;
            }
            return resolution;
        }

        /// <summary>
        /// 获取设备家族
        /// </summary>
        /// <returns>设备家族</returns>
        private static WindowsDeviceFamily GetDeviceFamily()
        {
            switch (AnalyticsInfo.VersionInfo.DeviceFamily)
            {
                case "Windows.Desktop": return WindowsDeviceFamily.Desktop;
                case "Windows.Mobile": return WindowsDeviceFamily.Mobile;
                case "Windows.Xbox": return WindowsDeviceFamily.Xbox;
                case "Windows.IoT": return WindowsDeviceFamily.IoT;
                default: return WindowsDeviceFamily.Unknown;
            }
        }

        /// <summary>
        /// 获取设备语言
        /// </summary>
        /// <returns>设备语言</returns>
        private static string GetLanguage()
        {
            var Languages = Windows.System.UserProfile.GlobalizationPreferences.Languages;
            if (Languages.Count > 0)
            {
                return Languages[0];
            }
            return Windows.Globalization.Language.CurrentInputMethodLanguageTag;
        }

        /// <summary>
        /// 获取设备时区名字
        /// </summary>
        /// <returns>设备时区名字</returns>
        private static string GetTimezone()
        {
            return TimeZoneInfo.Local.DisplayName;
        }

        /// <summary>
        /// 获取设备分辨率
        /// </summary>
        /// <returns>设备分辨率</returns>
        public static Size GetDeviceResolution()
        {
            Size resolution = Size.Empty;
            var rawPixelsPerViewPixel = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
            resolution.Width = DeviceScreenSize.Width * rawPixelsPerViewPixel;
            resolution.Height = DeviceScreenSize.Height * rawPixelsPerViewPixel;
            return resolution;
        }

        /// <summary>
        /// 获取设备ID
        /// </summary>
        /// <returns>设备ID</returns>
        private static string GetDeviceId()
        {
            HardwareToken token = HardwareIdentification.GetPackageSpecificToken(null);
            return CryptographyHelper.Md5Encrypt(token.Id);
        }

        /// <summary>
        /// 获取用户代理
        /// </summary>
        /// <returns>用户代理</returns>
        private static string GetUserAgent()
        {
            var Info = new EasClientDeviceInformation();
            return $"{Info.SystemManufacturer} {Info.SystemProductName}";
        }

        /// <summary>
        /// 获取操作系统版本
        /// </summary>
        /// <returns>操作系统版本</returns>
        private static string GetOsVersion()
        {
            ulong version = Convert.ToUInt64(AnalyticsInfo.VersionInfo.DeviceFamilyVersion);
            return $"{version >> 48 & 0xFFFF}.{version >> 32 & 0xFFFF}.{version >> 16 & 0xFFFF}.{version & 0xFFFF}";
        }
    }

    /// <summary>
    /// Windows设备家族
    /// </summary>
    public enum WindowsDeviceFamily
    {
        Unknown,
        Desktop,
        Mobile,
        Xbox,
        IoT,
        //IoTHeadless,
        //HoloLens,
        //Surface Hub
    }
}
