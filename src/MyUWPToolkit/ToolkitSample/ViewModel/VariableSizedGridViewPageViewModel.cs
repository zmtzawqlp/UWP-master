using MyUWPToolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolkitSample.Common;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace ToolkitSample.ViewModel
{

    public class Thing : IResizable
    {
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public ImageSource Image
        {
            get
            {
                return new BitmapImage(new Uri("ms-appx://" + this.ImagePath));
            }
        }


    }

    public class MainPageViewModel : BindableBase
    {
        public List<Thing> Things
        {
            get
            {
                List<Thing> results = new List<Thing>();

                results.Add(new Thing() { Name = "Beer", Height = 2, Width = 1, ImagePath = @"/Resource/Images/beer.jpg" });
                results.Add(new Thing() { Name = "Hops", Height = 2, Width = 2, ImagePath = @"/Resource/Images/hops.jpg" });
                results.Add(new Thing() { Name = "Malt", Height = 1, Width = 2, ImagePath = @"/Resource/Images/malt.jpg" });
                results.Add(new Thing() { Name = "Water", Height = 1, Width = 2, ImagePath = @"/Resource/Images/water.jpg" });
                results.Add(new Thing() { Name = "Yeast", Height = 1, Width = 1, ImagePath = @"/Resource/Images/yeast.jpg" });
                results.Add(new Thing() { Name = "Sugar", Height = 2, Width = 2, ImagePath = @"/Resource/Images/sugars.jpg" });
                //results.Add(new Thing() { Name = "Herbs", Height = 2, Width = 1, ImagePath = @"/Resource/Images/herbs.jpg" });
                //results.Add(new Thing() { Name = "Beer", Height = 2, Width = 2, ImagePath = @"/Resource/Images/beer.jpg" });
                //results.Add(new Thing() { Name = "Hops", Height = 1, Width = 1, ImagePath = @"/Resource/Images/hops.jpg" });
                //results.Add(new Thing() { Name = "Malt", Height = 1, Width = 1, ImagePath = @"/Resource/Images/malt.jpg" });
                //results.Add(new Thing() { Name = "Water", Height = 2, Width = 2, ImagePath = @"/Resource/Images/water.jpg" });
                //results.Add(new Thing() { Name = "Yeast", Height = 1, Width = 2, ImagePath = @"/Resource/Images/yeast.jpg" });
                //results.Add(new Thing() { Name = "Sugar", Height = 1, Width = 1, ImagePath = @"/Resource/Images/sugars.jpg" });
                //results.Add(new Thing() { Name = "Herbs", Height = 2, Width = 2, ImagePath = @"/Resource/Images/herbs.jpg" });
                //results.Add(new Thing() { Name = "Beer", Height = 1, Width = 2, ImagePath = @"/Resource/Images/beer.jpg" });
                //results.Add(new Thing() { Name = "Hops", Height = 1, Width = 1, ImagePath = @"/Resource/Images/hops.jpg" });
                //results.Add(new Thing() { Name = "Malt", Height = 2, Width = 2, ImagePath = @"/Resource/Images/malt.jpg" });
                //results.Add(new Thing() { Name = "Water", Height = 1, Width = 2, ImagePath = @"/Resource/Images/water.jpg" });
                //results.Add(new Thing() { Name = "Sugar", Height = 1, Width = 2, ImagePath = @"/Resource/Images/sugars.jpg" });
                //results.Add(new Thing() { Name = "Yeast", Height = 2, Width = 2, ImagePath = @"/Resource/Images/yeast.jpg" });
                //results.Add(new Thing() { Name = "Herbs", Height = 2, Width = 2, ImagePath = @"/Resource/Images/herbs.jpg" });

                return results;
            }
        }

        private Thing selectedThing;

        public Thing SelectedThing
        {
            get { return selectedThing; }
            set { this.SetProperty(ref this.selectedThing, value); }
        }
    }
}
