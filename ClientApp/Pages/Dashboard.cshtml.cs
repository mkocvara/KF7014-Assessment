using EasyNetQ;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClientApp.Pages
{
    public class DashboardModel : PageModel
    {
        private readonly IBus _eventBus;
        static private List<SubscriptionResult> _subscriptions = new();
        
        public readonly int DefaultUpdateInterval = 10000; // miliseconds

        public static int SeverePrecipitationAlertId { get; set; } = 0;
        //public bool SevereTemperatureAlertId { get; set; } = false;
        //public bool SevereHumidityAlertID { get; set; } = false;

        public DashboardModel(IBus eventBus)
        {
            _eventBus = eventBus;

            // Close obsolete subscription channels
            foreach (SubscriptionResult subscription in _subscriptions)
            {
                subscription.Dispose();
            }
            _subscriptions.Clear();

            try
            {
                _subscriptions.Add(_eventBus.PubSub.Subscribe<int>("PrecipitationSevereRisk", HandlePrecipSevereRiskMessage, x => x.WithTopic("Precipitation")));
                //_subscriptions.Add(_eventBus.PubSub.Subscribe<int>("TemperatureSevereRisk", HandleTempSevereRiskMessage, x => x.WithTopic("Temperature")));
                //_subscriptions.Add(_eventBus.PubSub.Subscribe<int>("HumiditySevereRisk", HandleHumiditySevereRiskMessage, x => x.WithTopic("Humidity")));
            }
            catch
            {
                Console.WriteLine($"Could not subscribe to event bus; it is possible RabbitMQ is not running.");
            }
        }

        public IActionResult OnGet()
        {
            if (!true /*TODO if user is NOT authenticated*/)
            {
                return LocalRedirect("/login");
            }

            return Page();
        }

        // Gets the partial view for Precipitation Dashboard
        public PartialViewResult OnGetPrecipitationDash()
        {
            PartialViewResult partial = Partial("_PrecipitationDash", SeverePrecipitationAlertId);
            return partial;
        }

        void HandlePrecipSevereRiskMessage(int message)
        {
            SeverePrecipitationAlertId = message;

#if DEBUG
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"=== MESSAGE RECEIVED; measurement with ID={message} poses a severe risk! ===");
            Console.ResetColor();
#endif
        }
    }
}
