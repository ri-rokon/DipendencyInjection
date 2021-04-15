using DependencyInjection.Models;
using DependencyInjection.Utility.AppSettingsClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DependencyInjection.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public readonly WazeForecastSettings _wazeForecast;
        public readonly StripeSettings _stripeOptions;
        public readonly SendGridSettings _sendGridOptions;
        public readonly TwilioSettings _twilioOptions;
        public HomeController(ILogger<HomeController> logger,IOptions<WazeForecastSettings> wazeForecast, 
            IOptions<StripeSettings> stripe, 
            IOptions<SendGridSettings> sendGrid,
            IOptions<TwilioSettings> twilio)
        {
            _logger = logger;
            _wazeForecast = wazeForecast.Value;
            _stripeOptions = stripe.Value;
            _sendGridOptions = sendGrid.Value;
            _twilioOptions = twilio.Value;
        }

        public IActionResult AllConfigSettings()
        {
            List<string> messages = new List<string>();
            messages.Add($"WazeForecastSettings Enable?: " + _wazeForecast.ForecastTrackerEnabled);
            messages.Add($"Stripe Publishable Key: " + _stripeOptions.PublishableKey);
            messages.Add($"Stripe Secret Key: " + _stripeOptions.SecretKey);
            messages.Add($"Send Grid Key: " + _sendGridOptions.SendGridKey);
            messages.Add($"Twilio Phone: " + _twilioOptions.PhoneNumber);
            messages.Add($"Twilio SID: " + _twilioOptions.AccountSid);
            messages.Add($"Twilio Token: " + _twilioOptions.AuthToken);
            return View(messages);
        }

        public IActionResult Index()
        {
            return View();
        }


        public IActionResult viewInjection()
        {
            return View(new  DependencyInjection.Utility.AppSettingsClasses.WazeForecastSettings());
           // return View();

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
