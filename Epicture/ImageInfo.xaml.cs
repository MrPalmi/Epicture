using FlickrNet;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
	}
}
