using System.Collections.Generic;

namespace PersonalizerBusinessDemo.Models
{
    public class NavBarItemModel
    {
        public string Label { get; set; }
        public List<NavBarItemModel> Items { get; set; }
    }
}
