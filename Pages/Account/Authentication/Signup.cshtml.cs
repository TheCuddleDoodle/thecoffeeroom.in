using Coffeeroom.Core.Helpers;
using Coffeeroom.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using System.Data;
using Serilog;
using Coffeeroom.Models.View;

namespace Coffeeroom.Pages.Account
{
  

    [ValidateAntiForgeryToken]
    public class SignupModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
