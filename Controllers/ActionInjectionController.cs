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
using WazeCredit.Service.LifeTimeExample;

namespace DependencyInjection.Controllers
{
    public class ActionInjectionController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public ActionInjectionController(ILogger<HomeController> logger)
        {
            _logger = logger;

        }


        public IActionResult AllConfigSettings(ScopedServiceTestActionInjection actionInjection,[FromServices] IOptions<WazeForecastSettings> wazeForecast,
            [FromServices] IOptions<StripeSettings> stripe, [FromServices] IOptions<SendGridSettings> sendGrid,
            [FromServices] IOptions<TwilioSettings> twilio, SingletonService withoutservice, [FromServices] SingletonService singleton)
        {
            List<string> messages = new List<string>();
            messages.Add($"WazeForecastSettings Enable?: " + wazeForecast.Value.ForecastTrackerEnabled);
            messages.Add($"Stripe Publishable Key: " + stripe.Value.PublishableKey);
            messages.Add($"Stripe Secret Key: " + stripe.Value.SecretKey);
            messages.Add($"Send Grid Key: " + sendGrid.Value.SendGridKey);
            messages.Add($"Twilio Phone: " + twilio.Value.PhoneNumber);
            messages.Add($"Twilio SID: " + twilio.Value.AccountSid);
            messages.Add($"Twilio Token: " + twilio.Value.AuthToken);
            messages.Add($"ScopedServiceTestActionInjection value: " + actionInjection.GetGuid().ToString());
            messages.Add($"ScopedServiceTestActionInjection from without[FromServices] value: " + actionInjection.alert());
            messages.Add($"SingletonService from Service value: " + singleton.GetGuid().ToString());
            messages.Add($"SingletonService from without[FromServices] value: " + withoutservice.GetGuid().ToString());



            return View(messages);
        }

        public IActionResult Index()
        {
            return View();
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
