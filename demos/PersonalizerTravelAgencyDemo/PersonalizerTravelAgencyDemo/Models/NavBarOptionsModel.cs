using System.Collections.Generic;

namespace PersonalizerTravelAgencyDemo.Models
{
    public class NavBarOptionsModel
    {
        public string LogoImageURL { get; set; }
        public string BackstageLabelBtn { get; set; }
        public string BackgroundColor { get; set; }
        public List<NavBarItemModel> OptionsList { get; set; }
    }
}
