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
	public partial class ImageInfo : UserControl
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
                if (Managers.Instance.cache.IsFavorite(photo.PhotoId))
                    StarsIcon.Foreground = new SolidColorBrush(Colors.Gray);
                return;
			}
			Stars.Visibility = Visibility.Collapsed;
		}

		private void Favorite(object sender, RoutedEventArgs e)
		{
            if (Managers.Instance.cache.IsFavorite(photo.PhotoId))
            {
			    Managers.Instance.cache.RemoveFavorite(photo);
                StarsIcon.Foreground = new SolidColorBrush(Colors.LightGray);
                return;
            }
            Managers.Instance.cache.AddFavorite(photo);
            StarsIcon.Foreground = new SolidColorBrush(Colors.Gray);
        }

        private void DownloadImage(object sender, RoutedEventArgs e)
		{
			try
			{
				if (photo.CanDownload != false)
				{
					MessageBoxResult done = MessageBox.Show("Download the picture?", "Download", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
					if (done == MessageBoxResult.Yes)
					{
						if (photo.LargeUrl != null)
						{
							using (WebClient client = new WebClient())
								client.DownloadFile(new Uri(photo.LargeUrl), Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\" + photo.PhotoId + ".jpg");
						}
						else if (photo.MediumUrl != null)
						{
							using (WebClient client = new WebClient())
								client.DownloadFile(new Uri(photo.MediumUrl), Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\" + photo.Title + ".jpg");
						}
						else
						{
							using (WebClient client = new WebClient())
								client.DownloadFile(new Uri(photo.SmallUrl), Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\" + photo.Title + ".jpg");
						}
						MessageBox.Show("Added photo to Pictures user folder");
					}
				}
			}
			catch (ExternalException)
			{
				//Something is wrong with Format -- Maybe required Format is not 
				// applicable here
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
			Download.Background = new SolidColorBrush(Colors.White);
		}

		private void ImageAwesome_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
		{
			download.Foreground = new SolidColorBrush(Colors.LightGray);
		}
	}
}
