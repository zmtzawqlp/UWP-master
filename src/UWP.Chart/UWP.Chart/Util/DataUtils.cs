using System;
using System.Collections;
using System.Collections.Generic;

namespace UWP.Chart.Util
{
    internal static class DataUtils
    {
        static List<Type> list = new List<Type>();

        public static void TryReset(IEnumerator enumerator)
        {
            // keep list of types not supported Reset()
            var type = enumerator.GetType();
            if (list.Contains(type))
                return;

            try
            {
                enumerator.Reset();
            }
            catch
            {
                list.Add(type);
            }
        }
    }
}
