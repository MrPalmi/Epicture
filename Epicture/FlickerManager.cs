
using FlickrNet;

namespace Epicture
{
    class FlickerManager
    {
        private string Key = "5c961f2ed3bb388479332e5ad980e93e";
        private string secretKey = "386f785c06ea0d7c";

        public Flickr flickr;
        public string userID;

        public int imagePerPage;
        public int page;

        public FlickerManager()
        {
        }

        public void Connect()
        {
            flickr = new Flickr(Key, secretKey);
            imagePerPage = 40;
            page = 1;
        }

        public void ConnectUser(string userMail)
        {
            userID = flickr.PeopleFindByEmail(userMail).UserId;
        }

        public void SetFavorite()
        {
            flickr.FavoritesAdd("");
        }

        public void UnsetFavorite()
        {
        }

    }
}
