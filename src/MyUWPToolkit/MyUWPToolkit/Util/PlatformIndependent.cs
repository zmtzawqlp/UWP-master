using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyUWPToolkit.Util
{
    public static class PlatformIndependent
    {
        /// <summary>
        /// Indicates whether the running device is windows phone device.
        /// </summary>
        internal static bool IsWindowsPhoneDevice
        {
            get
            {
                if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
                {
                    return true;
                }
                return false;
            }
        }
    }
}
