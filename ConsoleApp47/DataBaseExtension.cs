using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp47
{
    public static class DataBaseExtension
    {
        public static List<Singer> GetSingers(this DataBase database)
        {
            return MysqlTools.SimpleSelectFromTable<Singer>();
        }

        public static List<Music> GetMusic(this DataBase database)
        {
            return MysqlTools.SimpleSelectFromTable<Music>();
        }

        public static Singer GetSingerByID(this DataBase database, int id)
        {
            return MysqlTools.SelectRowFromTable<Singer>(id);
        }
    }
}
