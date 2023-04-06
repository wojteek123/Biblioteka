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
        public Klient klient { get => _klient; set=> SetProperty(ref _klient, value); }

        private string _Title;
        public string Title { get => _Title; set => SetProperty(ref _Title, value); }

        private ObservableCollection<Borrow> _borrows;
        public ObservableCollection<Borrow> borrows { get => _borrows; set => SetProperty(ref _borrows, value); }

        private int _selectedBorrow;
        public int selectedBorrow { get => _selectedBorrow; set => SetProperty(ref _selectedBorrow, value); }

        public ClientDetailsViewModel()
        {

        }
        public ClientDetailsViewModel(Klient klient)
        {
            borrows = new ObservableCollection<Borrow>();
            this.klient = klient;
            Title = klient.name + " " + klient.surname;

            GetBorrows();
            /*
            var CTask = new Task(() => { GetBorrows(); });
            CTask.Start();
            CTask.Wait();


            */
        }
        private async void GetBorrows()
        {
            try
            {
                using (var reader = await new DBConnector().GetBorrows(klient.ID))
                {

                    if (!reader.HasRows)
                    {
                        return;
                    }

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
                using (var conn = new DBConnector())
                {
                    var newCopies = int.Parse(borrows[selectedBorrow].BookCopy);
                    newCopies++;
                    var asd = conn.ModifyBorrow(borrows[selectedBorrow].ID, currentDate);

                    if (await conn.ModifyBorrow(borrows[selectedBorrow].ID, currentDate) != 1)
                    {
                        MessageBox.Show("Coś poszło nie tak!");
                    }
                    else
                    {
                        if (await conn.ModifyBookCopy(borrows[selectedBorrow].BookID, newCopies.ToString()) != 1)
                        {
                            MessageBox.Show("Coś poszło nie tak!");
                        }
                        else
                        {
                            currentDate = currentDate.Replace('-', '.');
                            borrows[selectedBorrow].BackDate = currentDate;


                        }

                    }


                }

            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }






        }





    }
}
