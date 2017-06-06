using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Alfa_1.Configuration
{
    public class SmtpConfig
    {
        public string Server { get; set; }
        public string User { get; set; }
        public string Pass { get; set; }
        public int Port { get; set; }
        public string Subject { get; set; }
    }
}
