using IdentityProvider.Enum;
using IdentityProvider.ValueObject;
using Microsoft.AspNetCore.Identity;

namespace IdentityProvider.Entity
{
    public class ApplicationUser : IdentityUser
    {
        //public UserStatus Status { get; private set; }
        //public UserProfile? Profile { get; private set; }
        //public IEnumerable<Access> Permissions => _permissions;

        // It's distiguish between users that have been authenticated by a Social Provider, they are not going to use two factor, becouse theirs provider is already implenenting it.
        public bool IsAuthenticatedExternaly {get;private set;}

        // OWn Config //
        //private List<Access> _permissions {get; set; } = new List<Access>();
        // End Config//

        public ApplicationUser(bool useExternalAuth)
        {
            IsAuthenticatedExternaly = useExternalAuth;
            if(IsAuthenticatedExternaly)
            {
                TwoFactorEnabled = false;
            }else {
                TwoFactorEnabled = true;
            }
            //Status = UserStatus.Active;
        }

        /// <summary>
        /// Creates user progfile
        /// </summary>
        /// <param name="name"></param>
        /// <param name="gender"></param>
        // /// <param name="birth"></param>
        // public void CreateProfile(PersonName name, UserGender gender, DateTime birth)
        // {

        //     if (Profile is null)
        //     {
        //         var newProfile = new UserProfile(name, gender, birth);
        //         Profile = newProfile;
        //     }
        // }

        /// <summary>
        /// Asign permission to self resources or thirds resources
        /// </summary>
        /// <param name="access"></param>
        // public void AggregatePermission(Access access)
        // {
        //     // Buscamos si el permiso ya ha sido agregado previamente
        //     var exists = _permissions.Contains(access);
        //     // Si no ha sido agregado, lo agregamos
        //     if(!exists)
        //     {
        //         _permissions.Add(access);
        //     }
        // }

        /// <summary>
        /// Remove an assigned permission
        /// </summary>
        /// <param name="access"></param>
        // public void RemovePermission(Access access)
        // {
        //     // Buscamos si el permiso ya ha sido agregado previamente
        //     var exists = _permissions.Contains(access);
        //     // Si fue agreagdo, lo quitamos
        //     if(exists)
        //     {
        //         _permissions.Remove(access);
        //     }
        // }

        /// <summary>
        /// Deactive an assigned permission
        /// </summary>
        /// <param name="access"></param>
        // public void DeactivatePermission(Access access)
        // {
        //     // Buscamos si el permiso ya ha sido agregado previamente
        //     var exists = _permissions.Contains(access);
        //     if(exists)
        //     {
        //         // Buscamos el permiso
        //         for (int i = 0;i < _permissions.Count; i++)
        //         {
        //             // Lo actualizamos
        //             if(_permissions[i] == access)
        //             {
        //                 _permissions[i] = Access.Deactive(_permissions[i].PermissionId,_permissions[i].ResourceId);
        //             }
        //         }
        //     }
        // }
    }
}
