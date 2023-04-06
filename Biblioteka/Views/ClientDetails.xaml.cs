
using System.Windows;
using System.Windows.Controls;

namespace Biblioteka.Views
{
    /// <summary>
    /// Interaction logic for ClientDetails.xaml
    /// </summary>
    public partial class ClientDetails : Window
    {
        public ClientDetails()
        {
            InitializeComponent();
            DataContext = new ViewModels.ClientDetailsViewModel();





        }


        private void ChangeBorrowSelection(object sender, RoutedEventArgs e)
        {
            var curItem = ((ListViewItem)BorrowList.ContainerFromElement((Button)sender));
            BorrowList.SelectedItem = null;
            BorrowList.SelectedItem = (ListViewItem)curItem;
            curItem.IsSelected = true;
        }









    }
}
