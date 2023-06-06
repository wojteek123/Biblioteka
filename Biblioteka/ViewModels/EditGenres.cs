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
    public class EditGenres : DbChoose
    {
        public ICommand EditGenresCommand { get; set; }

        public ObservableCollection<string> gatunki { get; set; }

        public EditGenres()
        {
            Init();

            EditGenresCommand = new RelayCommand(editGenres);
        }


        private void Init()
        {
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




        }




        Biblioteka.Views.EditGenres genresWindow;
        private void editGenres()
        {
            if (genresWindow != null)
            {
                genresWindow.Focus();
                return;
            }

            genresWindow = new Biblioteka.Views.EditGenres();
            var datacontext = new ViewModels.EditGenresViewModel(gatunki);
            genresWindow.DataContext = datacontext;
            genresWindow.Show();
            //genresWindow.Closing += Client_Closed;
            genresWindow.Closed += Client_Closed;


        }

        private void Client_Closed(object sender, EventArgs e)
        {
            genresWindow = null;

        }
    }
}
