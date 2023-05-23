using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Biblioteka.Views
{
    /// <summary>
    /// Interaction logic for EditGenres.xaml
    /// </summary>
    public partial class EditGenres : Window
    {
        private int selectedIndex;
        public EditGenres()
        {
            InitializeComponent();
           // selectedIndex = GenresComboBox.SelectedIndex;

        }

        private void GenresComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TextEditor.Text = (string)GenresComboBox.SelectedItem;
            Console.WriteLine(GenresComboBox.SelectedIndex);
           // selectedIndex = GenresComboBox.SelectedIndex;
        }

        private void Set_Index(object sender, RoutedEventArgs e)
        {
           // GenresComboBox.SelectedItem = selectedIndex;

        }

        //private void ChangeGenreSelection(object sender, RoutedEventArgs e)
        //{
        //    var curItem = ((ListViewItem)GenresList.ContainerFromElement((Button)sender));
        //    GenresList.SelectedItem = null;
        //    GenresList.SelectedItem = (ListViewItem)curItem;
        //    curItem.IsSelected = true;
        //}

    }
}
