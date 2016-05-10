using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace MyUWPToolkit.Util
{
    internal static class Util
    {

        public static bool IsPrimitive(Type t)
        {
            return
                t == null ? false :
                t == typeof(string) ? true :
                 t.GetTypeInfo().IsPrimitive;
        }
    }
}
