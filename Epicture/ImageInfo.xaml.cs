using FlickrNet;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
