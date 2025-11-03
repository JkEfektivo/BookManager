namespace BookManager.Services
{
    public interface ISessionService
    {
        bool IsAuthenticated { get; }
        string? CurrentUser { get; }
        void Login(string username);
        void Logout();
    }

    public class SessionService : ISessionService
    {
        private bool _isAuthenticated = false;
        private string? _currentUser = null;

        public bool IsAuthenticated => _isAuthenticated;
        public string? CurrentUser => _currentUser;

        public void Login(string username)
        {
            _isAuthenticated = true;
            _currentUser = username;
        }

        public void Logout()
        {
            _isAuthenticated = false;
            _currentUser = null;
        }
    }
}
