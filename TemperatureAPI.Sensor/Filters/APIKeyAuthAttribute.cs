using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TemperatureAPI.Temperature.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class APIKeyAuthAttribute : Attribute, IAsyncActionFilter
    {
        private const string APIKeyHeaderName = "APIKey";
        private string SettingName;

        public APIKeyAuthAttribute(string role)
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
            var APIKey = configuration.GetValue<string>(SettingName);

            if (!SubmittedAPIKey.Equals(APIKey))
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            await next();
        }
    }
}
