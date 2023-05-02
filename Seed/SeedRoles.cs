using IdentityProvider.Constant;
using IdentityProvider.Context;
using IdentityProvider.Entity;
using Microsoft.AspNetCore.Identity;

namespace IdentityProvider.Seed;

public class SeedRoles : IHostedService
{
     private readonly IServiceProvider _serviceProvider;

    public SeedRoles(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    public async Task StartAsync(CancellationToken cancellationToken)
    {
       using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Database.EnsureCreatedAsync(cancellationToken);
        var manager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

        if(!await manager.RoleExistsAsync(DefaultRoles.DefaultUser))
        {
            await manager.CreateAsync(new ApplicationRole(DefaultRoles.DefaultUser,RolType.ApplicationRole));
        }



        if(!await manager.RoleExistsAsync(DefaultRoles.ApplicationUser))
        {
            await manager.CreateAsync(new ApplicationRole(DefaultRoles.ApplicationUser,RolType.ApplicationRole));
        }

        if(!await manager.RoleExistsAsync(DefaultRoles.IdentityAdmin))
        {
            await manager.CreateAsync(new ApplicationRole(DefaultRoles.IdentityAdmin,RolType.ProviderRole));
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}