using Biblioteka.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Biblioteka.ViewModels
{
    class EditBookViewModel : ObservableObject
    {
        public ICommand EditBookCommand { get; }
        

        public Book OryginalBook;
        private Book _NewBook;
        public Book NewBook
        {
            get => _NewBook;
            set => SetProperty(ref _NewBook, value);
        }
        private bool UseSQLite = true;

        public ObservableCollection<string> gatunki { get; set; }



        public EditBookViewModel(Book editedBook)
        {
            OryginalBook = editedBook;
            NewBook = new Book(editedBook);

            gatunki = new ObservableCollection<string>();
            try
            {
                using (var db = GetNewDBConnector())
                {
                    var reader = db.SearchGenreSync();
                    if (!reader.HasRows)
                    {
                        MessageBox.Show("Nie znaleziono gatunków!");
                        return;
                    }

                    while (reader.Read())
                    {
                        gatunki.Add(reader.GetString(0));
                    }
                }
            }
            catch (Exception e) { MessageBox.Show(e.Message); }


            EditBookCommand = new AsyncRelayCommand(SaveChanges);

        }


        private async Task SaveChanges()
        {
            try
            {
                using (var db = GetNewDBConnector())
                {
                    var reader = await db.GetGatunekID(NewBook.gatunek);
                    string gatunekID = "1";
                    while (reader.Read())
                    {
                        gatunekID = reader.GetValue(0).ToString();
                    }
                    if (await db.EditBook(NewBook.ID, NewBook.tytul, gatunekID, NewBook.wydawca, NewBook.autor, NewBook.egzemplarze, NewBook.MakeSQLDateOnly()) !=1 )
                    {
                        MessageBox.Show("Cos poszlo nie tak!");
                        return;
                    }
                    else
                    {
                        MessageBox.Show("Pomyślnie zmodyfikowano '"+NewBook.tytul+"'!");
                        OryginalBook = NewBook;
                    }
                }
            }
            catch (Exception e) { MessageBox.Show(e.Message); }

            //MessageBox.Show(NewBook.ShowPrettyData());

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
    }
}
