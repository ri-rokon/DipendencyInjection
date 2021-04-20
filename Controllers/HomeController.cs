using DependencyInjection.Data;
using DependencyInjection.Models;
using DependencyInjection.Models.ViewModels;
using DependencyInjection.Service;
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

        private readonly ICreditValidator _creditValidator;

        private readonly ApplicationDbContext _context;

        private readonly IMarketForecaster _marketForecaster;
        public readonly WazeForecastSettings _wazeForecast;
        public readonly StripeSettings _stripeOptions;
        public readonly SendGridSettings _sendGridOptions;
        public readonly TwilioSettings _twilioOptions;

        [BindProperty]
        public CreditApplication creditModel { get; set; }
        public HomeController(IMarketForecaster marketForecaster, ILogger<HomeController> logger,
             ICreditValidator creditValidator,
             ApplicationDbContext context,

            IOptions<WazeForecastSettings> wazeForecast,
            IOptions<StripeSettings> stripe,
            IOptions<SendGridSettings> sendGrid,
            IOptions<TwilioSettings> twilio)
        {
            _context = context;

            _creditValidator = creditValidator;
            _marketForecaster = marketForecaster;
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
            _logger.LogInformation("home Controller Index option Call");
            HomeVM homeVM = new HomeVM();
            MarketResult currentMarket = _marketForecaster.GetMarketPrediction();

            switch (currentMarket.MarketCondition)
            {
                case MarketCondition.StableDown:
                    homeVM.MarketForecast = "Market shows signs to go down in a stable state! It is a not a good sign to apply for credit applications! But extra credit is always piece of mind if you have handy when you need it.";
                    break;
                case MarketCondition.StableUp:
                    homeVM.MarketForecast = "Market shows signs to go up in a stable state! It is a great sign to apply for credit applications!";
                    break;
                case MarketCondition.Volatile:
                    homeVM.MarketForecast = "Market shows signs of volatility. In uncertain times, it is good to have credit handy if you need extra funds!";
                    break;
                default:
                    homeVM.MarketForecast = "Apply for a credit card using our application!";
                    break;
            }
            _logger.LogInformation("home Controller Index option end");

            return View(homeVM);
        }


        public IActionResult CreditApplication()
        {
            return View(new CreditApplication());
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [ActionName("CreditApplication")]
        public async Task<IActionResult> CreditApplicationPOST([FromServices] Func<CreditApprovedEnum, ICreditApproved> _creditService)
        {
            if (ModelState.IsValid)
            {
                var (validationPassed, errorMessages) = await _creditValidator.PassAllValidations(creditModel);

                CreditResult creditResult = new CreditResult()
                {
                    ErrorList = errorMessages,
                    CreditID = 0,
                    Success = validationPassed
                };

                if (validationPassed)
                {
                    creditModel.CreditApproved = _creditService(creditModel.Salary > 50000 ?CreditApprovedEnum.High : CreditApprovedEnum.Low).GetCreditApproved(creditModel);

                    _context.CreditApplication.Add(creditModel);
                    _context.SaveChanges();
                    creditResult.CreditID = creditModel.Id;
                    creditResult.CreditApproved = creditModel.CreditApproved;

                    return RedirectToAction(nameof(CreditResult), creditResult);
                }
                else
                {
                    return RedirectToAction(nameof(CreditResult), creditResult);
                }

            }
            else
            {
                return View(creditModel);
            }
        }

        public IActionResult CreditResult(CreditResult creditResult)
        {

            return View(creditResult);
        }


        public IActionResult viewInjection()
        {
            return View(new DependencyInjection.Utility.AppSettingsClasses.WazeForecastSettings());
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
