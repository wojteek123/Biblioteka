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
    class EditGenresViewModel : ObservableObject
    {
        public ICommand AddNewCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        private int _selectedGenre;
        public int selectedGenre { get => _selectedGenre; set => SetProperty(ref _selectedGenre, value); }
        private string _NewGenre;
        public string NewGenre { get => _NewGenre; set => SetProperty(ref _NewGenre, value); }

        public ObservableCollection<string> Genres { get; set; }

        public EditGenresViewModel( ObservableCollection<string> genres)
        {
            this.Genres = genres;

            SaveCommand = new AsyncRelayCommand(SaveChange);
            AddNewCommand = new AsyncRelayCommand(AddNew);
            DeleteCommand = new AsyncRelayCommand(Delete);

        }
        private IDBConnector GetNewDBConnector()
        {
            if(true)   /// (UseSQLite)
            {
                return new SQLiteDBConnector();
            }
            else
            {
                return new MySqlDBConnector();
            }
        }

        private async Task AddNew()
        {
            if (NewGenre == null || NewGenre == "")
            {
                MessageBox.Show("Taki gatunek juz jest, lub nazwa gatunku jest pusta!");
                return;
            }

           
            foreach (string genre in Genres)
            {
                string LowerCaseGenre = genre.ToLower();
                string LowerCaseNewGenre = NewGenre.ToLower();
                if(LowerCaseGenre == LowerCaseNewGenre)
                {
                    MessageBox.Show("Taki gatunek juz jest w bazie!");
                    return;
                }
            }


            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show($"Jesteś pewien że chcesz dodać '{NewGenre}' do listy gatunków ?", "Potwierdzenie", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult != MessageBoxResult.Yes)
                return;


            try
            {
                using (var db = GetNewDBConnector())
                {

                    if (await db.AddGenre(NewGenre) != 1)
                    {
                        Console.WriteLine("Cos poszlo nie tak!");
                        // TODO restore transaction
                    }
                    else
                    {

                        MessageBox.Show($"Dodano nowy gatunek: {NewGenre} ");


                        Genres.Add(NewGenre);
                        selectedGenre = Genres.Count - 1;

                        

                        

                    }
                }

            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }




        }

        private async Task SaveChange()
        {
            try
            {

                if (NewGenre == "" || NewGenre == null || Genres[selectedGenre] == null || Genres[selectedGenre] == NewGenre )
                {
                    MessageBox.Show("Nie wybrano gatunku do edycji, nowa nazwa jest pusta lub taka sama ");
                    return;

                }
            }
            catch (IndexOutOfRangeException e){    return;  }
            

            try
            {
                using (var db = GetNewDBConnector())
                {

                    if (await db.ModifyGenre(Genres[selectedGenre],NewGenre) != 1)
                    {
                        Console.WriteLine("Cos poszlo nie tak!");
                        // TODO restore transaction
                    }
                    else
                    {
                        MessageBox.Show($"Zmieniono '{Genres[selectedGenre]}' na '{NewGenre}' ");

                        int tempIndex = selectedGenre; //bez tego combobox sie resetuje i nic nie jest wybrane
                        Genres[selectedGenre] = NewGenre;
                        selectedGenre = tempIndex;

                    }
                }

            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }


        }
    
        private async Task Delete()
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show($"Jesteś pewien że chcesz usunąć {Genres[selectedGenre]} z listy gatunków ?", "Potwierdzenie", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult != MessageBoxResult.Yes)
                return;


            try
            {
                using (var db = GetNewDBConnector())
                {


                    var reader = await db.CheckForForeignKeysInGenres(Genres[selectedGenre]); 
                    if (reader.Read()) //sprawdz czy gatunek jest uzywany (bo slite nie chce) //TODO dodac do sqlite ON DELeTE RESTRICT 
                    {
                        MessageBox.Show("Ten gatunek jest powiązany z przynajmniej jedną książką w bazie!");
                        return;
                    }




                    string deletedGenre = Genres[selectedGenre];
                    if (await db.RemoveGenre(Genres[selectedGenre]) != 1)
                    {
                        Console.WriteLine("Cos poszlo nie tak!");
                        // TODO restore transaction
                    }
                    else
                    {
                        MessageBox.Show($"Usunięto '{deletedGenre}'");
                        Genres.Remove(Genres[selectedGenre]);
                        selectedGenre = 0;
                        NewGenre = Genres[selectedGenre];


                    }
                }

            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }


        }

    }
}
