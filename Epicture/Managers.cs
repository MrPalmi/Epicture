﻿
namespace Epicture
{
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
        public UserManager user;
        public CacheManager cache;

        private Managers()
        {
            flicker = new FlickerManager();
            user = new UserManager();
            cache = new CacheManager();
        }
    }
}
