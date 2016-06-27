using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace MyUWPToolkit
{
    public interface IGroupCollection : ISupportIncrementalLoading
    {
        List<IGroupHeader> GroupHeaders { get; set; }
        int CurrentGroupIndex { get; }
    }

    public interface IGroupHeader
    {
        string Name { get; set; }
        int FirstIndex { get; set; }
        int LastIndex { get; set; }
        double Height { get; set; }
    }

    public class DefaultGroupHeader : IGroupHeader
    {
        public string Name { get; set; }
        public int FirstIndex { get; set; }
        public int LastIndex { get; set; }
        public double Height { get; set; }
        public DefaultGroupHeader()
        {
            FirstIndex = -1;
            LastIndex = -1;
        }
    }
}
