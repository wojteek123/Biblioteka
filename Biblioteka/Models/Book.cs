using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Input;

namespace Biblioteka.Models
{
    public class Book : ObservableObject
    {

        private string _ID;
        private string _tytul;
        private string _autor;
        private string _gatunek;
        private string _wydawca;
        private string _egzemplarze;
        private string _datawydania;

        public ICommand BookCommand { get; set; }

        public ICommand AddBookCommand { get; set; }
        public ICommand RemoveBookCommand { get; set; }

        public string ID { get => _ID;   set => SetProperty(ref _ID, value); }
        public string tytul { get => _tytul;  set => SetProperty(ref _tytul, value);  }
        public string autor { get => _autor;  set => SetProperty(ref _autor, value);  }
        public string gatunek { get => _gatunek;  set => SetProperty(ref _gatunek, value); }
        public string wydawca { get => _wydawca; set => SetProperty(ref _wydawca, value);  }
        public string egzemplarze {  get => _egzemplarze; set => SetProperty(ref _egzemplarze, value); }
        public string datawydania{ get => _datawydania;  set =>SetProperty(ref _datawydania, value);   }


        public Book() { ID = tytul = autor = gatunek = wydawca = egzemplarze = ""; datawydania = "1/29/2000 12:00:00 AM";}

        public Book(Book book)
        {
            this.ID = book.ID;
            this.tytul = book.tytul;
            this.gatunek = book.gatunek;
            this.wydawca = book.wydawca;
            this.autor = book.autor;
            this.egzemplarze = book.egzemplarze;
            this.datawydania = book.datawydania;
        }

        public Book(string ID,string tytul,string gatunek,string wydawca,string autor,string egzemplarze,string data)
        {
            this.ID = ID;
            this.tytul = tytul;
            this.gatunek = gatunek;
            this.wydawca = wydawca;
            this.autor = autor;
            this.egzemplarze = egzemplarze;
            this.datawydania = data;


        }
        public string MakeSQLDateOnly()
        {
            if (datawydania != null || datawydania.Contains(" "))
            {
                string SQLDate;
                string[] splittedDateTime = datawydania.Split(' ');
                SQLDate = splittedDateTime[0];
                SQLDate = SQLDate.Replace('/','-');
                string[] splitedDate = SQLDate.Split('-');
                SQLDate = splitedDate[2] +'-' +splitedDate[0] +'-'+splitedDate[1];
                return SQLDate;

            }
            return datawydania;

        }
        public bool IsEmpty()
        {
            if (tytul == "" || gatunek == "" || wydawca == "" || autor == "" || egzemplarze == "" || datawydania=="" )
                return true;
            else
                return false;

        }
        public string ShowPrettyData()
        {
            string data = " Tytuł: " + tytul + "\n Gatunek: " + gatunek + "\n Wydawca: " + wydawca + "\n Autor: " + autor + " \n Ilość egzemplarzy: " + egzemplarze +"\n Data wydania: "+ MakeSQLDateOnly();
            return data;
        }

    }
}

//SELECT IDKsiazki, Tytul, gatunek.Nazwa, Wydawnictwo, CONCAT(autorzy.Imie, " ", autorzy.Nazwisko) AS autor, Egzemplarze, ksiazki.Data_wydania FROM ksiazki
//LEFT JOIN autorzy ON ksiazki.AutorID=autorzy.AutorID
//LEFT JOIN gatunek ON ksiazki.GatunekID=gatunek.GatunekID;