using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Alfa_1.Models
{
    public class AppConfiguration
    {
        public string EmailUsername { get; set; }
        public string EmailHost { get; set; }
        public string EmailPassword { get; set; }
        public string EmailPort { get; set; }
        public string SiteTitle { get; set; }
        public string SiteUrl { get; set; }
        public string GoogleAnalyticsId { get; set; }
        public string Key { get; set; }
    }
}
