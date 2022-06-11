using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using XF.APP.ABSTRACTION;

namespace XF.APP.ApiService.Service
{
    public class AppSQLiteManager : ISQLiteManager
    {
        public static bool IsConnected { get; internal set; }
        public Task<string> GetDatabasePath()
        {
            string folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
            var path = Path.Combine(folder, ABSTRACTION.Constants.DbName);
            return Task.FromResult(path);
        }
        public SQLiteConnection GetSqlLiteConnection()
        {
            string path = GetDatabasePath().Result;
            SQLiteConnection connection = null;
            try
            {
                connection = new SQLiteConnection(path);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return connection;
        }
    }
}
