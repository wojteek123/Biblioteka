using Biblioteka.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Biblioteka.ViewModels
{
    public class AddClientViewModel : ObservableObject
    {
        public ICommand AddClientCommand { get; }

        private Klient _klient;
        public Klient klient { get => _klient; set => SetProperty(ref _klient, value); }
        private bool UseSQLite;

        public AddClientViewModel(bool usesqlite)
        {
            UseSQLite = usesqlite;
            this.klient = new Klient();
            AddClientCommand = new AsyncRelayCommand(AddClient);

        }

        private IDBConnector GetNewDBConnector()
        {
            if (UseSQLite)
            {
                return new SQLiteDBConnector();
            }
            else
            {
                return new MySqlDBConnector();
            }
        }

        private async Task AddClient()
        {
            if (!klient.CheckEmail(klient.email))
            {
                MessageBox.Show("Zły format adresu Email: " + klient.email);
                return;
            }
            if (!klient.CheckPhone(klient.telefon))
            {
                MessageBox.Show("Zły format numeru telefonu: " + klient.telefon);
                return;
            }
            if (!klient.CheckPesel(klient.pesel))
            {
                MessageBox.Show("Zły format Peselu: " + klient.pesel);
                return;
            }
            if (klient.IsEmpty())
            {
                MessageBox.Show("Jedno z pól jest puste!");
                return;
            }
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Czy na pewno chceż dodać tego użytkownika?\n" + klient.ShowPrettyData(), "Potwierdź dodanie czytlenika", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                try
                {
                    if (await GetNewDBConnector().AddClient(klient.name, klient.surname, klient.pesel, klient.email, klient.telefon) < 1)
                    {
                        MessageBox.Show("Cos poszlo nie tak!");
                    }
                    else
                    {
                        MessageBox.Show("Dodano Czytelnika!");
                    }

                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }


            }


        }



    }
}
