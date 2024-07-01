using ChatApp.Models;

namespace ChatApp.Services
{
    public class UserManagementService
    {

        #region Variables
        private List<User> usersList = new();
        #endregion


        #region Properties
        public List<User> UsersList { get { return usersList; } set { usersList = value; } }
        #endregion


    }
}
