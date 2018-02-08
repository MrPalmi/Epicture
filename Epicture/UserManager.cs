
namespace Epicture
{
    class UserManager
    {
        bool _connected;
        string _userName;
        string _token;
        bool _allowIndesirable;

        public bool Connected { get => _connected; set => _connected = value; }
        public string UserName { get => _userName; set => _userName = value; }
        public string Token { get => _token; set => _token = value; }
        public bool AllowIndesirable { get => _allowIndesirable; set => _allowIndesirable = value; }

        public UserManager()
        {
            _connected = false;
            _userName = "Gest";
            _allowIndesirable = true;
            _token = "";
        }
    }
}
