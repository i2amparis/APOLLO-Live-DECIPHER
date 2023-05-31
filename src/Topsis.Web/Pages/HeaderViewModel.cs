namespace Topsis.Web.Pages
{
    public struct HeaderViewModel
    {
        public HeaderViewModel(bool isModeratorLayout = false, 
            bool isGuestLayout = false,
            bool isDefaultLayout = false)
        {
            IsModeratorLayout = isModeratorLayout;
            IsGuestLayout = isGuestLayout;
            IsDefaultLayout = isDefaultLayout;
        }

        public bool IsModeratorLayout { get; set; }
        public bool IsGuestLayout { get; }
        public bool IsDefaultLayout { get; }
    }
}
