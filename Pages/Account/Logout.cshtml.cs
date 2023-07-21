using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Coffeeroom.Pages.Account
{
    public class LogoutModel : PageModel
    {
        public void OnGet()
        {
            HttpContext.Session.Clear();
            Response.Redirect("/");
        }
    }
}
