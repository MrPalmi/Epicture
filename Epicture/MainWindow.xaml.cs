using System.Windows;
using FlickrNet;
using System;
using System.Windows.Forms;

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
            Managers.Instance.cache.LoadFavorite();
            Managers.Instance.cache.LoadIndesirable();
            SearchRecent();
        }

        public void SearchRecent()
        {
            SearchMode();
            Pannel.Children.Clear();
            PhotoCollection photos = Managers.Instance.flicker.flickr.PhotosGetRecent(Managers.Instance.flicker.page, Managers.Instance.flicker.imagePerPage, PhotoSearchExtras.Tags | PhotoSearchExtras.Description);

            foreach (Photo photo in photos)
                LoadImage(photo);
        }

        public void Search(string searchTerm, int numPage, int imagePerPage)
        {
            if (String.IsNullOrEmpty(searchTerm))
            {
                SearchRecent();
                return;
            }
            SearchMode();

            var options = new PhotoSearchOptions { Text = searchTerm, PerPage = imagePerPage, Page = numPage, SafeSearch = SafetyLevel.Restricted, Extras = PhotoSearchExtras.Description | PhotoSearchExtras.Description | PhotoSearchExtras.Usage };

            PhotoCollection photos = Managers.Instance.flicker.flickr.PhotosSearch(options);
            Pannel.Children.Clear();
            foreach (Photo photo in photos)
                LoadImage(photo);
        }

        public void LoadImage(Photo photo)
        {
            ImageInfo imgProfil = new ImageInfo(photo);
            Pannel.Children.Add(imgProfil);
        }

        private void NextPage(object sender, RoutedEventArgs e)
        {
            Managers.Instance.flicker.page += 1;
            Search(search.Text, Managers.Instance.flicker.page, Managers.Instance.flicker.imagePerPage);
			ScrollPannel.ScrollToTop();
	    }

        private void PrevPage(object sender, RoutedEventArgs e)
        {
            Managers.Instance.flicker.page -= 1;
            if (Managers.Instance.flicker.page < 1)
                Managers.Instance.flicker.page = 1;
            Search(search.Text, Managers.Instance.flicker.page, Managers.Instance.flicker.imagePerPage);
			ScrollPannel.ScrollToTop();
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
            SearchMode();
            if (Managers.Instance.flicker.accessToken != null && Managers.Instance.flicker.accessToken.UserId != null)
            {
                Managers.Instance.flicker.page = 1;
                Pannel.Children.Clear();
                foreach (Photo photo in Managers.Instance.cache.Favorite)
                    LoadImage(photo);
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
            {
                FavorisSearch.Visibility = Visibility.Visible;
                Upload.Visibility = Visibility.Visible;
                Indesirable.Visibility = Visibility.Visible;
            }
            else
                ValidationToken.Visibility = Visibility.Visible;

            ValidationToken.Visibility = Visibility.Collapsed;
            ValidateTokenButton.Visibility = Visibility.Collapsed;

            UserInfo.Text = Managers.Instance.user.UserName;
        }

        private void SearchIndesirable(object sender, RoutedEventArgs e)
        {
            SearchMode();
        }

        private void UploadImage(object sender, RoutedEventArgs e)
        {
            try
            {
                Managers.Instance.flicker.flickr.OnUploadProgress += new EventHandler<FlickrNet.UploadProgressEventArgs>(OnUploadProgress);
                Managers.Instance.flicker.Upload(Filename.Text, Title.Text, Description.Text, CheckPublic.IsChecked.Value);
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
    }
}
