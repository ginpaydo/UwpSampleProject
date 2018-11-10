using System;

using App2.ViewModels;

using Windows.UI.Xaml.Controls;

namespace App2.Views
{
    public sealed partial class TabbedPage : Page
    {
        private TabbedViewModel ViewModel => DataContext as TabbedViewModel;

        public TabbedPage()
        {
            InitializeComponent();
        }
    }
}
