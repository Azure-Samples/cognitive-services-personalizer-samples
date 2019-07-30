using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalizerTravelAgencyDemo.Models
{
    public class Action
    {
        public string Id { get; set; }
        public string ButtonColor { get; set; }
        public PartialImageModal Image { get; set; }
        public string ToneFont { get; set; }
        public string Layout { get; set; }
        public bool Enabled { get; set; }

    }
}
