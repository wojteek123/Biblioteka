﻿using Biblioteka.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;



namespace Biblioteka.ViewModels
{

    public class AddBookViewModel : ObservableObject
    {
        public ICommand AddBook { get; set; }



        public ObservableCollection<string> gatunki { get; set; }

        private Book _book;
        public Book book
        {
            get => _book;
            set => SetProperty(ref _book, value);
        }

        private bool UseSQLite;

        public AddBookViewModel(bool usesqlite)
        {
            UseSQLite = usesqlite;
            Init();
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


        private void Init()
        {
            gatunki = new ObservableCollection<string>();
            book = new Book();
            book.egzemplarze = "1";


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
            

           AddBook = new AsyncRelayCommand(addBook);


        }


        private async Task addBook()
        {

            if (book.IsEmpty())
            {
                MessageBox.Show("Jedno z pól jest puste!");
                return;
            }

            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Jesteś pewien że chcesz dodać taką książkę?\n "+book.ShowPrettyData(), "Potwierdzenie", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult != MessageBoxResult.Yes)
                return;

            

            try
            {
                using (var db = GetNewDBConnector())
                {
                    var reader = await db.GetGatunekID(book.gatunek);
                    string gatunekID = "1";
                    if (reader.Read())
                        gatunekID = reader.GetValue(0).ToString();
                    if (await GetNewDBConnector().AddBook(book.tytul, gatunekID, book.wydawca, book.autor, book.egzemplarze, book.MakeSQLDateOnly()) < 1)
                        MessageBox.Show("Coś poszło nie tak!");
                    else
                        MessageBox.Show("Dodano " + book.tytul + "!");

                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }



            Console.WriteLine(book.gatunek + book.tytul + book.wydawca + book.datawydania + book.autor + book.egzemplarze);

        }
    }
}
