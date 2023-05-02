using IdentityProvider.Context;
using IdentityProvider.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace IdentityProvider.Manager;

public class ApplicationManager : UserManager<ApplicationUser>
{
    private readonly ApplicationDbContext _context;
    public ApplicationManager(
        IUserStore<ApplicationUser> store, 
        IOptions<IdentityOptions> optionsAccessor, 
        IPasswordHasher<ApplicationUser> passwordHasher, 
        IEnumerable<IUserValidator<ApplicationUser>> userValidators, 
        IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators, 
        ILookupNormalizer keyNormalizer, 
        IdentityErrorDescriber errors, 
        IServiceProvider services, 
        ILogger<UserManager<ApplicationUser>> logger,
        ApplicationDbContext context
        ) 
        : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
    {
        _context = context;
    }


    public async Task<UserProfile?> GetUserProfile(string userId)
    {
        ApplicationUser ? user = await base.FindByIdAsync(userId);
        if(user is not null)
        {
            _context.Entry(user).Reference(x=>x.Profile).Load();
            return user.Profile;
        }
        return null;
    }
}