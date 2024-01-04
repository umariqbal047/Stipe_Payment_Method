using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;

using Stripe.Checkout;
using Stripe8.Models;
using System.Diagnostics;


namespace Stripe8.Controllers
{
    public class HomeController : Controller
    {

        private readonly StripeSettings _stripeSettings;

        public HomeController(IOptions<StripeSettings> stripeSettings)
        {
            _stripeSettings = stripeSettings.Value;

        }

        public IActionResult Index()
        {
            return View();
        }


        public IActionResult CreateCheckoutSession(string amount)
        {

            var currency = "usd"; // Currency code
            var successUrl = "https://localhost:7196/Home/success";
            var cancelUrl = "https://localhost:7196/Home/cancel";
            StripeConfiguration.ApiKey = _stripeSettings.SecretKey;

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string>
                {
                    "card"
                },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = currency,
                            UnitAmount = Convert.ToInt32(amount) * 100,  // Amount in smallest currency unit (e.g., cents)
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Product Name",
                                Description = "Product Description"
                            }
                        },
                        Quantity = 1
                    }
                },
                Mode = "payment",
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl
            };

            var service = new SessionService();
            var session = service.Create(options);


            return Redirect(session.Url);
        }

        public async Task<IActionResult> success()
        {

            return View("Index");
        }

        public IActionResult cancel()
        {
            return View("Index");
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
