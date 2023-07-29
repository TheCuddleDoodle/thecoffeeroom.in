using Coffeeroom.Core.Helpers;
using Coffeeroom.Models.Domain;
using Coffeeroom.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Coffeeroom.Pages.Account
{
    public class PasswordRecoveryModel : PageModel
    {
        public IActionResult OnGetAsync()
        {
            var sessionStat = HttpContext.Session.GetString("role");
            if (sessionStat != null)
            {
                if (sessionStat.ToString() == "guest")
                {
                    return LocalRedirect("/account/login");
                    
                }
                else
                {
                    return Page();
                }
                
            }
            else
            {
                return LocalRedirect("/account/login");
            }
        }

    }
}
