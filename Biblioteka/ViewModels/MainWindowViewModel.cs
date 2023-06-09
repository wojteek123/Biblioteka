﻿
using Biblioteka.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Biblioteka.ViewModels
{

    public class MainWindowViewModel : ObservableObject
    {

        public ICommand SearchClientCommand { get; }
        public ICommand SearchBookCommand { get; }
        public ICommand ClientDetailsCommand { get; }
        public ICommand BorrowCommand { get; }
        public ICommand ClientsWithBooksCommand { get; }
        public ICommand ChangeDatabase { get; }
        public ICommand EditBookCommand { get; }



        private bool _UseSQLite;
        public bool UseSQLite { get => _UseSQLite; set => SetProperty(ref _UseSQLite, value); }

        private string _currentDate;
        public string currentDate { get => _currentDate; set => SetProperty(ref _currentDate, value); }

        private int _toBorrowDays;
        public int toBorrowDays { get => _toBorrowDays; set => SetProperty(ref _toBorrowDays, value); }

        private string _searchBookstring;
        public string searchBookString { get => _searchBookstring; set => SetProperty(ref _searchBookstring, value); }

        private string _searchClientstring;
        public string searchClientstring { get => _searchClientstring; set => SetProperty(ref _searchClientstring, value); }

        private int _selectedClient;
        public int selectedClient { get => _selectedClient; set => SetProperty(ref _selectedClient, value); }

        private int _selectedBook;
        public int selectedBook { get => _selectedBook; set => SetProperty(ref _selectedBook, value); }

        private int _selectedBorrow;
        public int selectedBorrow { get => _selectedBorrow; set => SetProperty(ref _selectedBorrow, value); }

        private Klient _SelectedKlient;
        public Klient SelectedClient { get => _SelectedKlient; set => SetProperty(ref _SelectedKlient, value); }

        private ObservableCollection<Klient> _users;
        public ObservableCollection<Klient> users { get => _users; set => SetProperty(ref _users, value); }

        private ObservableCollection<Book> _books;
        public ObservableCollection<Book> books { get => _books; set => SetProperty(ref _books, value); }

        private ObservableCollection<Book> _booksToBorrow;
        public ObservableCollection<Book> booksToBorrow { get => _booksToBorrow; set => SetProperty(ref _booksToBorrow, value); }

        private AddBookViewModel _AddBookVM;
        private AddClientViewModel _AddClientVM;
        public AddBookViewModel AddBookVM { get => _AddBookVM; set => SetProperty(ref _AddBookVM, value); }
        public AddClientViewModel AddClientVM { get => _AddClientVM; set => SetProperty(ref _AddClientVM, value); }

        
        public MainWindowViewModel()
        {
            UseSQLite = true;
            Init();
            ClientDetailsWindows = new List<ClientDetailsViewModel>();



            ChangeDatabase = new RelayCommand(Init);
            SearchClientCommand = new AsyncRelayCommand(SearchClient);
            SearchBookCommand = new AsyncRelayCommand(SearchBook);
            ClientDetailsCommand = new RelayCommand(ShowClientDetails);
            BorrowCommand = new RelayCommand(Borrow);
            ClientsWithBooksCommand = new AsyncRelayCommand(SearchClientsWithBooks);
            EditBookCommand = new RelayCommand(EditBook);
        }
        private void Init()
        {
            currentDate = DateConverter.MakeDatePickerDate(DateTime.Now.ToString());
            toBorrowDays = 14;

            searchBookString = "";
            searchClientstring = "";


            users = new ObservableCollection<Klient>();
            books = new ObservableCollection<Book>();
            booksToBorrow = new ObservableCollection<Book>();



            booksToBorrow.Clear();


            AddBookVM = new AddBookViewModel(UseSQLite);
            AddClientVM = new AddClientViewModel(UseSQLite);

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

        private async void Borrow()
        {
            if (booksToBorrow.Count == 0)
            {
                MessageBox.Show("Nie wybrano żadnych ksiązek!");
                return;
            }
            foreach (Book book in booksToBorrow) //TODO działa ale to jest źle   ŹLE
            {
                if (int.Parse(book.egzemplarze) < 1)
                {
                    MessageBox.Show("Brak egzemplarzy tej książki w bibliotece!");
                    return;
                }
            }



            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Czy na pewno chcesz dokonać wypożyczenia?\n", "Potwierdź wypożyczenie", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.No)
                return;

            if(toBorrowDays <= 1)
            {
                MessageBox.Show("Okres wypożyczenia nie może być krótszy niż 2 dni!");
                return;
            }

            string dataOddania = DateConverter.AddDaysTo(currentDate, toBorrowDays);
            dataOddania = DateConverter.MakeSQLDateOnly(dataOddania);

            string dataWyp = DateConverter.MakeSQLDateOnly(currentDate);


            for (int i = 0; i < booksToBorrow.Count; i++)
            {
                int copies = int.Parse(booksToBorrow[i].egzemplarze);
                copies--;
                try
                {
                    using (var db = GetNewDBConnector())
                    {

                        if (await db.Borrow(users[selectedClient].ID, booksToBorrow[i].ID, dataWyp, dataOddania) != 1)
                        {
                            MessageBox.Show("Cos poszlo nie tak!");
                            // TODO restore transaction
                        }
                        if (await db.ModifyBookCopy(booksToBorrow[i].ID, copies.ToString()) != 1)
                        {
                            MessageBox.Show("Cos poszlo nie tak!");
                            // restore transaction
                        }
                        else
                        {
                            //refresh current shown books copies and borrows
                            for (int j = 0; j < books.Count; j++)
                            {
                                if (books[j].ID == booksToBorrow[i].ID)
                                {
                                    books[j].egzemplarze = copies.ToString();
                                    booksToBorrow[i].egzemplarze = copies.ToString();
                                    //TODO add code for refreshing main window books

                                }

                            }

                        }
                    }

                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }


            MessageBox.Show("Dodano Wypożyczenie!");

        }


        private List<ClientDetailsViewModel> ClientDetailsWindows;
        private void ShowClientDetails()
        {
            bool b_CanCreate = true;
            //TODO show only one
            foreach (ClientDetailsViewModel clientWinowVM in ClientDetailsWindows)
            {
                if(clientWinowVM.klient.ID == users[selectedClient].ID)
                {
                    b_CanCreate = false;

                    

                }
            }
            if (b_CanCreate)
            {
                var client = new Biblioteka.Views.ClientDetails();
                var datacontext = new ClientDetailsViewModel(UseSQLite, users[selectedClient]);
                client.DataContext = datacontext;
                ClientDetailsWindows.Add(datacontext);
                client.Show();
                //client.Closed += Client_Closed;
                client.Closing += Client_Closed;
            }



        }

        private void Client_Closed(object sender, EventArgs e)
        {
            var Klient = (Views.ClientDetails)sender;
            ClientDetailsViewModel toRemove = new ClientDetailsViewModel();

            foreach (ClientDetailsViewModel clientWinowVM in ClientDetailsWindows)
            {
                if(Klient.DataContext == clientWinowVM)
                {
                    toRemove = clientWinowVM;
                }
            }
            ClientDetailsWindows.Remove(toRemove);


        }

        private void AddBorrow()
        {
            if (!booksToBorrow.Contains(books[selectedBook]))
            {
                booksToBorrow.Add(books[selectedBook]);
                books[selectedBook].BookCommand = new RelayCommand(RemoveBorrow);
            }
        }
        private void RemoveBorrow()
        {
            booksToBorrow.RemoveAt(selectedBorrow);
        }



        private async Task SearchBook()
        {

            try
            {
                using (var db = GetNewDBConnector())
                {
                    var reader = await db.SearchBook(searchBookString);
                    if (!reader.HasRows)
                        MessageBox.Show("Nie znaleziono ksiazek");
                    else
                        books.Clear();

                    while (reader.Read())
                    {
                        books.Add(new Book(reader.GetValue(0).ToString(), reader.GetString(1),
                            reader.GetString(2), reader.GetString(3),
                            reader.GetString(4), reader.GetValue(5).ToString(),
                            DateConverter.GetDateOnly(reader.GetValue(6).ToString())
                            ));
                        books.Last().BookCommand = new RelayCommand(AddBorrow);
                        books.Last().AddBookCommand = new AsyncRelayCommand(AddCopy);
                        books.Last().RemoveBookCommand = new AsyncRelayCommand(RemoveCopy);
                    }

                }

            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        private async Task SearchClient()
        {
            try
            {
                using (var db = GetNewDBConnector())
                {
                    var reader = await db.SearchClient(searchClientstring);

                        if (!reader.HasRows)
                        MessageBox.Show("Nie znaleziono użytkownika");
                    else
                        users.Clear();

                    while (reader.Read())
                    {
                        users.Add(new Klient(reader.GetValue(0).ToString(), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5)));
                        users.Last().ClientCommand = new RelayCommand(ShowClientDetails);
                    }
                }

            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }


        }
        private async Task SearchClientsWithBooks()
        {
            users.Clear();
            try
            {
                using (var db = GetNewDBConnector()) {
                {
                        var reader = await db.ClientsWithBooks();
                        if (!reader.HasRows)
                        {
                            MessageBox.Show("Nie znaleziono użytkownika");
                            return;
                        }
                        else
                            users.Clear();

                        while (reader.Read())
                        {
                            string ID = reader.GetValue(0).ToString();
                            bool b_Add = true;
                            foreach (Klient klient in users)
                            {
                                if (klient.ID == ID)
                                    b_Add = false;
                            }
                            if (b_Add)
                            {
                                users.Add(new Klient(ID, reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5)));
                                users.Last().ClientCommand = new RelayCommand(ShowClientDetails);
                            }

                        }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }



        }



        private async Task AddCopy()
        {
            int copies = int.Parse(books[selectedBook].egzemplarze);
            copies++;
            try
            {
                using (var db = GetNewDBConnector())
                {

                    if (await db.ModifyBookCopy(books[selectedBook].ID, copies.ToString()) != 1)
                    {
                        MessageBox.Show("Cos poszlo nie tak! :(");
                    }
                    else
                    {
                        MessageBox.Show("Dodano Kopię!");
                        books[selectedBook].egzemplarze = copies.ToString();
                    }
                }

            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }


        }
        private async Task RemoveCopy()
        {

            int copies = int.Parse(books[selectedBook].egzemplarze);
            if(copies < 1)
            {
                MessageBox.Show("Brak kopii ksiązki!");
                return;
            }
            copies--;

            try
            {
                using (var db = GetNewDBConnector())
                {

                    if (await db.ModifyBookCopy(books[selectedBook].ID, copies.ToString()) != 1)
                    {
                        MessageBox.Show("Cos poszlo bardzo nie tak! :(");
                    }
                    else
                    {
                        MessageBox.Show("Usunięto Kopię!");
                        books[selectedBook].egzemplarze = copies.ToString();
                    }
                }

            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

            //if (warunek)
            //{
            //    // blok kodu do wykonania, jeśli warunek jest prawdziwy
            //}

            //int liczba = 5;
            //if (liczba > 0)
            //{
            //    Console.WriteLine("Liczba jest dodatnia.");
            //}


            //int liczba = 5;
            //if (liczba > 0)
            //{
            //    Console.WriteLine("Liczba jest dodatnia.");
            //}
            //else if (liczba < 0)
            //{
            //    Console.WriteLine("Liczba jest ujemna.");
            //}
            //else
            //{
            //    Console.WriteLine("Liczba wynosi zero.");
            //}

            //int liczba = 10;
            //if (liczba > 0)
            //{
            //    if (liczba % 2 == 0)
            //    {
            //        Console.WriteLine("Liczba jest dodatnia i parzysta.");
            //    }
            //    else
            //    {
            //        Console.WriteLine("Liczba jest dodatnia, ale nieparzysta.");
            //    }
            //}
            //else
            //{
            //    Console.WriteLine("Liczba jest mniejsza lub równa zero.");
            //}




            //char opcja = 'B';
            //switch (opcja)
            //{
            //    case 'A':
            //         Console.WriteLine("Wybrano opcję A.");
            //        break;
            //    case 'B':
            //         Console.WriteLine("Wybrano opcję B.");
            //        break;
            //    case 'C':
            //         Console.WriteLine("Wybrano opcję C.");
            //        break;
            //    default:
            //         Console.WriteLine("Nieznana opcja.");
            //        break;
            //}


            //int liczba = 5;
            //// Pętla while
            //while (liczba < 5)
            //{
            //    Console.WriteLine("Pętla while");
            //    liczba++;
            //}

            //// Pętla do-while
            //do
            //{
            //    Console.WriteLine("Pętla do-while");
            //    liczba++;
            //} while (liczba < 5);


            //for (inicjalizacja; warunek; iteracja)
            //{
            //    // blok kodu do wykonania
            //}

            //for (int i = 0; i < 5; i++)
            //{
            //    Console.WriteLine(i);
            //}



            //foreach (typ element in kolekcja)
            //{
            //    // blok kodu do wykonania dla każdego elementu
            //}


            //string[] imiona = { "Anna", "Kamil", "Maria", "Adam" };

            //foreach (string imie in imiona)
            //{
            //    Console.WriteLine("Witaj, " + imie + "!");
            //}


            //for (int i = 1; i <= 10; i++)
            //{
            //    if (i == 5)
            //    {
            //        break;
            //    }
            //    Console.WriteLine(i);
            //}




            //for (int i = 1; i <= 5; i++)
            //{
            //    if (i == 3)
            //    {
            //        continue;
            //    }
            //    Console.WriteLine(i);
            //}


            float divide(float a,float b)
            {
                if (b == 0)
                {
                    throw new DivideByZeroException($"{a}/{b}");
                }
                return a / b;
            }


            try
            {
                // Kod, który może zgłosić wyjątek
                float result = divide(10, 0);
                Console.WriteLine("Wynik: " + result);
            }
            catch (DivideByZeroException ex)
            {
                // Obsługa konkretnego rodzaju wyjątku
                Console.WriteLine("Wystąpił błąd dzielenia przez zero: " + ex.Message);
            }
            catch (Exception ex)
            {
                // Obsługa ogólnego typu wyjątku
                Console.WriteLine("Wystąpił nieoczekiwany błąd: " + ex.Message);
            }







        }



        Biblioteka.Views.EditBook EditBookWindow;
        ViewModels.EditBookViewModel EditBookDataContext;

        private void EditBook()
        {
            if (EditBookWindow != null)
            {
                if(books[selectedBook] != EditBookDataContext.OryginalBook)
                {
                    EditBookDataContext = new ViewModels.EditBookViewModel(books[selectedBook]);
                    EditBookWindow.DataContext = EditBookDataContext;
                    EditBookWindow.Title = books[selectedBook].tytul;
                }

                EditBookWindow.Focus();
                return;
            }

            EditBookWindow = new Biblioteka.Views.EditBook();

            EditBookDataContext = new ViewModels.EditBookViewModel(books[selectedBook]);
            EditBookWindow.DataContext = EditBookDataContext;
            EditBookWindow.Title = books[selectedBook].tytul;
            EditBookWindow.Show();
            EditBookWindow.Closing += EditBook_Closing;
            EditBookWindow.Closed += EditBook_Closed;


        }
        private void EditBook_Closing(object sender, EventArgs e)
        {
            EditBookDataContext = null;
        }
        private void EditBook_Closed(object sender, EventArgs e)
        {
            EditBookWindow = null;
            EditBookDataContext = null;
        }


    }
}









/*


3 zakladki: ksiazki,czytelnicy,wyporzycz ksiazke dla czytelnika

ksiazki{

wyswietl ksiazke
dodac nowa ksiazke do bazy
dodac istniejaca ksiazke do bazy danych

}
czytelnicy{
wyswietl
dodaj nowego
wyporzycz ksiazke dla istneijacego (nowe menu )

potwierdz oddanie ksiazki
}

-wyswietl nie oddane ksiazki




SELECT IDKsiazki,Tytul,gatunek.Nazwa,Wydawnictwo,CONCAT(autorzy.Imie," ",autorzy.Nazwisko) AS autor,Egzemplarze,ksiazki.Data_wydania FROM ksiazki
LEFT JOIN autorzy ON ksiazki.AutorID=autorzy.AutorID
LEFT JOIN gatunek ON ksiazki.GatunekID=gatunek.GatunekID;

*/