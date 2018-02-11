
namespace Epicture
{
    class NavigationManager
    {
        public int Page { get; set; }
        public int ImagePerPage {get; set;}

        public NavigationManager()
        {
            Page = 1;
            ImagePerPage = 40;
        }
    }
}
