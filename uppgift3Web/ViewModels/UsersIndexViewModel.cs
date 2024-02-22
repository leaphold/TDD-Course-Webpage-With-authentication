using Microsoft.AspNetCore.Identity;

namespace uppgift3Web.ViewModels
{
    public class UsersIndexViewModel
    {

      public List<UserWithRoles> Users { get; set; } = new List<UserWithRoles>();

    }

    public class UserWithRoles
    {

      public IdentityUser User { get; set; }
      
      public List<string> Roles { get; set; }
      

    }
}
