using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace IdentityProvider.Pages
{
    public class Terms : PageModel
    {
        private readonly ILogger<Terms> _logger;

        public Terms(ILogger<Terms> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}