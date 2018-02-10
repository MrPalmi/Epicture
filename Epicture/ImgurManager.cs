using Imgur.API;
using Imgur.API.Authentication.Impl;
using Imgur.API.Endpoints.Impl;
using Imgur.API.Enums;
using Imgur.API.Models;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace Epicture
{
    class ImgurManager
    {
        private string id = "3229ec5962e2e0f";
        private string secretId = "cacd24a3b1dc136dd1d90ced9a87499a4b2ee4b9";
        ImgurClient Imgur;
        IOAuth2Token accessToken;

        public bool Connect()
        {
            try
            {
                Imgur = new ImgurClient(id, secretId);
                return true;
            }
            catch (ImgurException)
            {
                return false;
            }
        }

        public void AskToken()
        {
            var endpoint = new OAuth2Endpoint(Imgur);
            var authorizationUrl = endpoint.GetAuthorizationUrl(OAuth2ResponseType.Pin);

            System.Diagnostics.Process.Start(authorizationUrl);
        }

        public bool ValidateToken(string code)
        {
            var endpoint = new OAuth2Endpoint(Imgur);
            var token = endpoint.GetTokenByPinAsync(code);
            token.Wait();
            accessToken = token.Result;
            Managers.Instance.user.UserName = accessToken.AccountUsername;
            Managers.Instance.user.Token = accessToken.AccessToken;
            Managers.Instance.user.Connected = true;
            MessageBox.Show("Successfully authenticated as " + accessToken.AccountUsername);
            return true;
        }

        public async Task GetImage()
        {
            try
            {
                var endpoint = new ImageEndpoint(Imgur);
                var image = await endpoint.GetImageAsync("IMAGE_ID");
                Console.WriteLine("Image retrieved. Image Url: " + image.Link);
            }
            catch (ImgurException imgurEx)
            {
                Console.WriteLine("An error occurred getting an image from Imgur.");
                Console.WriteLine(imgurEx.Message);
            }
        }

        public async Task UploadImage(string url)
        {
            try
            {
                var endpoint = new ImageEndpoint(Imgur);
                IImage image;
                using (var fs = new FileStream(url, FileMode.Open))
                {
                    image = await endpoint.UploadImageStreamAsync(fs);
                }
                Console.WriteLine("Image uploaded. Image Url: " + image.Link);
            }
            catch (ImgurException imgurEx)
            {
                Console.WriteLine("An error occurred uploading an image to Imgur.");
                Console.WriteLine(imgurEx.Message);
            }
        }
    }
}
