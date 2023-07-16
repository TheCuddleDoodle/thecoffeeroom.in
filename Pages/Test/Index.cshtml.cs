using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Reflection;

namespace Coffeeroom.Pages.Test
{
    public class Gdata
    {
        public string MyName { get; set; }
    }
    [ValidateAntiForgeryToken]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }

        public JsonResult OnPostSomething([FromBody] Gdata gdata)
        {
            var response = new
            {
                Message = "Data received successfully",
                Data = gdata.MyName,
                Type = "success"
            };
            return new JsonResult(response);
        }

        public JsonResult OnGetAnything()
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