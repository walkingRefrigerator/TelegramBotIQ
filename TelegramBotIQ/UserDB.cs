namespace TelegramBotIQ
{
    internal class UserDB
    {
        private string UserName;
        private string Password;

        public void SetUser(string username)
            => UserName = username;

        public void SetPass(string password)
            => Password = password;

        public string GetUser()
        {
            return UserName;
        }

        public string GetPass()
        {
            return Password;
        }

        public UserDB()
        { }
    }
}
