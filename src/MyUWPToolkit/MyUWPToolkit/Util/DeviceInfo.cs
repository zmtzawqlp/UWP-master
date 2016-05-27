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

        static DeviceInfo()
        {
            DeviceId = GetDeviceId();
            UserAgent = GetUserAgent();
            OsVersion = GetOsVersion();
            DeviceResolution = GetDeviceResolution();
        }
        /// <summary>
        /// 获取设备分辨率
        /// </summary>
        /// <returns>设备分辨率</returns>
        private static Size GetDeviceResolution()
        {
            Size resolution = Size.Empty;
            var rawPixelsPerViewPixel = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
            foreach (var item in PointerDevice.GetPointerDevices())
            {
                resolution.Width = item.ScreenRect.Width * rawPixelsPerViewPixel;
                resolution.Height = item.ScreenRect.Height * rawPixelsPerViewPixel;
                break;
            }
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
}
