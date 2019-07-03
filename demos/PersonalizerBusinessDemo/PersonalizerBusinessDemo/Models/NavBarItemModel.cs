using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalizerBusinessDemo.Models
{
    public class NavBarItemModel
    {
        public string Label { get; set; }
        public List<NavBarItemModel> Items { get; set; }
    }
}
