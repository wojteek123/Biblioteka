using MySqlConnector;
using System;
using System.Data.Common;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Biblioteka.Models
{
    internal class MySqlDBConnector : IDisposable, IDBConnector
    {

        public static string connString = "Server=db4free.net;Port=3306;User ID = libadmin;pwd=btBZW#mgTUT!z2J;Database = biblioteka;";
        private MySqlConnection connection;
        private MySqlCommand command;




        public MySqlDBConnector()
        {
            string textFile = "conn.txt";
            if (File.Exists(textFile))
            {
                string text = File.ReadAllText(textFile);
                connString = text;
                Console.WriteLine(text);
            }
            else
            {
                connection = new MySqlConnection(connString);
                return;
            }
            connection = new MySqlConnection(connString);

        }

        public void Close()
        {
            connection.Close();
            command.Dispose();
            connection.Dispose();

        }
        private async Task OpenConnection()
        {
            while (connection.State == System.Data.ConnectionState.Open)
            {
                Console.WriteLine("Słabo Słabo");
                Thread.Sleep(100);
            }
            if (connection.State == System.Data.ConnectionState.Closed)
                await connection.OpenAsync();
        }


        public Task<DbDataReader> CheckForForeignKeysInGenres(string genres)
        {
            throw new NotImplementedException();
        }

        public Task<int> ModifyGenre(string GenreName, string NewName)
        {
            throw new NotImplementedException();

        }
        public Task<int> AddGenre(string GenreName)
        {
            throw new NotImplementedException();

        }
        public Task<int> RemoveGenre(string GenreName)
        {
            throw new NotImplementedException();

        }


        public async Task<DbDataReader> SearchClient(string search)
        {
            await OpenConnection();
            string query = "SELECT * FROM klienci WHERE Imie LIKE  @search OR Nazwisko LIKE  @search;";
            command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@search", "%" + search + "%");

            return await command.ExecuteReaderAsync();


            //return await command.ExecuteReaderAsync();
        }
        public async Task<DbDataReader> SearchBook(string search)
        {
            await OpenConnection();
            string booksearch = "SELECT IDKsiazki,Tytul,gatunek.Nazwa,Wydawnictwo,Autor,Egzemplarze,ksiazki.Data_wydania FROM ksiazki LEFT JOIN gatunek ON ksiazki.GatunekID = gatunek.GatunekID WHERE Tytul LIKE @search ;";
            command = new MySqlCommand(booksearch, connection);
            command.Parameters.AddWithValue("@search", "%" + search + "%");
            return await command.ExecuteReaderAsync();

        }
        public async Task<DbDataReader> SearchGenre()
        {
            await OpenConnection();
            string booksearch = "SELECT gatunek.Nazwa FROM gatunek;";
            command = new MySqlCommand(booksearch, connection);

            return await command.ExecuteReaderAsync();
        }
        public DbDataReader SearchGenreSync()
        {
            connection.Open();
            string booksearch = "SELECT gatunek.Nazwa FROM gatunek;";
            command = new MySqlCommand(booksearch, connection);

            return command.ExecuteReader();
        }


        public async Task<DbDataReader> GetGatunekID(string name)
        {
            await OpenConnection();
            string booksearch = "SELECT GatunekID FROM gatunek WHERE gatunek.Nazwa='@name';";
            command = new MySqlCommand(booksearch, connection);
            command.Parameters.AddWithValue("@name", name);


            return await command.ExecuteReaderAsync();

        }
        public async Task<DbDataReader> ClientsWithBooks()
        {
            await OpenConnection();

            string query1 = "SELECT klienci.`IDKlienta`, `Imie`, `Nazwisko`, `Pesel`, `Email`, `Telefon` FROM `klienci` ";
            string query2 = "RIGHT JOIN wypozyczenia ON klienci.IDKlienta = wypozyczenia.IDKlienta ";
            string query3 = "WHERE wypozyczenia.Data_zwrotu IS NULL;";

            command = new MySqlCommand(query1 + query2 + query3, connection);

            return await command.ExecuteReaderAsync();
        }


        public async Task<int> AddBook(string tytul, string gatunek, string wydawca, string autor, string egzemplarze, string data)
        {
            await OpenConnection();

            //string values = "'"+tytul+"','"+gatunek+"','"+wydawca+"','"+autor+"','"+egzemplarze+"','"+data+"'";
            string values = "@tytul,@gatunek,@wydawca,@autor,@egzemplarze,@data";
            string query = "INSERT INTO ksiazki(Tytul,GatunekID,Wydawnictwo,Autor,Egzemplarze,Data_wydania)Values(" + values + ");";
            //string query = "INSERT INTO ksiazki(Tytul,GatunekID,Wydawnictwo,Autor,Egzemplarze,Data_wydania)Values("+values+");";
            command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@tytul", tytul);
            command.Parameters.AddWithValue("@gatunek", int.Parse(gatunek));
            command.Parameters.AddWithValue("@wydawca", wydawca);
            command.Parameters.AddWithValue("@autor", autor);
            command.Parameters.AddWithValue("@egzemplarze", int.Parse(egzemplarze));
            command.Parameters.AddWithValue("@data", data);
            return await command.ExecuteNonQueryAsync();

        }
        public async Task<int> AddClient(string name, string surname, string pesel, string email, string telefon)
        {
            await OpenConnection();

            string values = "@name,@surname,@pesel,@email,@telefon";
            string query = "INSERT INTO klienci(Imie,Nazwisko,Pesel,Email,Telefon)VALUES(" + values + ");";

            command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@surname", surname);
            command.Parameters.AddWithValue("@pesel", pesel);
            command.Parameters.AddWithValue("@email", email);
            command.Parameters.AddWithValue("@telefon", telefon);


            return await command.ExecuteNonQueryAsync();

        }


        public async Task<DbDataReader> GetBorrows(string ClientID)
        {
            await OpenConnection();
            string borrowSearch = "SELECT wypozyczenia.IDWypozyczenia, ksiazki.Tytul,wypozyczenia.Data_wypozyczenia,wypozyczenia.Czas_do_zwrotu,wypozyczenia.Data_zwrotu,ksiazki.IDKsiazki,ksiazki.Egzemplarze";
            string borrowSearch2 = " FROM wypozyczenia";
            string borrowSearch3 = " LEFT JOIN ksiazki ON wypozyczenia.IDKsiazki = ksiazki.IDKsiazki WHERE IDKlienta = @ClientID;";
            string search = borrowSearch + borrowSearch2 + borrowSearch3;

            command = new MySqlCommand(search, connection);
            command.Parameters.AddWithValue("@ClientID", ClientID);

            return await command.ExecuteReaderAsync();



        }
        public async Task<int> Borrow(string IDKlienta, string IDKsiazki, string data_wypozyczenia, string czas_do_zwrotu)
        {
            await OpenConnection();
            // (1,1,'2000-1-6','2000-1-7')
            string values = "@IDKlienta,@IDKsiazki,@data_wypozyczenia,@czas_do_zwrotu";
            string query = "INSERT INTO `wypozyczenia`(`IDKlienta`, `IDKsiazki`, `Data_wypozyczenia`, `Czas_do_zwrotu`) VALUES(" + values + ");";
            command = new MySqlCommand(query, connection);

            command.Parameters.AddWithValue("@IDKlienta", IDKlienta);
            command.Parameters.AddWithValue("@IDKsiazki", IDKsiazki);
            command.Parameters.AddWithValue("@data_wypozyczenia", data_wypozyczenia);
            command.Parameters.AddWithValue("@czas_do_zwrotu", czas_do_zwrotu);


            return await command.ExecuteNonQueryAsync();

        }

        public async Task<DbDataReader> GetCopies(string IDKsiazki)
        {
            await OpenConnection();
            string copiesSearch = "SELECT Egzemplarze FROM ksiazki WHERE IDKsiazki=@IDKsiazki;";
            command = new MySqlCommand(copiesSearch, connection);
            command.Parameters.AddWithValue("@IDKsiazki", IDKsiazki);

            return await command.ExecuteReaderAsync();



        }

        public async Task<int> ModifyBookCopy(string IDKsiazki, string setCopies)
        {
            await OpenConnection();
            string Query = "UPDATE `ksiazki` SET `Egzemplarze`=@setCopies WHERE IDKsiazki=@IDKsiazki;";
            command = new MySqlCommand(Query, connection);
            command.Parameters.AddWithValue("@IDKsiazki", IDKsiazki);
            command.Parameters.AddWithValue("@setCopies", setCopies);

            return await command.ExecuteNonQueryAsync();

        }
        public async Task<int> ModifyBorrow(string IDBorrow, string backdate)
        {
            await OpenConnection();
            string Query = "UPDATE `wypozyczenia` SET `Data_zwrotu`= @backdate WHERE IDWypozyczenia=@IDBorrow;";
            command = new MySqlCommand(Query, connection);

            command.Parameters.AddWithValue("@IDBorrow", IDBorrow);
            command.Parameters.AddWithValue("@backdate", backdate);

            return await command.ExecuteNonQueryAsync();
        }



        //~DBConnector()
        //{
        //    Console.WriteLine("ZAMKNIETO");
        //    MySqlConnection.ClearAllPools();
        //    command.Dispose();
        //    connection.Close();
        //    connection.Dispose();
        //}







        public void Dispose()
        {

            Console.WriteLine("CLOSE!");
            command.Dispose();
            connection.Close();
            connection.Dispose();


        }


    }
}
