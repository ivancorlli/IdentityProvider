
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityProvider.Pages
{
    public class ConfirmPhone : PageModel
    {
        private readonly ILogger<ConfirmPhone> _logger;

        public ConfirmPhone(ILogger<ConfirmPhone> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}