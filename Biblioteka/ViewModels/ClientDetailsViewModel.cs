using Biblioteka.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Biblioteka.ViewModels
{
    public class ClientDetailsViewModel : ObservableObject
    {
        private Klient _klient;
        public Klient klient { get => _klient; set => SetProperty(ref _klient, value); }

        private string _Title;
        public string Title { get => _Title; set => SetProperty(ref _Title, value); }

        private ObservableCollection<Borrow> _borrows;
        public ObservableCollection<Borrow> borrows { get => _borrows; set => SetProperty(ref _borrows, value); }

        private int _selectedBorrow;
        public int selectedBorrow { get => _selectedBorrow; set => SetProperty(ref _selectedBorrow, value); }

        private bool UseSQLite;
        public ClientDetailsViewModel() { }
        public ClientDetailsViewModel(bool usesqlite, Klient klient)
        {
            UseSQLite = usesqlite;
            borrows = new ObservableCollection<Borrow>();
            this.klient = klient;
            Title = klient.name + " " + klient.surname;

            GetBorrows();

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
        private async void GetBorrows()
        {
            try
            {
                using (var db = GetNewDBConnector())
                {
                    var reader = await db.GetBorrows(klient.ID);

                    while (reader.Read())
                    {
                        string temp;
                        if (reader.IsDBNull(4))
                        {
                            temp = "00.00.0000";
                        }
                        else
                        {
                            temp = DateConverter.GetDateOnly(reader.GetValue(4).ToString());
                        }

                        borrows.Add(new Borrow(reader.GetValue(0).ToString(), reader.GetString(1),
                           DateConverter.GetDateOnly(reader.GetValue(2).ToString()), DateConverter.GetDateOnly(reader.GetValue(3).ToString()),
                            temp, reader.GetValue(5).ToString(), reader.GetValue(6).ToString()
                            ));
                        borrows.Last().BorrowCommand = new AsyncRelayCommand(ConfirmBack);
                    }

                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

        }



        private async Task ConfirmBack()
        {
            if (!borrows[selectedBorrow].BackDate.Contains("Nie oddano!"))
            {
                MessageBox.Show("Książkę juz oddano!");
                return;
            }

            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Potwierdzić oddanie książki?\n", "Potwierdź", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.No)
                return;

            string currentDate = DateConverter.MakeDatePickerDate(DateTime.Now.ToString());
            currentDate = DateConverter.MakeSQLDateOnly(currentDate);



            try
            {
                using (var db = GetNewDBConnector())
                {
                    var reader = await db.GetCopies(borrows[selectedBorrow].BookID);
                    if (reader.Read())
                        borrows[selectedBorrow].BookCopy = reader.GetValue(0).ToString();

                    var newCopies = int.Parse(borrows[selectedBorrow].BookCopy);
                    MessageBox.Show("New copie: "+newCopies+"borrwos copies:" +borrows[selectedBorrow].BookCopy );
                    newCopies++;

                    await db.ModifyBorrow(borrows[selectedBorrow].ID, currentDate);
                    var zmieniono =await db.ModifyBookCopy(borrows[selectedBorrow].BookID, newCopies.ToString());
                    MessageBox.Show("ROWS:"+zmieniono);
                    currentDate = currentDate.Replace('-', '.');
                    borrows[selectedBorrow].BackDate = currentDate;

                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }






}





    }
}
