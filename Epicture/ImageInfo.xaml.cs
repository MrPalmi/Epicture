using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Media;
using System.Drawing;
using System.Windows;
using System.Net;
using FlickrNet;
using System;

namespace Epicture
{
    /// <summary>
    /// Interaction logic for ImageInfo.xaml
    /// </summary>
    public partial class ImageInfo : System.Windows.Controls.UserControl
	{
        public Photo photo;

		public ImageInfo(Photo photo_)
        {
            InitializeComponent();
            photo = photo_;
            LoadImage();
		}

		private void LoadImage()
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(photo.Small320Url, UriKind.Absolute);
            bitmap.EndInit();

            ImageSource imageSource = bitmap;
            Image.Source = imageSource;
            Title.Text = photo.Title;
			Description.Text = photo.Description;

            if (Managers.Instance.user.Connected)
            {
                var photos = Managers.Instance.flicker.flickr.FavoritesGetList(Managers.Instance.flicker.accessToken.UserId);

                foreach (var it in photos)
                {
                    if (it.PhotoId == photo.PhotoId)
                    {
                        StarsIcon.Foreground = new SolidColorBrush(Colors.Gray);
                        return;
                    }
                }
            }
            else
                Stars.Visibility = Visibility.Collapsed;
        }

        private void Favorite(object sender, RoutedEventArgs e)
        {
            var photos = Managers.Instance.flicker.flickr.FavoritesGetList(Managers.Instance.flicker.accessToken.UserId);

            foreach (var it in photos)
            {
                if (it.PhotoId == photo.PhotoId)
                {
                    Managers.Instance.flicker.UnsetFavorite(it.PhotoId);
                    return;
                }
            }
            Managers.Instance.flicker.SetFavorite(photo.PhotoId);
        }

		private void DownloadImage(object sender, RoutedEventArgs e)
		{
			try
			{
				MessageBox.Show(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\n\n" + photo.LargeUrl + "\n\n" + photo.Title);
				using (WebClient client = new WebClient())
					client.DownloadFile(new Uri(photo.LargeUrl), Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\" +  photo.Title + ".jpg");
			}
			catch (ExternalException)
			{
				//Something is wrong with Format -- Maybe required Format is not 
				// applicable here
				MessageBox.Show("Can't download the picture");
			}
			MessageBox.Show("Added photo to Pictures user folder");
		}

		private void Grid_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
		{
			Description.Visibility = Visibility.Visible;
			Description.Foreground = new SolidColorBrush(Colors.White);
			Description.Background.Opacity = 0.7;
		}
		private void Grid_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
		{
			Description.Visibility = Visibility.Hidden;
		}

		private void ImageAwesome_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
		{
			download.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 84, 145, 242));
			Download.Background = new SolidColorBrush(Colors.White);
		}

		private void ImageAwesome_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
		{
			download.Foreground = new SolidColorBrush(Colors.LightGray);
		}
	}
}
