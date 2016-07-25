using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyUWPToolkit
{
    public interface IResizable
    {
        /// <summary>
        /// 
        /// </summary>
        int Width { get; set; }

        /// <summary>
        /// 
        /// </summary>
        int Height { get; set; }
    }


    public class ResizeableItems : List<ResizeableItem>
    {

        public int RowItemsCount
        {
            get
            {
                foreach (var item in this)
                {
                    return item.Items.Count;
                }

                return 0;
            }
        }

        public ResizeableItem GetItem(double width)
        {
            foreach (var item in this)
            {
                if (item.Min<= width && item.Max >= width)
                {
                    return item;
                }
            }

            var min = this.Min(x => x.Min);
            if (width < min)
            {
                return this.FirstOrDefault(x => x.Min == min);
            }
            var max = this.Max(x => x.Max);
            if (width > max)
            {
                return this.FirstOrDefault(x => x.Max == max);
            }
            return null;
        }
    }

    public class ResizeableItem
    {
        public int Columns { get; set; }

        public int ItemWidth { get; set; }

        public List<Resizable> Items {get;set; }

        public double Min { get; set; }

        public double Max { get; set; }
    }

    public class Resizable
    {
        /// <summary>
        /// 
        /// </summary>
       public int Width { get; set; }

        /// <summary>
        /// 
        /// </summary>
       public int Height { get; set; }
    }

    public interface IResizeableItems
    {
        int RowItemsCount { get;}
        ResizeableItems ResizeableItems { get; set; }
    }
}
