using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Coffeeroom.Pages
{
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
