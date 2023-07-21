using Coffeeroom.Models.View;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Coffeeroom.Pages.Account
{
    public class IndexModel : PageModel
    {
       
        public IActionResult OnGet()
        {
            var sessionStat = HttpContext.Session.GetString("role");
            if (sessionStat != null)
            {
                if (sessionStat.ToString() == "user" || sessionStat.ToString() == "admin")
                {
                    return Page();
                }
                else
                {
                    return LocalRedirect("/account/login");
                }
                
            }
            else
            {
                return LocalRedirect("/account/login");
            }
        }
    }
}
