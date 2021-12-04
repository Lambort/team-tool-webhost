using System;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace AES_TeamTool.Utils
{
    public class SqlHelper
    {
        public SqlHelper(string connStr, bool isPooling = false)
        {
            IsPooling = isPooling;
            ConnectString = connStr;
            PrePoolString = IsPooling ? @"connection lifetime=15;min pool size=1;max pool size=25;" : null;
            PoolOptions = IsPooling ? $@"pooling=true;{PrePoolString}" : null;
            Connection = new SqlConnection(PoolOptions + ConnectString);
            if (IsPooling) { Connection.Open(); }
        }

        protected bool IsPooling { get; }
        protected string PrePoolString { get; }
        protected string PoolOptions { get; }
        protected string ConnectString { get; }
        protected SqlConnection Connection { get; set; }

        public void ReInitConnectin()
        {
            Connection = null;
            Connection = new SqlConnection(PoolOptions + ConnectString);
        }

        public void CloseConnection()
        {
            if (Connection.State == ConnectionState.Closed) return;
            Connection.Close();
        }

        public void DisposeConnection()
        {
            CloseConnection();
            Connection.Dispose();
        }

        public bool InsertDataLine(string tableName, List<string> columnList, List<string> valueList)
        {
            try
            {
                string colString = string.Empty;
                string valString = string.Empty;
                columnList.ForEach(col => colString += col == columnList.Last() ? $"[{col}]" : $"[{col}],");
                //warning here, if follow linq, value list must be mapped
                //valueList.ForEach(val => valString += val == valueList.Last() ? $"'{val}'" : $"'{val}',");
                for(int i=0; i < valueList.Count; i++)
                {
                    valString += (i == valueList.Count - 1) ? $"'{valueList[i]}'" : $"'{valueList[i]}',";
                }
                if (Connection.State == ConnectionState.Closed) { Connection.Open(); }
                string command = $@"INSERT INTO {tableName} ( {colString} ) values ( {valString} )";
                SqlCommand sqlCmd = new SqlCommand(command, Connection);
                sqlCmd.ExecuteNonQuery();
                sqlCmd.Dispose();
                return true;
            }
            catch (Exception err)
            {
                LogHelper.WriteText(LogType.ERROR, err.Message);
                return false;
            }
            finally
            {
                if (!IsPooling && Connection.State == ConnectionState.Open) { Connection.Close(); }
            }
        }

        public bool InsertDataLine(string command)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(command)) { throw new Exception("No SQL command to be excute"); }
                if (Connection.State == ConnectionState.Closed) { Connection.Open(); }
                SqlCommand sqlCmd = new SqlCommand(command, Connection);
                sqlCmd.ExecuteNonQuery();
                sqlCmd.Dispose();
                return true;
            }
            catch (Exception err)
            {
                LogHelper.WriteText(LogType.ERROR, err.Message);
                return false;
            }
            finally
            {
                if (!IsPooling && Connection.State == ConnectionState.Open) { Connection.Close(); }
            }
        }

        public bool InsertDataTable(string tableName, DataTable dataTable, IDbTransaction transaction = null)
        {
            SqlBulkCopy sqlBulkCopy = transaction == null ?
                new SqlBulkCopy(Connection) :
                new SqlBulkCopy(Connection, SqlBulkCopyOptions.KeepNulls, (SqlTransaction)transaction);
            sqlBulkCopy.BulkCopyTimeout = 15000;
            try
            {
                if (Connection.State == ConnectionState.Closed) { Connection.Open(); }
                sqlBulkCopy.DestinationTableName = tableName;
                sqlBulkCopy.NotifyAfter = dataTable.Rows.Count;
                sqlBulkCopy.WriteToServer(dataTable);
                return true;
            }
            catch (Exception err)
            {
                LogHelper.WriteText(LogType.ERROR, err.Message);
                return false;
            }
            finally
            {
                sqlBulkCopy.Close();
                if (transaction == null && !IsPooling && Connection.State == ConnectionState.Open) { Connection.Close(); }
            }
        }

        public bool UpdateDataLine(string tableName, List<string> colList, List<string> valList, string findCondition)
        {
            try
            {
                if (colList.Count == 0 || colList.Count != valList.Count || string.IsNullOrWhiteSpace(findCondition))
                {
                    throw new Exception("Cannot update data cause, data or condition required!");
                }
                string setValString = string.Empty;
                for (int i = 0; i < colList.Count; i++)
                {
                    if (string.IsNullOrWhiteSpace(colList[i]))
                    {
                        string joinChar = i == 0 ? "" : ",";
                        string valChar = string.IsNullOrWhiteSpace(valList[i]) ? "NULL" : valList[i];
                        setValString += $"{joinChar} {colList[i]}='{valChar}'";
                    }
                }
                if (Connection.State == ConnectionState.Closed) { Connection.Open(); }
                string command = $@"UPDATE {tableName} SET {setValString} WHERE {findCondition}";
                SqlCommand sqlCmd = new SqlCommand(command, Connection);
                sqlCmd.ExecuteNonQuery();
                sqlCmd.Dispose();
                return true;
            }
            catch (Exception err)
            {
                LogHelper.WriteText(LogType.ERROR, err.Message);
                return false;
            }
            finally
            {
                if (!IsPooling && Connection.State == ConnectionState.Open) { Connection.Close(); }
            }
        }

        public DataTable CommonQuery(string tableName, string selectColumns, string queryConditions = null, string queryOrder = null)
        {
            DataTable queryResultData = new DataTable();
            try
            {
                string columns = string.IsNullOrWhiteSpace(selectColumns) ? "*" : selectColumns;
                string condition = string.IsNullOrWhiteSpace(queryConditions) ? "" : $"WHERE {queryConditions}";
                string order = string.IsNullOrWhiteSpace(queryOrder) ? "" : $"order by {queryOrder}";
                string queryStr = $@"SELECT {columns} FROM {tableName} {condition} {order}";
                if (Connection.State == ConnectionState.Closed) { Connection.Open(); }
                SqlCommand sqlCmd = new SqlCommand(queryStr, Connection);
                queryResultData.Load(sqlCmd.ExecuteReader());
                sqlCmd.Dispose();
                return queryResultData;
            }
            catch (Exception err)
            {
                LogHelper.WriteText(LogType.ERROR, err.Message);
                return null;
            }
            finally
            {
                if (!IsPooling && Connection.State == ConnectionState.Open) { Connection.Close(); }
            }
        }

        public DataTable CommonQuery(string tableName, List<string> selectColumns, string queryConditions = null, string queryOrder = null)
        {
            DataTable queryResultData = new DataTable();
            try
            {
                string columns = string.Empty;
                selectColumns.ForEach(col => { columns += col == selectColumns.Last() ? $"[{col}]" : $"[{col}],"; });
                columns = string.IsNullOrWhiteSpace(columns) ? "*" : columns;
                string condition = string.IsNullOrWhiteSpace(queryConditions) ? "" : $"WHERE {queryConditions}";
                string order = string.IsNullOrWhiteSpace(queryOrder) ? "" : $"order by {queryOrder}";
                string queryStr = $@"SELECT {columns} FROM {tableName} {condition} {order}";
                if (Connection.State == ConnectionState.Closed) { Connection.Open(); }
                SqlCommand sqlCmd = new SqlCommand(queryStr, Connection);
                queryResultData.Load(sqlCmd.ExecuteReader());
                sqlCmd.Dispose();
                return queryResultData;
            }
            catch (Exception err)
            {
                LogHelper.WriteText(LogType.ERROR, err.Message);
                return null;
            }
            finally
            {
                if (!IsPooling && Connection.State == ConnectionState.Open) { Connection.Close(); }
            }
        }

        public DataTable CommonQuery(string excQuery)
        {
            DataTable queryResultData = new DataTable();
            try
            {
                if (string.IsNullOrWhiteSpace(excQuery)) { throw new Exception("query string connot be null"); }
                if (Connection.State == ConnectionState.Closed) { Connection.Open(); }
                SqlCommand sqlCmd = new SqlCommand(excQuery, Connection);
                queryResultData.Load(sqlCmd.ExecuteReader());
                sqlCmd.Dispose();
                return queryResultData;
            }
            catch (Exception err)
            {
                LogHelper.WriteText(LogType.ERROR, err.Message);
                return null;
            }
            finally
            {
                if (!IsPooling && Connection.State == ConnectionState.Open) { Connection.Close(); }
            }
        }
    }
}