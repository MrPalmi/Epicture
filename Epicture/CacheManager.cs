using System.Collections.Generic;
using FlickrNet;
using System.IO;
using System.Linq;
using System;

namespace Epicture
{
    class CacheManager
    {
        private List<string> OldFavorite;
        private List<string> Indesirable;

        public List<Photo> Favorite;

        public CacheManager()
        {
            Indesirable = new List<string>();
            OldFavorite = new List<string>();

            Favorite = new List<Photo>();
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
                    Console.WriteLine(Managers.Instance.flicker.SetFavorite(it.PhotoId));
                foreach (var it in OldFavorite)
                    Managers.Instance.flicker.UnsetFavorite(it);
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
            if (OldFavorite.FindIndex(x => x == photo.PhotoId) == 1)
                OldFavorite.Remove(photo.PhotoId);
            Favorite.Add(photo);
            return true;
        }

        public bool RemoveFavorite(Photo photo)
        {
            if (!IsFavorite(photo.PhotoId))
                return false;
            if (OldFavorite.FindIndex(x => x == photo.PhotoId) == -1)
                OldFavorite.Add(photo.PhotoId);
            Favorite.Remove(photo);
            return true;
        }

        public void LoadIndesirable()
        {
            string text = "";

            try
            {
                text = System.IO.File.ReadAllText("./Indesirable_Save.db");
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

            foreach (var it in Indesirable)
                text += it + " ";

            File.WriteAllText("./Indesirable_Save.db", text);
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
