using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace MyUWPToolkit.Common
{
    public static class ApplicationDataContainerExtensions
    {
        public static ApplicationDataContainer GetContainer(this ApplicationDataContainer container, string key)
            => container.Containers.ContainsKey(key) ? container.Containers[key] : null;

        public static T GetValue<T>(this ApplicationDataContainer container, [CallerMemberName] string key = null, T defaultValue = default(T))
            => container.Values.ContainsKey(key) ? (T)container.Values[key] : defaultValue;

        public static bool SetValue(this ApplicationDataContainer container, object value, [CallerMemberName] string key = null)
        {
            bool valueChanged = false;

            try
            {
                if (!Equals(container.Values[key], value))
                {
                    container.Values[key] = value;
                    valueChanged = true;
                }
            }
            catch(Exception e)
            {

            }

            return valueChanged;
        }
    }
}
