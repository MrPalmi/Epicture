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
        public List<Photo> Favorite;
        public List<Photo> Indesirable;

        public CacheManager()
        {
            Favorite = new List<Photo>();
            Indesirable = new List<Photo>();
        }

        public bool LoadFavorite()
        {
            if (Managers.Instance.user.Connected)
            {
                var favorite = Managers.Instance.flicker.flickr.FavoritesGetList(Managers.Instance.flicker.accessToken.UserId);
                foreach (var it in favorite)
                {
                    if (!IsFavorite(it.PhotoId) && !IsIndesirable(it.PhotoId))
                        Favorite.Add(it);
                }
                return true;
            }
            return false;
        }

        public bool SaveFavorite()
        {
            if (Managers.Instance.user.Connected)
            {
                foreach (var it in Favorite)
                    Managers.Instance.flicker.flickr.FavoritesAdd(it.PhotoId);
                return true;
            }
            return false;
        }

        public bool IsFavorite(string id)
        {
            if (Favorite.FindIndex(x => x.PhotoId == id) == -1)
                return false;
            return true;
        }

        public bool AddFavorite(Photo photo)
        {
            if (IsFavorite(photo.PhotoId))
                return false;
            Favorite.Add(photo);
            return true;
        }

        public bool RemoveFavorite(Photo photo)
        {
            if (!IsFavorite(photo.PhotoId))
                return false;
            Favorite.Remove(photo);
            return true;
        }

        public void LoadIndesirable()
        {
        }

        public void SaveIndesirable()
        {
        }

        public bool IsIndesirable(string id)
        {
            if (Indesirable.FindIndex(x => x.PhotoId == id) == -1)
                return false;
            return true;
        }

        public bool AddIndesirable(Photo photo)
        {
            if (IsIndesirable(photo.PhotoId))
                return false;
            Indesirable.Add(photo);
            return true;
        }

        public bool RemoveIndesirable(Photo photo)
        {
            if (!IsIndesirable(photo.PhotoId))
                return false;
            Indesirable.Remove(photo);
            return true;
        }
    }
}
