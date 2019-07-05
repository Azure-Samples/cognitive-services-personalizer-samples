using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalizerBusinessDemo.Models
{
    public class NavBarOptionsModel
    {
        public string LogoImageURL { get; set; }
        public string BackgroundColor { get; set; }

        public string BackstageLabelBtn { get; set; }
        public List<NavBarItemModel> OptionsList { get; set; }
    }
}
