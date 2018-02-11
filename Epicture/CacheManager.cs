using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Epicture
{
    class CacheManager
    {
        private List<string> Indesirable;
        public List<string> Favorite;

        public CacheManager()
        {
            Indesirable = new List<string>();

            Favorite = new List<string>();
        }

        public bool LoadFavorite()
        {
            Favorite.Clear();
            if (Managers.Instance.user.Connected)
            {
                var favorite = Managers.Instance.flicker.flickr.FavoritesGetList(Managers.Instance.flicker.accessToken.UserId);
                foreach (var it in favorite)
                {
                    if (!IsFavorite(it.PhotoId) && !IsIndesirable(it.PhotoId))
                        Favorite.Add(it.PhotoId);
                }
                return true;
            }
            return false;
        }

        public bool IsFavorite(string id)
        {
            if (Favorite.FindIndex(x => x == id) == -1)
                return false;
            return true;
        }

        public bool AddFavorite(string id)
        {
            if (!Managers.Instance.flicker.SetFavorite(id))
                return false;
            LoadFavorite();
            return true;
        }

        public bool RemoveFavorite(string id)
        {
            if (!Managers.Instance.flicker.UnsetFavorite(id))
                return false;
            LoadFavorite();
            return true;
        }

        public void LoadIndesirable()
        {
            string text = "";
            string path = "";
            switch (Managers.Instance.service)
            {
                case SERVICE.FLICKR:
                    path = "./Indesirable_Save_Flickr.db";
                    break;
                case SERVICE.IMGUR:
                    path = "./Indesirable_Save_Imgur.db";
                    break;
            }

            try
            {
                text = System.IO.File.ReadAllText(path);
                Indesirable = text.Split(' ').ToList();
            }
            catch (FileNotFoundException)
            {
                return;
            }
        }

        public void SaveIndesirable()
        {
            string text = "";
            string path = "";
            switch (Managers.Instance.service)
            {
                case SERVICE.FLICKR:
                    path = "./Indesirable_Save_Flickr.db";
                    break;
                case SERVICE.IMGUR:
                    path = "./Indesirable_Save_Imgur.db";
                    break;
            }

            foreach (var it in Indesirable)
                text += it + " ";

            File.WriteAllText(path, text);
        }

        public bool IsIndesirable(string id)
        {
            if (Indesirable.FindIndex(x => x == id) == -1)
                return false;
            return true;
        }

        public bool AddIndesirable(string photo)
        {
            if (IsIndesirable(photo))
                return false;
            Indesirable.Add(photo);
            return true;
        }

        public bool RemoveIndesirable(string photo)
        {
            if (!IsIndesirable(photo))
                return false;
            Indesirable.Remove(photo);
            return true;
        }
    }
}
