using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP.ChartSample.Models;

namespace UWP.ChartSample.ViewModels
{
    public class BasicsSamplePageViewModel
    {
        public List<Product> Data { get; set; }

        Random rnd = new Random();
        public BasicsSamplePageViewModel()
        {
            Data = new List<Product>();
            for (int i = 0; i < 8; i++)
                Data.Add(new Product() { Name = "Product " + i, Price = rnd.Next(5, 50) });
        }
    }

}
