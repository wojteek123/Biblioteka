using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for EditBook.xaml
    /// </summary>
    public partial class EditBook : Window
    {
        public EditBook()
        {
            InitializeComponent();
        }

        private void OnlyNumeric(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"\d");
            if (regex.IsMatch(e.Text) && egz.Text.Length < 2)
                e.Handled = false;
            else
                e.Handled = true;

        }


    }


  


}
