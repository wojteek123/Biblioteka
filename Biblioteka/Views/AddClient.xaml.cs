using Biblioteka.ViewModels;

using System.Text.RegularExpressions;

using System.Windows.Controls;

using System.Windows.Input;


namespace Biblioteka.Views
{
    /// <summary>
    /// Interaction logic for AddClient.xaml
    /// </summary>
    public partial class AddClient : UserControl
    {
        Regex regex = new Regex(@"\d");

        public AddClient()
        {
            InitializeComponent();
            DataContext = new AddClientViewModel();

        }

        private void PhoneNumberPreviewChecker(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"\d");

            if (regex.IsMatch(e.Text) && phoneNumnber.Text.Length < 9)
                e.Handled = false;
            else
                e.Handled = true;

        }
        private void PeselPreviewChecker(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"\d");
            if (regex.IsMatch(e.Text) && pesel.Text.Length < 11)
                e.Handled = false;
            else
                e.Handled = true;

        }




    }
}
