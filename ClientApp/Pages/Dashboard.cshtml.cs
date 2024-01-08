using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClientApp.Pages
{
    public class DashboardModel : PageModel
    {
        public string t = "Hello from DashboardModel!";

        public IActionResult OnGet()
        {
            if (!true /*TODO if user is NOT authenticated*/)
            {
                return LocalRedirect("/login");
            }

            return Page();
        }
    }
}
