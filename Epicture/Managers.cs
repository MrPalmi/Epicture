
namespace Epicture
{
    enum SERVICE
    {
        FLICKR,
        IMGUR
    };

    class Managers
    {
        private static Managers _instance;

        public static Managers Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Managers();
                }
                return _instance;
            }
        }

        public FlickerManager flicker;
        public ImgurManager imgur;
        public UserManager user;
        public CacheManager cache;
        public NavigationManager nav;

        public SERVICE service;

        private Managers()
        {
            flicker = new FlickerManager();
            imgur = new ImgurManager();
            user = new UserManager();
            cache = new CacheManager();
            nav = new NavigationManager();
            service = SERVICE.FLICKR;
        }
    }
}
