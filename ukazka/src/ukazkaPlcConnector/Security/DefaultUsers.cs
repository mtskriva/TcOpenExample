using System;
using System.Collections.ObjectModel;
using TcOpen.Inxton;
using TcOpen.Inxton.Data;
using TcOpen.Inxton.Local.Security;
using TcOpen.Inxton.Security;

namespace ukazkaPlcConnector
{
    public class DefaultUsers
    {
        private DefaultUsers()
        {
            CreateUser(SecurityManager.Manager.UserRepository,"MTSAdmin","MTSservis",DefaultGroups.Administrator,true);
            CreateUser(SecurityManager.Manager.UserRepository, "operator", "", DefaultGroups.Operator,true);

        }



        private static DefaultUsers _users;


        private void CreateUser(IRepository<UserData> repository,string name, string password, string group,bool canChangePsw)
        {
            try
            {
                var newUser = new UserData();
                newUser.Username = name;
                newUser.SetPlainTextPassword(password);
                newUser._Created = DateTime.Now;
                newUser._Modified = DateTime.Now;
                newUser.CanUserChangePassword = canChangePsw;
               newUser.Roles = new ObservableCollection<string>(new string[] { group });
                newUser.Level = group;
                repository.Create(newUser.Username, newUser);

                TcoAppDomain.Current.Logger.Information($"New user '{newUser.Username}' created. {{@sender}}", new { UserName = newUser.Username, Roles = newUser.Roles });

            }
            catch (Exception ex)
            {
                TcoAppDomain.Current.Logger.Information($"Error creating new user: '{ex.Message}'", "Failed to create user");
            }
        }
        public static void Create()
        {
            _users = new DefaultUsers();
        }
        
    }
}
