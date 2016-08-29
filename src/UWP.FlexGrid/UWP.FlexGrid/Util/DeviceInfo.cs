using System;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.System.Profile;
using Windows.UI.ViewManagement;

namespace UWP.FlexGrid.Util
{
    /// <summary>
    /// Device information
    /// </summary>
    public static class DeviceInfo
    {
        public static readonly string DeviceId;

        public static readonly string UserAgent;

        public static readonly string OsVersion;

        public static readonly Size DeviceResolution;

        public static readonly string Timezone;

        public static readonly string Language;

        public static readonly WindowsDeviceType DeviceType;

        public static readonly Size DeviceScreenSize;

        public static UserInteractionMode InteractionMode
        {
            get
            {
                return UIViewSettings.GetForCurrentView().UserInteractionMode;
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
            DeviceType = GetDeviceType();
        }

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

        private static WindowsDeviceType GetDeviceType()
        {
            var deviceFamily = AnalyticsInfo.VersionInfo.DeviceFamily;

            if (deviceFamily == "Windows.Desktop")
            {
                return WindowsDeviceType.Desktop;
            }
            else if (deviceFamily == "Windows.Mobile")
            {
                return WindowsDeviceType.Mobile;
            }
            else if (deviceFamily == "Windows.Xbox")
            {
                return WindowsDeviceType.XBox;
            }
            else if (deviceFamily == "Windows.IoT")
            {
                return WindowsDeviceType.IOT;
            }
            else
            {
                throw new ArgumentException("Unkonw device family.");
            }
        }

        private static string GetLanguage()
        {
            var Languages = Windows.System.UserProfile.GlobalizationPreferences.Languages;
            if (Languages.Count > 0)
            {
                return Languages[0];
            }
            return Windows.Globalization.Language.CurrentInputMethodLanguageTag;
        }

        private static string GetTimezone()
        {
            return TimeZoneInfo.Local.DisplayName;
        }

        public static Size GetDeviceResolution()
        {
            Size resolution = Size.Empty;
            var rawPixelsPerViewPixel = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
            resolution.Width = DeviceScreenSize.Width * rawPixelsPerViewPixel;
            resolution.Height = DeviceScreenSize.Height * rawPixelsPerViewPixel;
            return resolution;
        }

        private static string GetDeviceId()
        {
            HardwareToken token = HardwareIdentification.GetPackageSpecificToken(null);
            return CryptographyHelper.Md5Encrypt(token.Id);
        }

        private static string GetUserAgent()
        {
            var Info = new EasClientDeviceInformation();
            return $"{Info.SystemManufacturer} {Info.SystemProductName}";
        }

        private static string GetOsVersion()
        {
            ulong version = Convert.ToUInt64(AnalyticsInfo.VersionInfo.DeviceFamilyVersion);
            return $"{version >> 48 & 0xFFFF}.{version >> 32 & 0xFFFF}.{version >> 16 & 0xFFFF}.{version & 0xFFFF}";
        }
    }

    public enum WindowsDeviceType
    {
        // HoloLens,
        // Surface Hub,
        Desktop,
        Mobile,
        XBox,
        IOT,
        IotHeadless
    }
}
