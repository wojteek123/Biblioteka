using Biblioteka.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteka.ViewModels
{
    public class DbChoose
    {
        private bool UseSQLite = true;

        public IDBConnector GetNewDBConnector()
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
