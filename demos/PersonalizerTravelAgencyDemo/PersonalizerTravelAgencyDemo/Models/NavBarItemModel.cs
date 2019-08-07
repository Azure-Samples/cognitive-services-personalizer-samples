using System.Collections.Generic;

namespace PersonalizerTravelAgencyDemo.Models
{
    public class NavBarItemModel
    {
        public string Label { get; set; }
        public List<NavBarItemModel> Items { get; set; }
    }
}
