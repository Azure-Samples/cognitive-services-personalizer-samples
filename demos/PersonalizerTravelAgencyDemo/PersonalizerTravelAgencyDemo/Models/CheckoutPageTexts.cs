using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalizerTravelAgencyDemo.Models
{
    public class CheckoutPageText
    {
        public string Title { get; set; }
        public string PackageTitle { get; set; }
        public List<string> PackageItems { get; set; }
        public string CostsTitle { get; set; }
        public List<string> CostsItems { get; set; }
        public string DiscoverTitle { get; set; }
        public List<string> DiscoverItems { get; set; }
        public string PaymentTitle { get; set; }
        public string PaymentText { get; set; }
        public string ConfirmButtonLabel { get; set; }
        public string SaveForLaterLinkLabel { get; set; }
    }
}
