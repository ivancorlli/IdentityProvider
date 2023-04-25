using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityProvider.Pages
{
    public class Index : PageModel
    {
        private readonly ILogger<Index> _logger;

        public Index(ILogger<Index> logger)
        {
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            return  Redirect("/Signin");
        }
    }
}