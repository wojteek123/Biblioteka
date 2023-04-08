using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteka.Models
{
    public interface IDBConnector : IDisposable //HACK shallow module red flag 
    {
        void Close();
        
        Task<DbDataReader> SearchClient(string search);
        Task<DbDataReader> SearchBook(string search);
        Task<DbDataReader> SearchGenre();
        DbDataReader SearchGenreSync();


        Task<DbDataReader> GetGatunekID(string name);
        Task<DbDataReader> ClientsWithBooks();


        Task<int> AddBook(string tytul, string gatunek, string wydawca, string autor, string egzemplarze, string data);
        Task<int> AddClient(string name, string surname, string pesel, string email, string telefon);

        Task<DbDataReader> GetBorrows(string ClientID);
        Task<int> Borrow(string IDKlienta, string IDKsiazki, string data_wypozyczenia, string czas_do_zwrotu);
        Task<DbDataReader> GetCopies(string IDKsiazki);
        Task<int> ModifyBookCopy(string IDKsiazki, string setCopies);
        Task<int> ModifyBorrow(string IDBorrow, string backdate);















    }
}
