using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Input;

namespace Biblioteka.Models
{
    public class Borrow : ObservableObject
    {
        private string _ID;
        private string _Title;
        private string _BookID;
        private string _BorrowDate;
        private string _DateToGetBack;
        private string _BackDate;
        private string _BookCopy;
        public ICommand BorrowCommand { get; set; }
        

        public string ID { get => _ID; set => SetProperty(ref _ID, value); }
        public string BookID { get => _BookID; set => SetProperty(ref _BookID, value); }
        public string Title { get => _Title; set => SetProperty(ref _Title, value); }
        public string BorrowDate { get => _BorrowDate; set => SetProperty(ref _BorrowDate, value); }
        public string DateToGetBack { get => _DateToGetBack; set => SetProperty(ref _DateToGetBack, value); }
        public string BackDate { get => _BackDate; set => SetProperty(ref _BackDate, value); }
        public string BookCopy { get => _BookCopy; set => SetProperty(ref _BookCopy, value); }

        public Borrow()
        {

        }
        public Borrow(string borrowID, string title, string borrowDate, string TimetToGetBack, string GotBackTime,string BookID,string BookCopy)
        {
            ID = borrowID;
            this.BookID = BookID;
            Title = title;
            BorrowDate = borrowDate;
            DateToGetBack = TimetToGetBack;
            BackDate = GotBackTime.Contains("00.00.0000") ? "Nie oddano!" : GotBackTime;
            this.BookCopy = BookCopy;



        }

    }
}
