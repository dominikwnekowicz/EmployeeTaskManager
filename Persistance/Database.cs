﻿using System;
using System.Collections.Generic;
using System.IO;
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
                case EMPLOYEE_TABLE_NAME:
                    if (!tableExist) dbConnection.CreateTable<Employee>();
                    data = dbConnection.Table<Employee>().ToList();
                    return data;
                case JOB_TABLE_NAME:
                    if (!tableExist) dbConnection.CreateTable<Job>();
                    data = dbConnection.Table<Job>().OrderBy(j => j.Date).ToList();
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
                case EMPLOYEE_TABLE_NAME:
                    if (!tableExist) dbConnection.CreateTable<Employee>();
                    foreach (var item in data as List<Employee>) dbConnection.Insert(item);
                    OnDataChanged(tableName);
                    return;
                case JOB_TABLE_NAME:
                    if (!tableExist) dbConnection.CreateTable<Job>();
                    foreach (var item in data as List<Job>) dbConnection.Insert(item);
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
                case EMPLOYEE_TABLE_NAME:
                    if (!tableExist) dbConnection.CreateTable<Employee>();
                    dbConnection.Insert((Employee)data);
                    OnDataChanged(tableName);
                    return;
                case JOB_TABLE_NAME:
                    if (!tableExist) dbConnection.CreateTable<Job>();
                    dbConnection.Insert((Job)data);
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
                case EMPLOYEE_TABLE_NAME:
                    if (!tableExist) dbConnection.CreateTable<Employee>();
                    foreach (var item in data as List<Employee>) dbConnection.Update(item);
                    OnDataChanged(tableName);
                    return;
                case JOB_TABLE_NAME:
                    if (!tableExist) dbConnection.CreateTable<Job>();
                    foreach (var item in data as List<Job>) dbConnection.Update(item);
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
                case EMPLOYEE_TABLE_NAME:
                    if (!tableExist) dbConnection.CreateTable<Employee>();
                    dbConnection.Update((Employee)data);
                    OnDataChanged(tableName);
                    return;
                case JOB_TABLE_NAME:
                    if (!tableExist) dbConnection.CreateTable<Job>();
                    dbConnection.Update((Job)data);
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
                case EMPLOYEE_TABLE_NAME:
                    if (!tableExist) dbConnection.CreateTable<Employee>();
                    foreach (var item in data as List<Employee>) dbConnection.Delete(item);
                    OnDataChanged(tableName);
                    return;
                case JOB_TABLE_NAME:
                    if (!tableExist) dbConnection.CreateTable<Job>();
                    foreach (var item in data as List<Job>) dbConnection.Delete(item);
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
                case EMPLOYEE_TABLE_NAME:
                    if (!tableExist) dbConnection.CreateTable<Employee>();
                    dbConnection.Delete((Employee)data);
                    OnDataChanged(tableName);
                    return;
                case JOB_TABLE_NAME:
                    if (!tableExist) dbConnection.CreateTable<Job>();
                    dbConnection.Delete((Job)data);
                    OnDataChanged(tableName);
                    return;
                default:
                    return;
            }
        }


    }
}