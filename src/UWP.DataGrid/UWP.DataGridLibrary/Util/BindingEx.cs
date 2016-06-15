using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace UWP.DataGridLibrary.DataGrid.Util
{
    internal static class BindingEx
    {
        public static object Execute<T>(this Binding binding, object source)
        {

            return source.GetPropertyValue<T>(binding.Path.Path, binding.Converter, binding.ConverterParameter, binding.ConverterLanguage);

        }
    }
}
