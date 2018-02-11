using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using Imgur.API.Models.Impl;
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
	public partial class ImageInfo : UserControl
	{
        string id;
        string title;
        string smallUrl;
        string mediumUrl;
        string largeUrl;
        string description;

        public ImageInfo(Photo photo)
        {
            InitializeComponent();
            id = photo.PhotoId;
            title = photo.Title;
            smallUrl = photo.SmallUrl;
            mediumUrl = photo.MediumUrl;
            largeUrl = photo.LargeUrl;
            description = photo.Description;
			LoadImage();
		}

        public ImageInfo(GalleryImage photo)
        {
            InitializeComponent();
            id = photo.Id;
            title = photo.Title;
            smallUrl = photo.Link;
            mediumUrl = photo.Link;
            largeUrl = photo.Link;
            description = photo.Description;
            LoadImage();
        }

        private void LoadImage()
		{
            BitmapImage bitmap = new BitmapImage();
			bitmap.BeginInit();
			bitmap.UriSource = new Uri(smallUrl, UriKind.Absolute);
			bitmap.EndInit();

			ImageSource imageSource = bitmap;
			Image.Source = imageSource;
			Title.Text = title;
			Description.Text = description;

            if (Managers.Instance.user.Connected)
			{
                if (Managers.Instance.cache.IsFavorite(id))
                    StarsIcon.Foreground = new SolidColorBrush(Colors.Gray);
			}
            else
    			Stars.Visibility = Visibility.Collapsed;
            if (Managers.Instance.cache.IsIndesirable(id))
            {
                if (Managers.Instance.user.AllowIndesirable)
                {
                    Background = new SolidColorBrush(Colors.IndianRed);
                    IndesirableIcon.Foreground = new SolidColorBrush(Colors.Gray);
                    Image.Visibility = Visibility.Collapsed;
                    Stars.Visibility = Visibility.Collapsed;
                }
                else
                    Visibility = Visibility.Hidden;
            }
        }

		private void FavoriteSetter(object sender, RoutedEventArgs e)
		{
            if (Managers.Instance.cache.IsFavorite(id))
            {
                Managers.Instance.cache.RemoveFavorite(id);
                StarsIcon.Foreground = new SolidColorBrush(Colors.LightGray);
                return;
            }
            Managers.Instance.cache.AddFavorite(id);
            StarsIcon.Foreground = new SolidColorBrush(Colors.Gray);
        }

        private void IndesirableSetter(object sender, RoutedEventArgs e)
        {
            if (Managers.Instance.cache.IsIndesirable(id))
            {
                Managers.Instance.cache.RemoveIndesirable(id);
                IndesirableIcon.Foreground = new SolidColorBrush(Colors.Gray);
                Background = new SolidColorBrush(Colors.White);
                Image.Visibility = Visibility.Visible;
                if (Managers.Instance.user.Connected)
                    Stars.Visibility = Visibility.Visible;
                return;
            }
            Managers.Instance.cache.AddIndesirable(id);
            if (Managers.Instance.user.AllowIndesirable)
            {
                Background = new SolidColorBrush(Colors.IndianRed);
                Image.Visibility = Visibility.Collapsed;
                IndesirableIcon.Foreground = new SolidColorBrush(Colors.LightGray);
                Stars.Visibility = Visibility.Collapsed;
            }
            else
                Visibility = Visibility.Hidden;
        }

        private void DownloadFile(string url, string id)
        {
            using (WebClient client = new WebClient())
                client.DownloadFile(new Uri(url), Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\" + id + ".jpg");
        }

        private void DownloadImage(object sender, RoutedEventArgs e)
		{
			try
			{
				MessageBoxResult done = MessageBox.Show("Download the picture?", "Download", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
				if (done == MessageBoxResult.Yes)
				{
                    if (largeUrl != null)
                        DownloadFile(largeUrl, id);
                    else if (mediumUrl != null)
                        DownloadFile(mediumUrl, id);
                    else
                        DownloadFile(smallUrl, id);
					MessageBox.Show("Added photo to Pictures user folder");
                }
            }
			catch (ExternalException)
			{
				MessageBox.Show("Can't download the picture");
			}
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
		}

		private void ImageAwesome_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
		{
			download.Foreground = new SolidColorBrush(Colors.LightGray);
		}

		private void Indesirable_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
		{
			IndesirableIcon.Foreground = new SolidColorBrush(Colors.Red);
		}

		private void Indesirable_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
		{
            IndesirableIcon.Foreground = new SolidColorBrush(Colors.LightGray);
            if (Managers.Instance.cache.IsIndesirable(id))
    			IndesirableIcon.Foreground = new SolidColorBrush(Colors.Gray);
        }

        private void Stars_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
		{
			StarsIcon.Foreground = new SolidColorBrush(Colors.Gold);
		}

		private void Stars_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
		{
            StarsIcon.Foreground = new SolidColorBrush(Colors.LightGray);
            if (Managers.Instance.cache.IsFavorite(id))
    			StarsIcon.Foreground = new SolidColorBrush(Colors.Gray);
        }
    }
}
