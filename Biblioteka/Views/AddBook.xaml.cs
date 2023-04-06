using Biblioteka.ViewModels;
using System;
using System.Globalization;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

using System.Text.RegularExpressions;

namespace Biblioteka.Views
{
    /// <summary>
    /// Interaction logic for AddBook.xaml
    /// </summary>
    public partial class AddBook : UserControl
    {
        public AddBook()
        {
            InitializeComponent();
            //DataContext = new AddBookViewModel();

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

    [ValueConversion(typeof(DateTime), typeof(String))]
    public class ShortDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime date = (DateTime)value;
            return date.ToShortDateString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string strValue = value as string;
            DateTime resultDateTime;
            if (DateTime.TryParse(strValue, out resultDateTime))
            {
                return resultDateTime;
            }
            return DependencyProperty.UnsetValue;
        }
    }


}
