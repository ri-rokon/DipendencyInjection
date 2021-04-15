using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DependencyInjection.Utility.AppSettingsClasses
{
    public class TwilioSettings
    {
        public string PhoneNumber { get; set; }
        public string AuthToken { get; set; }
        public string AccountSid { get; set; }
    }
}
