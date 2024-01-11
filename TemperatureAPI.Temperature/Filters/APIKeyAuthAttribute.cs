using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TemperatureAPI.Temperature.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class APIKeyAuthAttribute : Attribute, IAsyncActionFilter
    {
        private const String APIKeyHeaderName = "APIKey";
        private String SettingName;

        public APIKeyAuthAttribute(String role)
        {
            switch (role.ToLower())
            {
                case "admin":
                    SettingName = "AdminKey";
                    break;
                case "sensor":
                    SettingName = "SensorKey";
                    break;
                default:
                    throw new ArgumentException();
            }
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // if no API key is supplied
            if (!context.HttpContext.Request.Headers.TryGetValue(APIKeyHeaderName, out var SubmittedAPIKey))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // if a wrong API key is supplied
            var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            string APIKey = configuration.GetValue<string>(SettingName);

            if (string.IsNullOrEmpty(APIKey)){
                if (SettingName == "AdminKey")
                {
                    APIKey = "0d9d9e10-5631-4f8c-a793-2bcef08cf2ae";
                } else if (SettingName == "SensorKey") {
                    APIKey = "85f82393-59ca-485e-99f3-01d9915bd195";
                }
            }

            Console.WriteLine($"{APIKey} - {SubmittedAPIKey}");

            if (!SubmittedAPIKey.Equals(APIKey))
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            await next();
        }
    }
}