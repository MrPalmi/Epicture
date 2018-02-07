using FlickrNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epicture
{
    class CacheManager
    {
        public PhotoCollection Favorite;

        public CacheManager()
        {
        }

        public bool ReloadFavoriteCache()
        {
            if (Managers.Instance.user.Connected)
            {
                Favorite = Managers.Instance.flicker.flickr.FavoritesGetList(Managers.Instance.flicker.accessToken.UserId);
                return true;
            }
            return false;
        }
    }
}
