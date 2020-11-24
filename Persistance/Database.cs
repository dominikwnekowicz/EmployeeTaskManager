using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using FakroApp.Model;
using SQLite;
using static FakroApp.Persistance.Constants;

namespace FakroApp.Persistance
{
    public class Database
    {
        public class DataChangedEventArgs
        {
            public string ChangedTableName { get; set; }
        };

        public event EventHandler<DataChangedEventArgs> DataChanged;

        protected virtual void OnDataChanged(string tableName)
        {
            DataChanged?.Invoke(this, new DataChangedEventArgs() { ChangedTableName = tableName });
        }

        private static SQLiteConnection ConnectWithDb()
        {
            string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            string path = Path.Combine(documentsPath, "FakroDatabase.db3");
            //System.IO.File.Delete(path);
            const SQLiteOpenFlags flags = SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create;
            SQLiteConnection dbConnection = new SQLiteConnection(path, flags);
            return dbConnection;
        }

        private static bool TableExist(SQLiteConnection db, string tableName)
        {
            try
            {
                if (db.GetTableInfo(tableName).Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<dynamic> GetItems(Activity activity, string tableName)
        {
            await Permissions.CheckPermissions(activity);
            var dbConnection = ConnectWithDb();
            var tableExist = TableExist(dbConnection, tableName);
            dynamic data;

            switch (tableName)
            {
                case DAYOFF_TABLE_NAME:
                    if (!tableExist) dbConnection.CreateTable<DayOff>();
                    data = dbConnection.Table<DayOff>().OrderBy(d => d.From).ThenBy(d => d.Id).ToList();
                    return data;
                case JOB_TABLE_NAME:
                    if (!tableExist) dbConnection.CreateTable<Job>();
                    data = dbConnection.Table<Job>().OrderBy(j => j.Date).ToList();
                    return data;
                case WORK_TABLE_NAME:
                    if (!tableExist) dbConnection.CreateTable<Work>();
                    data = dbConnection.Table<Work>().OrderBy(j => j.Name).ToList();
                    return data;
                default:
                    return null;
            }
        }

        public async Task PutItems(Activity activity, dynamic data, string tableName)
        {
            await Permissions.CheckPermissions(activity);
            var dbConnection = ConnectWithDb();
            var tableExist = TableExist(dbConnection, tableName);

            switch (tableName)
            {
                case DAYOFF_TABLE_NAME:
                    if (!tableExist) dbConnection.CreateTable<DayOff>();
                    foreach (var item in data as List<DayOff>) dbConnection.Insert(item);
                    OnDataChanged(tableName);
                    return;
                case JOB_TABLE_NAME:
                    if (!tableExist) dbConnection.CreateTable<Job>();
                    foreach (var item in data as List<Job>) dbConnection.Insert(item);
                    OnDataChanged(tableName);
                    return;
                case WORK_TABLE_NAME:
                    if (!tableExist) dbConnection.CreateTable<Work>();
                    foreach (var item in data as List<Work>) dbConnection.Insert(item);
                    OnDataChanged(tableName);
                    return;
                default:
                    return;
            }
        }

        public async Task PutItem(Activity activity, dynamic data, string tableName)
        {
            await Permissions.CheckPermissions(activity);
            var dbConnection = ConnectWithDb();
            var tableExist = TableExist(dbConnection, tableName);

            switch (tableName)
            {
                case DAYOFF_TABLE_NAME:
                    if (!tableExist) dbConnection.CreateTable<DayOff>();
                    dbConnection.Insert((DayOff)data);
                    OnDataChanged(tableName);
                    return;
                case JOB_TABLE_NAME:
                    if (!tableExist) dbConnection.CreateTable<Job>();
                    dbConnection.Insert((Job)data);
                    OnDataChanged(tableName);
                    return;
                case WORK_TABLE_NAME:
                    if (!tableExist) dbConnection.CreateTable<Work>();
                    dbConnection.Insert((Work)data);
                    OnDataChanged(tableName);
                    return;
                default:
                    return;
            }
        }

        public async Task UpdateItems(Activity activity, dynamic data, string tableName)
        {
            await Permissions.CheckPermissions(activity);
            var dbConnection = ConnectWithDb();
            var tableExist = TableExist(dbConnection, tableName);

            switch (tableName)
            {
                case DAYOFF_TABLE_NAME:
                    if (!tableExist) dbConnection.CreateTable<DayOff>();
                    foreach (var item in data as List<DayOff>) dbConnection.Update(item);
                    OnDataChanged(tableName);
                    return;
                case JOB_TABLE_NAME:
                    if (!tableExist) dbConnection.CreateTable<Job>();
                    foreach (var item in data as List<Job>) dbConnection.Update(item);
                    OnDataChanged(tableName);
                    return;
                case WORK_TABLE_NAME:
                    if (!tableExist) dbConnection.CreateTable<Work>();
                    foreach (var item in data as List<Work>) dbConnection.Update(item);
                    OnDataChanged(tableName);
                    return;
                default:
                    return;
            }
        }

        public async Task UpdateItem(Activity activity, dynamic data, string tableName)
        {
            await Permissions.CheckPermissions(activity);
            var dbConnection = ConnectWithDb();
            var tableExist = TableExist(dbConnection, tableName);

            switch (tableName)
            {
                case DAYOFF_TABLE_NAME:
                    if (!tableExist) dbConnection.CreateTable<DayOff>();
                    dbConnection.Update((DayOff)data);
                    OnDataChanged(tableName);
                    return;
                case JOB_TABLE_NAME:
                    if (!tableExist) dbConnection.CreateTable<Job>();
                    dbConnection.Update((Job)data);
                    OnDataChanged(tableName);
                    return;
                case WORK_TABLE_NAME:
                    if (!tableExist) dbConnection.CreateTable<Work>();
                    dbConnection.Update((Work)data);
                    OnDataChanged(tableName);
                    return;
                default:
                    return;
            }
        }

        public async Task DeleteItems(Activity activity, dynamic data, string tableName)
        {
            await Permissions.CheckPermissions(activity);
            var dbConnection = ConnectWithDb();
            var tableExist = TableExist(dbConnection, tableName);

            switch (tableName)
            {
                case DAYOFF_TABLE_NAME:
                    if (!tableExist) dbConnection.CreateTable<DayOff>();
                    foreach (var item in data as List<DayOff>) dbConnection.Delete(item);
                    OnDataChanged(tableName);
                    return;
                case JOB_TABLE_NAME:
                    if (!tableExist) dbConnection.CreateTable<Job>();
                    foreach (var item in data as List<Job>) dbConnection.Delete(item);
                    OnDataChanged(tableName);
                    return;
                case WORK_TABLE_NAME:
                    if (!tableExist) dbConnection.CreateTable<Work>();
                    foreach (var item in data as List<Work>) dbConnection.Delete(item);
                    OnDataChanged(tableName);
                    return;
                default:
                    return;
            }
        }
        public async Task DeleteItem(Activity activity, dynamic data, string tableName)
        {
            await Permissions.CheckPermissions(activity);
            var dbConnection = ConnectWithDb();
            var tableExist = TableExist(dbConnection, tableName);

            switch (tableName)
            {
                case DAYOFF_TABLE_NAME:
                    if (!tableExist) dbConnection.CreateTable<DayOff>();
                    dbConnection.Delete((DayOff)data);
                    OnDataChanged(tableName);
                    return;
                case JOB_TABLE_NAME:
                    if (!tableExist) dbConnection.CreateTable<Job>();
                    dbConnection.Delete((Job)data);
                    OnDataChanged(tableName);
                    return;
                case WORK_TABLE_NAME:
                    if (!tableExist) dbConnection.CreateTable<Work>();
                    dbConnection.Delete((Work)data);
                    OnDataChanged(tableName);
                    return;
                default:
                    return;
            }
        }


    }

    public static class DataManager
    {
        public static double GetDailyMinutes(Activity activity)
        {
            Database database = new Database();
            var works = (List<Work>)database.GetItems(activity, WORK_TABLE_NAME).Result;
            var jobs = (List<Job>)database.GetItems(activity, JOB_TABLE_NAME).Result;
            var dailyJobs = jobs.Where(j => DateTime.Today.Date == j.Date.Date && j.Type == CURRENT_JOB_TYPE);
            double dailyMinutes = 0;
            if (dailyJobs.Any())
            {
                foreach (var job in dailyJobs)
                {
                    if (job.IsNormalized) dailyMinutes += works.FirstOrDefault(w => w.Id == job.WorkId).Norm * job.Quantity;
                    else dailyMinutes += Convert.ToDouble(job.Time, CultureInfo.InvariantCulture);
                }
            }

            return dailyMinutes;
        }
    }
}