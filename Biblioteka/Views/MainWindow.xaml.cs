using System.Windows;
using System.Windows.Controls;


namespace Biblioteka
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ViewModels.MainWindowViewModel();

            borrow.IsEnabled = false;

        }
        private void ChangeClientSelection(object sender, RoutedEventArgs e)
        {
            var curItem = ((ListViewItem)ClientList.ContainerFromElement((Button)sender));
            ClientList.SelectedItem = null;
            ClientList.SelectedItem = (ListViewItem)curItem;
            curItem.IsSelected = true;
        }
        private void ChangeBorrowSelection(object sender, RoutedEventArgs e)
        {
            var curItem = ((ListViewItem)BorrowList.ContainerFromElement((Button)sender));
            BorrowList.SelectedItem = null;
            BorrowList.SelectedItem = (ListViewItem)curItem;
            curItem.IsSelected = true;
        }

        private void ChangeBooksSelection(object sender, RoutedEventArgs e)
        {
            var curItem = ((ListViewItem)BooksList.ContainerFromElement((Button)sender));
            BooksList.SelectedItem = null;
            BooksList.SelectedItem = (ListViewItem)curItem;
            curItem.IsSelected = true;
        }

        













        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ClientList.SelectedItem != null)
            {
                borrow.IsEnabled = true;
                var list = e.AddedItems;
                var selectedClient = (Biblioteka.Models.Klient)list[0];
                ShowSelectedClient.Text = selectedClient.name + " " + selectedClient.surname;
            }
        }

        private void SearchClient_Click(object sender, RoutedEventArgs e)
        {
            borrow.IsEnabled = false;

        }
    }
}
