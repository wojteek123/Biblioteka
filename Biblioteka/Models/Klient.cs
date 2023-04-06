using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Biblioteka.Models
{
    public class Klient : ObservableObject
    {
        private string _ID;
        private string _name;
        private string _surname;
        private string _pesel;
        private string _email;
        private string _telefon;
        public ICommand ClientCommand { get; set; }

        public string ID {  get => _ID;  set => SetProperty(ref _ID, value); }
        public string name{ get => _name;set => SetProperty(ref _name, value); }
        public string surname { get => _surname; set => SetProperty(ref _surname, value); }
        public string pesel{ get => _pesel; set => SetProperty(ref _pesel, value); }
        public string email{ get => _email; set => SetProperty(ref _email, value); }
        public string telefon{  get => _telefon; set =>SetProperty(ref _telefon, value); }


        public Klient() { ID = name = surname = pesel = email = telefon = ""; }

        public Klient(string ID,string name, string surname, string pesel,string email,string telefon)
        {
            this.ID = ID;
            this.name = name;
            this.surname = surname;
            this.pesel = pesel;
            this.email = email;
            this.telefon = telefon;
        }

        bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }

        public bool CheckEmail(string match)
        {
            Regex regex = new Regex("[a-z0-9]+@[a-z]+\\.[a-z]{2,3}");
            return regex.IsMatch(match);
        }
        public bool CheckPhone(string match)
        {
            Regex regex = new Regex("(\\d{9})");
            var NewPhone = Regex.Match(match, regex.ToString());
            return regex.IsMatch(match);
        }
        public bool CheckPesel(string match)
        {
            Regex regex = new Regex("(\\d{11})");
            var NewPesel = Regex.Match(match, regex.ToString());
            return regex.IsMatch(match);
        }
        public bool IsEmpty()
        {
            if(name =="" || surname =="" || pesel=="" || email=="" || telefon=="")
                return true;
            else
                return false;

        }
        public string ShowPrettyData()
        {
            string data = " Imię: " + name + "\n Nazwisko: " + surname + "\n Pesel: " + pesel + "\n Email: " + email + " \n Telefon: " + telefon;
            return data;
        }


    }
}
