
namespace Epicture
{
    class UserManager
    {
        bool _fconnected;
        bool _iconnected;
        string _userName;
        string _token;
        bool _allowIndesirable;

        public bool Connected
        {
            get
            {
                switch (Managers.Instance.service)
                {
                    case SERVICE.FLICKR:
                        return _fconnected;
                    case SERVICE.IMGUR:
                        return _iconnected;
                }
                return false;
            }
            set
            {
                switch (Managers.Instance.service)
                {
                    case SERVICE.FLICKR:
                        _fconnected = value;
                        break;
                    case SERVICE.IMGUR:
                        _iconnected = value;
                        break;
                }
            }
        }
        public string UserName { get => _userName; set => _userName = value; }
        public string Token { get => _token; set => _token = value; }
        public bool AllowIndesirable { get => _allowIndesirable; set => _allowIndesirable = value; }

        public UserManager()
        {
            _fconnected = false;
            _iconnected = false;
            _userName = "Gest";
            _allowIndesirable = true;
            _token = "";
        }
    }
}
