using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClientApp.Pages
{
    public class IndexModel : PageModel
    {
        public IndexModel()
        {
        }

        public IActionResult OnGet()
        {
            if (/* TODO: User is logged in */ true)
                return LocalRedirect("/dashboard");
            else
                return LocalRedirect("/login");
        }
    }
}