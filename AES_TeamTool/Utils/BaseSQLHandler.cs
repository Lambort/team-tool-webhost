using System;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace AES_TeamTool.Utils
{
    public class BaseSQLHandler
    {
        public BaseSQLHandler(string connStr, bool isPooling = false)
        {
            IsPooling = isPooling;
            ConnectString = connStr;
            PrePoolString = IsPooling ? @"connection lifetime=5;min pool size = 1;max pool size=10;" : null;
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

        public bool InsertDataLine(string tableName, List<string> columnList, List<string> valueList)
        {
            try
            {
                string colString = columnList.Count > 1 ? string.Join(",", columnList.Select(item => $"[{item}]")) : $"[{columnList[0]}]";
                string valString = valueList.Count > 1 ? string.Join(",", valueList.Select(item => item == null ? "null" : $"'{item}'"))
                    : valueList[0] != null ? $"'{valueList[0]}'"
                    : "null";
                if (Connection.State == ConnectionState.Closed) { Connection.Open(); }
                string command = $@"INSERT INTO {tableName} ( {colString} ) values ( {valString} )";
                SqlCommand sqlCmd = new SqlCommand(command, Connection);
                sqlCmd.ExecuteNonQuery();
                sqlCmd.Dispose();
                return true;
            }
            catch (Exception err)
            {
                CommonTextLogger.WriteText(LogType.ERROR, err.Message);
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
                CommonTextLogger.WriteText(LogType.ERROR, err.Message);
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
                if (colList.Count == 0 || colList.Count != valList.Count) { return false; }
                string setValString = string.Empty;
                for (int i = 0; i < colList.Count; i++)
                {
                    if (colList[i] != null && colList[i] != string.Empty)
                    {
                        string _joinChar = i == 0 ? "" : ",";
                        string _valChar = (valList[i] == null || valList[i] == string.Empty) ? "NULL" : valList[i].ToString();
                        setValString += $"{_joinChar} {colList[i]}='{_valChar}'";
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
                CommonTextLogger.WriteText(LogType.ERROR, err.Message);
                return false;
            }
            finally
            {
                if (!IsPooling && Connection.State == ConnectionState.Open) { Connection.Close(); }
            }
        }

        public DataTable CommonQuery(string tableName, string selectColumns, string queryConditions)
        {
            DataTable queryResultData = new DataTable();
            try
            {
                string queryStr = $@"SELECT {selectColumns} FROM {tableName} WHERE {queryConditions}";
                if (Connection.State == ConnectionState.Closed) { Connection.Open(); }

                SqlCommand sqlCmd = new SqlCommand(queryStr, Connection);
                queryResultData.Load(sqlCmd.ExecuteReader());
                sqlCmd.Dispose();
                return queryResultData;
            }
            catch (Exception err)
            {
                CommonTextLogger.WriteText(LogType.ERROR, err.Message);
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
                CommonTextLogger.WriteText(LogType.ERROR, err.Message);
                return null;
            }
            finally
            {
                if (!IsPooling && Connection.State == ConnectionState.Open) { Connection.Close(); }
            }
        }
    }
}