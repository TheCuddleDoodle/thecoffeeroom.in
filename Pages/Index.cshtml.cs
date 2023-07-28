using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Coffeeroom.Core.Helpers;
using Serilog;
using Serilog.Events;

namespace Coffeeroom.Pages
{
    public class IndexModel : PageModel
    {

        public void OnGet()
        {
        }

        public JsonResult OnPostSubscribe()
        {
        
            var response = new
            {
                Message = "GETTED data successfully",
                Type = "success"
            };
            return new JsonResult(response);
        }
    }
}
