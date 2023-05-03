using IdentityProvider.Enumerables;
using IdentityProvider.ValueObject;
using Microsoft.AspNetCore.Identity;

namespace IdentityProvider.Entity
{
    public class ApplicationUser : IdentityUser
    {
        public UserStatus Status { get; private set; } = default!;
        // It distiguish between users that have been authenticated by a Social Provider, they are not going to use two factor, becouse theirs provider is already implenenting it.
        public bool IsAuthenticatedExternaly {get;private set;} = default!;
        public IEnumerable<Access> Permissions => _permissions;
        public UserProfile? Profile { get; private set; }
        // OWn Config //
        private List<Access> _permissions {get; set; } = new List<Access>();
        // End Config//

        private ApplicationUser(){}
        public static ApplicationUser CreateExternalUser(string email)
        {
            var newUser = new ApplicationUser
            {
                Status = UserStatus.Active,
                Email = email,
                EmailConfirmed = true,
                UserName =email,
                IsAuthenticatedExternaly = true,
                TwoFactorEnabled = false,
                LockoutEnabled=false
            };
            return newUser;
        }

        public static ApplicationUser CreateExternalUser(string email,string phone)
        {
            var newUser = new ApplicationUser
            {
                Status = UserStatus.Active,
                Email = email,
                EmailConfirmed = true,
                UserName =email,
                IsAuthenticatedExternaly = true,
                TwoFactorEnabled = false,
                PhoneNumber = phone,
                PhoneNumberConfirmed =true,
                LockoutEnabled=false
            };
            return newUser;
        }

        public static ApplicationUser CreateLocalUser(string email)
        {
            var newUser = new ApplicationUser {
                Status = UserStatus.Active,
                Email = email,
                UserName = email,
                IsAuthenticatedExternaly = false,
                TwoFactorEnabled = false,
                LockoutEnabled=false
            };
            return newUser;
        }


        public void UseTwoFactor()
        {
            TwoFactorEnabled = true;
        }

        public void DeactiveTwoFactor()
        {
            TwoFactorEnabled = false;
        }

        /// <summary>
        /// Creates user profile
        /// </summary>
        /// <param name="name"></param>
        // /// <param name="birth"></param>
        public void CreateProfile(PersonName name)
        {

            if (Profile is null)
            {
                var newProfile = new UserProfile(Id,name);
                Profile = newProfile;
            }
        }

        /// <summary>
        /// Asign permission to self resources or thirds resources
        /// </summary>
        /// <param name="access"></param>
        public void AggregatePermission(Access access)
        {
            // Buscamos si el permiso ya ha sido agregado previamente
            var exists = _permissions.Contains(access);
            // Si no ha sido agregado, lo agregamos
            if(!exists)
            {
                _permissions.Add(access);
            }
        }

        /// <summary>
        /// Remove an assigned permission
        /// </summary>
        /// <param name="access"></param>
        public void RemovePermission(Access access)
        {
            // Buscamos si el permiso ya ha sido agregado previamente
            var exists = _permissions.Contains(access);
            // Si fue agreagdo, lo quitamos
            if(exists)
            {
                _permissions.Remove(access);
            }
        }

        /// <summary>
        /// Deactive an assigned permission
        /// </summary>
        /// <param name="access"></param>
        public void DeactivatePermission(Access access)
        {
            // Buscamos si el permiso ya ha sido agregado previamente
            var exists = _permissions.Contains(access);
            if(exists)
            {
                // Buscamos el permiso
                for (int i = 0;i < _permissions.Count; i++)
                {
                    // Lo actualizamos
                    if(_permissions[i] == access)
                    {
                        _permissions[i] = Access.Deactive(_permissions[i].PermissionId,_permissions[i].ResourceId);
                    }
                }
            }
        }
    }
}
