using ChatApp.Models;

namespace ChatApp.Services
{
    public class ChatHubService
    {

        #region Variables
        private List<UserMessage> userMessages = new();
        public event Action Notify;

        #endregion


        #region Properties
        public List<UserMessage> UserMessages { get { return userMessages; } set { userMessages = value; } }

        #endregion


        #region Methods
        /// <summary>
        /// Method that stores the message on the list
        /// </summary>
        /// <param name="msg">Message sent with all the information associated with it</param>
        public async Task Send(UserMessage msg)
        {
            await Task.Run(() => userMessages.Add(msg));

            Notify?.Invoke();
        }

        #endregion
    }
}