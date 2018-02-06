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
            UserInfo.Text = Managers.Instance.user.UserName;
        }

        public void Search(string searchTerm, int numPage, int imagePerPage)
        {
            if (String.IsNullOrEmpty(searchTerm))
                return;
            var options = new PhotoSearchOptions { Tags = searchTerm, PerPage = imagePerPage, Page = numPage };
            PhotoCollection photos = Managers.Instance.flicker.flickr.PhotosSearch(options);

            Pannel.Children.Clear();

            foreach (Photo photo in photos)
            {
                Console.WriteLine("Photo {0} has title '{1}' and is at {2}", photo.PhotoId, photo.Title, photo.LargeUrl);
                LoadImage(photo);
            }
        }

        public void LoadImage(Photo photo)
        {
            ImageInfo imgProfil = new ImageInfo(photo);
            Pannel.Children.Add(imgProfil);
        }

        private void NextPage(object sender, RoutedEventArgs e)
        {
            Managers.Instance.flicker.page += 1;
            Search(search.Text, Managers.Instance.flicker.page, 30);
        }

        private void PrevPage(object sender, RoutedEventArgs e)
        {
            Managers.Instance.flicker.page -= 1;
            if (Managers.Instance.flicker.page < 1)
                Managers.Instance.flicker.page = 1;
            Search(search.Text, Managers.Instance.flicker.page, 30);
        }

        private void Search(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key != System.Windows.Input.Key.Enter)
                return;

            e.Handled = true;
            Managers.Instance.flicker.page = 1;
            Search(search.Text, Managers.Instance.flicker.page, Managers.Instance.flicker.imagePerPage);
        }

        private void SearchFavoris(object sender, RoutedEventArgs e)
        {
            if (Managers.Instance.flicker.accessToken != null && Managers.Instance.flicker.accessToken.UserId != null)
            {
                Managers.Instance.flicker.page = 1;
                PhotoCollection favoris = Managers.Instance.flicker.flickr.FavoritesGetPublicList(Managers.Instance.flicker.accessToken.UserId, DateTime.MinValue, DateTime.MaxValue, PhotoSearchExtras.All, Managers.Instance.flicker.page, Managers.Instance.flicker.imagePerPage);
                Pannel.Children.Clear();

                foreach (Photo photo in favoris)
                {
                    Console.WriteLine("Photo {0} has title '{1}' and is at {2}", photo.PhotoId, photo.Title, photo.LargeUrl);
                    LoadImage(photo);
                }
            }
        }

        private void AskToken(object sender, RoutedEventArgs e)
        {
            Managers.Instance.flicker.AskToken();
            ValidationToken.Visibility = Visibility.Visible;
            ValidateTokenButton.Visibility = Visibility.Visible;
            AskTokenButton.Visibility = Visibility.Collapsed;
        }

        private void ValidateToken(object sender, RoutedEventArgs e)
        {
            if (Managers.Instance.flicker.ValidateToken(ValidationToken.Text))
                FavorisSearch.Visibility = Visibility.Visible;
            else
                ValidationToken.Visibility = Visibility.Visible;

            ValidationToken.Visibility = Visibility.Collapsed;
            ValidateTokenButton.Visibility = Visibility.Collapsed;

            UserInfo.Text = Managers.Instance.user.UserName;
        }
    }
}
