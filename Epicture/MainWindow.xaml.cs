using Imgur.API.Endpoints.Impl;
using Imgur.API.Models.Impl;
using System.Windows.Forms;
using System.Windows;
using FlickrNet;
using System;

namespace Epicture
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            Managers.Instance.flicker.Connect();
            Managers.Instance.imgur.Connect();
            Init();
        }

        public void SearchRecent()
        {
            SearchMode();
            Pannel.Children.Clear();

            switch (Managers.Instance.service)
            {
                case SERVICE.FLICKR:
                    PhotoCollection photos = Managers.Instance.flicker.flickr.PhotosGetRecent(Managers.Instance.nav.Page, Managers.Instance.nav.ImagePerPage, PhotoSearchExtras.Tags | PhotoSearchExtras.Description);

                    foreach (Photo photo in photos)
                    {
                        if (!Managers.Instance.user.AllowIndesirable)
                        {
                            if (!Managers.Instance.cache.IsIndesirable(photo.PhotoId))
                                LoadImage(photo.PhotoId, photo.Title, photo.Description, photo.SmallUrl, photo.MediumUrl, photo.LargeUrl);
                        }
                        else
                            LoadImage(photo.PhotoId, photo.Title, photo.Description, photo.SmallUrl, photo.MediumUrl, photo.LargeUrl);
                    }
                    break;
                case SERVICE.IMGUR:
                    var endpoint = new GalleryEndpoint(Managers.Instance.imgur.Imgur);
                    var result = endpoint.GetMemesSubGalleryAsync();
                    result.Wait();
                    var list = result.Result;

                    foreach (var it in list)
                    {
                        if (it.GetType() == typeof(GalleryImage))
                        {
                            var img = it as GalleryImage;
                            if (!Managers.Instance.user.AllowIndesirable)
                            {
                                if (!Managers.Instance.cache.IsIndesirable(img.Id))
                                    LoadImage(img.Id, img.Title, img.Description, img.Link, img.Link, img.Link);
                            }
                            else
                                LoadImage(img.Id, img.Title, img.Description, img.Link, img.Link, img.Link);
                        }
                        else if (it.GetType() == typeof(GalleryAlbum))
                        {
                            var album = it as GalleryAlbum;
                            foreach (var img in album.Images)
                            {
                                if (!Managers.Instance.user.AllowIndesirable)
                                {
                                    if (!Managers.Instance.cache.IsIndesirable(img.Id))
                                        LoadImage(img.Id, img.Title, img.Description, img.Link, img.Link, img.Link, album.Id);
                                }
                                else
                                    LoadImage(img.Id, img.Title, img.Description, img.Link, img.Link, img.Link, album.Id);
                            }
                        }
                    }
                    break;
            }

            ScrollPannel.ScrollToTop();
        }

        public void Search(string searchTerm, int numPage, int imagePerPage)
        {
            if (String.IsNullOrEmpty(searchTerm))
            {
                SearchRecent();
                return;
            }
            SearchMode();
            Pannel.Children.Clear();

            switch (Managers.Instance.service)
            {
                case SERVICE.FLICKR:
                    var options = new PhotoSearchOptions { Text = searchTerm, PerPage = imagePerPage, Page = numPage, SafeSearch = SafetyLevel.Safe, Extras = PhotoSearchExtras.Description | PhotoSearchExtras.Description | PhotoSearchExtras.Usage };

                    PhotoCollection photos = Managers.Instance.flicker.flickr.PhotosSearch(options);
                    foreach (Photo photo in photos)
                    {
                        if (!Managers.Instance.user.AllowIndesirable)
                        {
                            if (!Managers.Instance.cache.IsIndesirable(photo.PhotoId))
                                LoadImage(photo.PhotoId, photo.Title, photo.Description, photo.SmallUrl, photo.MediumUrl, photo.LargeUrl);
                        }
                        else
                            LoadImage(photo.PhotoId, photo.Title, photo.Description, photo.SmallUrl, photo.MediumUrl, photo.LargeUrl);
                    }
                    break;
                case SERVICE.IMGUR:
                    var endpoint = new GalleryEndpoint(Managers.Instance.imgur.Imgur);
                    var result = endpoint.SearchGalleryAsync(searchTerm);
                    result.Wait();
                    var list = result.Result;

                    foreach (var it in list)
                    {
                        if (it.GetType() == typeof(GalleryImage))
                        {
                            var img = it as GalleryImage;
                            if (!Managers.Instance.user.AllowIndesirable)
                            {
                                if (!Managers.Instance.cache.IsIndesirable(img.Id))
                                    LoadImage(img.Id, img.Title, img.Description, img.Link, img.Link, img.Link);
                            }
                            else
                                LoadImage(img.Id, img.Title, img.Description, img.Link, img.Link, img.Link);
                        }
                        else if (it.GetType() == typeof(GalleryAlbum))
                        {
                            var album = it as GalleryAlbum;
                            foreach (var img in album.Images)
                            {
                                if (!Managers.Instance.user.AllowIndesirable)
                                {
                                    if (!Managers.Instance.cache.IsIndesirable(img.Id))
                                        LoadImage(img.Id, img.Title, img.Description, img.Link, img.Link, img.Link, album.Id);
                                }
                                else
                                    LoadImage(img.Id, img.Title, img.Description, img.Link, img.Link, img.Link, album.Id);
                            }
                        }
                    }
                    break;
            }
            ScrollPannel.ScrollToTop();
        }

        public void LoadImage(string id, string title, string description, string smallUrl, string mediumUrl, string largeUrl, string albumId = null)
        {
            ImageInfo imgProfil = new ImageInfo(id, title, description, smallUrl, mediumUrl, largeUrl, albumId);
            Pannel.Children.Add(imgProfil);
        }
       
        private void NextPage(object sender, RoutedEventArgs e)
        {
            Managers.Instance.nav.Page += 1;
            Search(search.Text, Managers.Instance.nav.Page, Managers.Instance.nav.ImagePerPage);
            ScrollPannel.ScrollToTop();
        }

        private void PrevPage(object sender, RoutedEventArgs e)
        {
            Managers.Instance.nav.Page -= 1;
            if (Managers.Instance.nav.Page < 1)
                Managers.Instance.nav.Page = 1;
            Search(search.Text, Managers.Instance.nav.Page, Managers.Instance.nav.ImagePerPage);
            ScrollPannel.ScrollToTop();
        }

        private void Search(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key != System.Windows.Input.Key.Enter)
                return;

            e.Handled = true;
            Managers.Instance.nav.Page = 1;
            Search(search.Text, Managers.Instance.nav.Page, Managers.Instance.nav.ImagePerPage);
        }

        private void SearchFavoris(object sender, RoutedEventArgs e)
        {
            SearchMode();

            if (Managers.Instance.user.Connected)
            {
                Managers.Instance.nav.Page = 1;
                Pannel.Children.Clear();

                switch (Managers.Instance.service)
                {
                    case SERVICE.FLICKR:
                        Managers.Instance.cache.Favorite.Clear();
                        var favorite = Managers.Instance.flicker.flickr.FavoritesGetList(Managers.Instance.flicker.accessToken.UserId);
                        foreach (var it in favorite)
                        {
                            if (!Managers.Instance.cache.IsFavorite(it.PhotoId) && !Managers.Instance.cache.IsIndesirable(it.PhotoId))
                                Managers.Instance.cache.Favorite.Add(it.PhotoId);
                            if (!Managers.Instance.user.AllowIndesirable)
                            {
                                if (!Managers.Instance.cache.IsIndesirable(it.PhotoId))
                                    LoadImage(it.PhotoId, it.Title, it.Description, it.SmallUrl, it.MediumUrl, it.LargeUrl);
                            }
                            else
                                LoadImage(it.PhotoId, it.Title, it.Description, it.SmallUrl, it.MediumUrl, it.LargeUrl);
                        }
                        break;
                    case SERVICE.IMGUR:
                        var endpoint = new AccountEndpoint(Managers.Instance.imgur.Imgur);
                        var result = endpoint.GetAccountGalleryFavoritesAsync();
                        result.Wait();
                        var list = result.Result;

                        foreach (var it in list)
                        {
                            if (it.GetType() == typeof(GalleryImage))
                            {
                                var img = it as GalleryImage;
                                if (!Managers.Instance.user.AllowIndesirable)
                                {
                                    if (!Managers.Instance.cache.IsIndesirable(img.Id))
                                        LoadImage(img.Id, img.Title, img.Description, img.Link, img.Link, img.Link);
                                }
                                else
                                    LoadImage(img.Id, img.Title, img.Description, img.Link, img.Link, img.Link);
                            }
                            else if (it.GetType() == typeof(GalleryAlbum))
                            {
                                var album = it as GalleryAlbum;
                                foreach (var img in album.Images)
                                {
                                    if (!Managers.Instance.user.AllowIndesirable)
                                    {
                                        if (!Managers.Instance.cache.IsIndesirable(img.Id))
                                            LoadImage(img.Id, img.Title, img.Description, img.Link, img.Link, img.Link, album.Id);
                                    }
                                    else
                                        LoadImage(img.Id, img.Title, img.Description, img.Link, img.Link, img.Link, album.Id);
                                }
                            }
                        }
                        break;
                }
            }
        }

        private void AskToken(object sender, RoutedEventArgs e)
        {
            switch (Managers.Instance.service)
            {
                case SERVICE.FLICKR:
                    Managers.Instance.flicker.AskToken();
                    break;
                case SERVICE.IMGUR:
                    Managers.Instance.imgur.AskToken();
                    break;
            }

            ValidationToken.Visibility = Visibility.Visible;
            ValidateTokenButton.Visibility = Visibility.Visible;
            AskTokenButton.Visibility = Visibility.Collapsed;
        }

        private void ValidateToken(object sender, RoutedEventArgs e)
        {
            bool ok = false;

            switch (Managers.Instance.service)
            {
                case SERVICE.FLICKR:
                    if (Managers.Instance.flicker.ValidateToken(ValidationToken.Text))
                        ok = true;
                    break;
                case SERVICE.IMGUR:
                    if (Managers.Instance.imgur.ValidateToken(ValidationToken.Text))
                        ok = true;
                    break;
            }

            if (ok)
            {
                ActionPannel.Height = new GridLength();
                ConnectionPannel.Height = new GridLength(0);
            }

            UserInfo.Text = Managers.Instance.user.UserName;
        }

        private void DisplayIndesirable(object sender, RoutedEventArgs e)
        {
            Managers.Instance.user.AllowIndesirable = !Managers.Instance.user.AllowIndesirable;
            Indesirable.Content = "Display indesirables in safe mode";
            if (Managers.Instance.user.AllowIndesirable)
                Indesirable.Content = "Hide indesirables";
        }

        private void DisplayUploaded(object sender, RoutedEventArgs e)
        {
            SearchMode();

            Pannel.Children.Clear();

            if (Managers.Instance.user.Connected)
            {
                Managers.Instance.nav.Page = 1;
                Pannel.Children.Clear();

                switch (Managers.Instance.service)
                {
                    case SERVICE.FLICKR:
                        var options = new PartialSearchOptions { Extras = PhotoSearchExtras.Description | PhotoSearchExtras.Usage, PerPage = Managers.Instance.nav.ImagePerPage, Page = Managers.Instance.nav.Page };

                        PhotoCollection photos = Managers.Instance.flicker.flickr.PhotosGetNotInSet(options);
                        foreach (Photo photo in photos)
                            LoadImage(photo.PhotoId, photo.Title, photo.Description, photo.SmallUrl, photo.MediumUrl, photo.LargeUrl);
                        break;
                    case SERVICE.IMGUR:
                        var endpoint = new AccountEndpoint(Managers.Instance.imgur.Imgur);
                        var result = endpoint.GetAccountSubmissionsAsync();
                        result.Wait();
                        var list = result.Result;

                        foreach (var it in list)
                        {
                            if (it.GetType() == typeof(GalleryImage))
                            {
                                var img = it as GalleryImage;
                                if (!Managers.Instance.user.AllowIndesirable)
                                {
                                    if (!Managers.Instance.cache.IsIndesirable(img.Id))
                                        LoadImage(img.Id, img.Title, img.Description, img.Link, img.Link, img.Link);
                                }
                                else
                                    LoadImage(img.Id, img.Title, img.Description, img.Link, img.Link, img.Link);
                            }
                            else if (it.GetType() == typeof(GalleryAlbum))
                            {
                                var album = it as GalleryAlbum;
                                foreach (var img in album.Images)
                                {
                                    if (!Managers.Instance.user.AllowIndesirable)
                                    {
                                        if (!Managers.Instance.cache.IsIndesirable(img.Id))
                                            LoadImage(img.Id, img.Title, img.Description, img.Link, img.Link, img.Link);
                                    }
                                    else
                                        LoadImage(img.Id, img.Title, img.Description, img.Link, img.Link, img.Link);
                                }
                            }
                        }
                        break;
                }
                
                ScrollPannel.ScrollToTop();
            }
        }

        private void UploadImage(object sender, RoutedEventArgs e)
        {
            try
            {
                switch (Managers.Instance.service)
                {
                    case SERVICE.FLICKR:
                        Managers.Instance.flicker.flickr.OnUploadProgress += new EventHandler<FlickrNet.UploadProgressEventArgs>(OnUploadProgress);
                        Managers.Instance.flicker.Upload(Filename.Text, Title.Text, Description.Text, CheckPublic.IsChecked.Value);
                        break;
                    case SERVICE.IMGUR:
                        break;
                }
                
                System.Windows.MessageBox.Show("File " + Title.Text + " has been successfully uploaded.");
                Filename.Text = "";
                Title.Text = "";
                Description.Text = "";
                CheckPublic.IsChecked = true;
                UploadProgress.Value = 0;
            }
            catch (ArgumentException ex)
            {
                System.Windows.MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void OnUploadProgress(object sender, FlickrNet.UploadProgressEventArgs e)
        {
            UploadProgress.Value = e.ProcessPercentage;
        }

        private void SearchMode()
        {
            ScrollPannel.Visibility = Visibility.Visible;
            Pannel.Visibility = Visibility.Visible;
            Navigation.Visibility = Visibility.Visible;

            UploadForm.Visibility = Visibility.Collapsed;
        }

        private void UploadMode(object sender, RoutedEventArgs e)
        {
            Pannel.Visibility = Visibility.Collapsed;
            ScrollPannel.Visibility = Visibility.Collapsed;
            Navigation.Visibility = Visibility.Collapsed;

            UploadForm.Visibility = Visibility.Visible;
            Filename.Text = "";
            Title.Text = "";
            Description.Text = "";
            CheckPublic.IsChecked = true;
            UploadProgress.Value = 0;
        }

        private void Explore(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog();
            var result = fileDialog.ShowDialog();
            switch (result)
            {
                case System.Windows.Forms.DialogResult.OK:
                    var file = fileDialog.FileName;
                    Filename.Text = file;
                    Filename.ToolTip = file;
                    break;
                case System.Windows.Forms.DialogResult.Cancel:
                default:
                    Filename.Text = null;
                    Filename.ToolTip = null;
                    break;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Managers.Instance.cache.SaveIndesirable();
        }

        private void Init()
        {
            Managers.Instance.cache.LoadFavorite();
            Managers.Instance.cache.LoadIndesirable();

            if (Managers.Instance.user.Connected)
            {
                ActionPannel.Height = new GridLength();
                ConnectionPannel.Height = new GridLength(0);
                AskTokenButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                ActionPannel.Height = new GridLength(0);
                ConnectionPannel.Height = new GridLength(150);
                AskTokenButton.Visibility = Visibility.Visible;
            }

            ValidationToken.Visibility = Visibility.Collapsed;
            ValidateTokenButton.Visibility = Visibility.Collapsed;

            SearchRecent();
        }

        private void SwitchService(object sender, RoutedEventArgs e)
        {
            Managers.Instance.cache.SaveIndesirable();

            switch (Managers.Instance.service)
            {
                case SERVICE.FLICKR:
                    Managers.Instance.service = SERVICE.IMGUR;
                    Init();
                    SwitchButton.Content = "Use Flicker Api";
                    break;
                case SERVICE.IMGUR:
                    Managers.Instance.service = SERVICE.FLICKR;
                    Init();
                    SwitchButton.Content = "Use Imgur Api";
                    break;
            }
        }
    }
}
