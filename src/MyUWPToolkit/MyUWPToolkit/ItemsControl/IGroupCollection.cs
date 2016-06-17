using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyUWPToolkit
{
    public interface IGroupCollection
    {
        List<GroupHeader> GroupHeaders { get; set; }
    }
    //
    public interface GroupHeader
    {
        string Name { get; set; }
        int FirstIndex { get; set; }
    }

    public class DefaultGroupHeader : GroupHeader
    {
        public string Name { get; set; }
        public int FirstIndex { get; set; }
        public DefaultGroupHeader()
        {
            FirstIndex = -1;
        }
    }
}
