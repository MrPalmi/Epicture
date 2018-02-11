using System.Collections.Generic;
using Imgur.API.Endpoints.Impl;
using System.Linq;
using System.IO;
using Imgur.API.Models.Impl;

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
            switch (Managers.Instance.service)
            {
                case SERVICE.FLICKR:
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
                    break;
                case SERVICE.IMGUR:
                    if (Managers.Instance.user.Connected)
                    {
                        var endpoint = new AccountEndpoint(Managers.Instance.imgur.Imgur);
                        var result = endpoint.GetAccountGalleryFavoritesAsync();
                        result.Wait();
                        var list = result.Result;

                        foreach (var it in list)
                        {
                            if (it.GetType() == typeof(GalleryImage))
                            {
                                var img = it as GalleryImage;
                                Favorite.Add(img.Id);
                            }
                            else if (it.GetType() == typeof(GalleryAlbum))
                            {
                                var album = it as GalleryAlbum;
                                foreach (var img in album.Images)
                                    Favorite.Add(img.Id);
                            }
                        }
                        return true;
                    }
                    break;
            }
            return false;
        }

        public bool IsFavorite(string id)
        {
            if (Favorite.FindIndex(x => x == id) == -1)
                return false;
            return true;
        }

        public bool AddFavorite(string id, string album = null)
        {
            switch (Managers.Instance.service)
            {
                case SERVICE.FLICKR:
                    if (!Managers.Instance.flicker.SetFavorite(id))
                        return false;
                    break;
                case SERVICE.IMGUR:
                    if (album != null)
                    {
                        if (!Managers.Instance.imgur.SetFavoriteAlbum(album))
                            return false;
                        return true;
                    }
                    if (!Managers.Instance.imgur.SetFavorite(id))
                        return false;
                    break;
            }
            LoadFavorite();
            return true;
        }

        public bool RemoveFavorite(string id, string album = null)
        {
            switch (Managers.Instance.service)
            {
                case SERVICE.FLICKR:
                    if (!Managers.Instance.flicker.UnsetFavorite(id))
                        return false;
                    LoadFavorite();
                    break;
                case SERVICE.IMGUR:
                    if (!Managers.Instance.imgur.UnsetFavorite(id))
                        return false;
                    LoadFavorite();
                    break;
            }
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
