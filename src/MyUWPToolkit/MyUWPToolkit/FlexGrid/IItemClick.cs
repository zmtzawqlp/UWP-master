using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyUWPToolkit.FlexGrid
{
    public interface IItemClick
    {
        event FlexGridItemClickEventHandler ItemClick;
        void OnItemClick(object clickedItem, object originalSource);
    }
}
