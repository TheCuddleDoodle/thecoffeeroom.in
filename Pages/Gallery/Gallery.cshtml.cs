using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Coffeeroom.Pages.Gallery
{
    public class GalleryModel : PageModel
    {
        public string Urlhandle { get; set; }
        public string Year{ get; set; }
        public void OnGet(string urlhandle,string year)
        {
            Urlhandle = urlhandle;
            Year = year;
        }
    }
}
