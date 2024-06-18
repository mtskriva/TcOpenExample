using TcOpen.Inxton.Local.Security;
using TcOpen.Inxton.Security;
using ukazkaPlcConnector;

namespace ukazkaHmi.Wpf
{
    public class Roles
    {
        private Roles()
        {
            SecurityManager.Manager.GetOrCreateRole(new Role(write_yours_own_role_here, DefaultGroups.Administrator));
         
        }
        public const string write_yours_own_role_here = nameof(write_yours_own_role_here);
    
        private static Roles _roles;

        public static void Create()
        {
            _roles = new Roles();
        }
        
    }
}
