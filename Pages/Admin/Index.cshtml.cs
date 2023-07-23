using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Coffeeroom.Pages.Admin
{
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
            if (HttpContext.Session.GetString("role") != "admin")
            {
                Response.Redirect("/account");
            }
        }
    }
}
