using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;

namespace MyUWPToolkit.Util
{
    public static class NetworkInfo
    {
        /// <summary>
        /// 网络是否可用
        /// </summary>
        public static bool IsNetworkAvailable
        {
            get
            {
                ConnectionProfile profile = NetworkInformation.GetInternetConnectionProfile();
                return (profile?.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess);
            }
        }

        /// <summary>
        /// 获取IP地址
        /// </summary>
        /// <returns>IP地址</returns>
        public static string GetIpAddress()
        {
            Guid? networkAdapterId = NetworkInformation.GetInternetConnectionProfile()?.NetworkAdapter?.NetworkAdapterId;
            return (networkAdapterId.HasValue ? NetworkInformation.GetHostNames().FirstOrDefault(hn => hn?.IPInformation?.NetworkAdapter.NetworkAdapterId == networkAdapterId)?.CanonicalName : null);
        }

        /// <summary>
        /// 获取网络运营商信息
        /// </summary>
        /// <returns></returns>
        public static string GetNetworkName()
        {
            try
            {
                ConnectionProfile profile = NetworkInformation.GetInternetConnectionProfile();
                if (profile != null)
                {
                    if (profile.IsWwanConnectionProfile)
                    {
                        var homeProviderId = profile.WwanConnectionProfileDetails.HomeProviderId;
                        //4600是我手机测试出来的。
                        if (homeProviderId == "46000" || homeProviderId == "46002" || homeProviderId == "4600")
                        {
                            return "中国移动";
                        }
                        //已验证
                        else if (homeProviderId == "46001")
                        {
                            return "中国联通";
                        }
                        //貌似还没win10 电信手机。。待验证
                        else if (homeProviderId == "46003")
                        {
                            return "中国电信";
                        }
                    }
                    else
                    {
                        return "其他";
                    }
                    //也可以用下面的方法，已验证移动和联通
                    //var name = profile.GetNetworkNames().FirstOrDefault();
                    //if (name != null)
                    //{
                    //    name = name.ToUpper();
                    //    if (name == "CMCC")
                    //    {
                    //        return "中国移动";
                    //    }
                    //    else if (name == "UNICOM")
                    //    {
                    //        return "中国联通";
                    //    }
                    //    else if (name == "TELECOM")
                    //    {
                    //        return "中国电信";
                    //    }
                    //}
                    //return "其他";
                }

                return "其他";
            }
            catch (Exception)
            {

                return "其他";
            }

        }


        /// <summary>
        /// 获取网络连接类型
        /// </summary>
        /// <returns></returns>
        public static string GetNetWorkType()
        {
            try
            {
                ConnectionProfile profile = NetworkInformation.GetInternetConnectionProfile();
                if (profile == null)
                {
                    return "未知";
                }
                if (profile.IsWwanConnectionProfile)
                {
                    WwanDataClass connectionClass = profile.WwanConnectionProfileDetails.GetCurrentDataClass();
                    switch (connectionClass)
                    {
                        //2G-equivalent
                        case WwanDataClass.Edge:
                        case WwanDataClass.Gprs:
                            return "2G";
                        //3G-equivalent
                        case WwanDataClass.Cdma1xEvdo:
                        case WwanDataClass.Cdma1xEvdoRevA:
                        case WwanDataClass.Cdma1xEvdoRevB:
                        case WwanDataClass.Cdma1xEvdv:
                        case WwanDataClass.Cdma1xRtt:
                        case WwanDataClass.Cdma3xRtt:
                        case WwanDataClass.CdmaUmb:
                        case WwanDataClass.Umts:
                        case WwanDataClass.Hsdpa:
                        case WwanDataClass.Hsupa:
                            return "3G";
                        //4G-equivalent
                        case WwanDataClass.LteAdvanced:
                            return "4G";

                        //not connected
                        case WwanDataClass.None:
                            return "未连接";

                        //unknown
                        case WwanDataClass.Custom:
                        default:
                            return "未知";
                    }
                }
                else if (profile.IsWlanConnectionProfile)
                {
                    return "WIFI";
                }
                return "未知";
            }
            catch (Exception)
            {
                return "未知"; //as default
            }

        }
    }
}
