using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalizerDemo.Models
{
    public class UserContext
    {
        public string WeekDay { get; set; }

        public bool UseUserAgent { get; set; }

        public bool UseTextAnalytics { get; set; }
    }
}

