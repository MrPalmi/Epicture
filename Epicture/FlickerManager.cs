
using FlickrNet;
using System;
using System.Windows;

namespace Epicture
{
    class FlickerManager
    {
        private string Key = "5c961f2ed3bb388479332e5ad980e93e";
        private string secretKey = "386f785c06ea0d7c";

        public Flickr flickr;
        public string userID;

        private OAuthRequestToken requestToken;
        public OAuthAccessToken accessToken;

        public FlickerManager()
        {
        }

        public void Connect()
        {
            flickr = new Flickr(Key, secretKey);
        }

        public void ConnectUser(string userMail)
        {
            userID = flickr.PeopleFindByEmail(userMail).UserId;
        }

        public bool SetFavorite(string photoId)
        {
            try
            {
                flickr.FavoritesAdd(photoId);
                return true;
            }
            catch (FlickrApiException)
            {
                return false;
            }
        }

        public bool UnsetFavorite(string photoId)
        {
            try
            {
                flickr.FavoritesRemove(photoId);
                return true;
            }
            catch (FlickrException)
            {
                return false;
            }
        }

        public void AskToken()
        {
            requestToken = flickr.OAuthGetRequestToken("oob");
            string url = flickr.OAuthCalculateAuthorizationUrl(requestToken.Token, AuthLevel.Write);

            System.Diagnostics.Process.Start(url);
        }

        public bool ValidateToken(string txt)
        {
            if (String.IsNullOrEmpty(txt))
            {
                MessageBox.Show("You must paste the verifier code into the textbox above.");
                return false;
            }

            Flickr f = Managers.Instance.flicker.flickr;
            try
            {
                accessToken = f.OAuthGetAccessToken(requestToken, txt);
                f.OAuthAccessToken = accessToken.Token;
                Managers.Instance.user.UserName = accessToken.FullName;
                Managers.Instance.user.Token = accessToken.Token;
                Managers.Instance.user.Connected = true;
                MessageBox.Show("Successfully authenticated as " + accessToken.FullName);
                Managers.Instance.cache.LoadFavorite();
            }
            catch (FlickrApiException ex)
            {
                MessageBox.Show("Failed to get access token. Error message: " + ex.Message);
                return false;
            }
            return true;
        }

        public void Upload(string filename, string title, string description, bool public_)
        {
            flickr.UploadPicture(filename, title, description, null, public_, false, false);
        }

    }
}
