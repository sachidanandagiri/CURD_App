using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace CURD_App.Models
{
    public class SQLDataAccess : IDisposable
    {
        private int _commandTimeout;
        private string _connectionString;
        private SqlTransaction _sqlTransaction;
        private SqlConnection _sqlCon;

        public SQLDataAccess()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["CS"].ConnectionString;
            _commandTimeout = 1000 * 60 * 60 * 20;
        }

        private void OpenConnection()
        {
            if (_sqlCon == null)
                _sqlCon = new SqlConnection(_connectionString);

            if (_sqlCon.State == ConnectionState.Closed)
                _sqlCon.Open();
        }

        private void CloseConnection()
        {
            if (_sqlTransaction == null)
                _sqlCon.Close();
        }

        public SqlParameterCollection OutputParameters;
        public List<SqlParameter> Parameters = null;

        public void BeginTrans()
        {
            OpenConnection();
            _sqlTransaction = _sqlCon.BeginTransaction();
        }

        public void Commit()
        {
            _sqlTransaction.Commit();
            _sqlTransaction = null;
            CloseConnection();
        }

        public void Rollback()
        {
            _sqlTransaction.Rollback();
        }

        public int CommandTimeout
        {
            get { return _commandTimeout; }
            set { _commandTimeout = value; }
        }

        public void FillData(DataSet dataSet, string commandName, CommandType commandType = CommandType.StoredProcedure)
        {
            OpenConnection();

            using (SqlCommand cm = _sqlCon.CreateCommand())
            {
                if (cm.CommandTimeout != 0)
                    cm.CommandTimeout = CommandTimeout;

                cm.CommandText = commandName;
                cm.CommandType = commandType;

                if (Parameters != null)
                    cm.Parameters.AddRange(Parameters.ToArray());

                SqlDataAdapter adp = new SqlDataAdapter(cm);

                try
                {
                    adp.Fill(dataSet);
                    OutputParameters = cm.Parameters;
                }
                catch (SqlException ex)
                {
                    //ExceptionLog.Write(ex, cm);
                    throw ex;
                }
                finally
                {
                    CloseConnection();
                }
            }
        }

        public SqlDataReader ExecDataReader(string commandName, CommandType commandType = CommandType.StoredProcedure)
        {
            OpenConnection();
            using (SqlCommand sqlCmd = new SqlCommand(commandName, _sqlCon))
            {
                try
                {
                    if (_sqlTransaction != null)
                        sqlCmd.Transaction = _sqlTransaction;

                    if (sqlCmd.CommandTimeout != 0)
                        sqlCmd.CommandTimeout = CommandTimeout;

                    sqlCmd.CommandType = commandType;

                    if (Parameters != null)
                    {
                        sqlCmd.Parameters.AddRange(Parameters.ToArray());
                    }
                    return sqlCmd.ExecuteReader();
                }
                catch (SqlException ex)
                {
                    //ExceptionLog.Write(ex, sqlCmd);
                    throw ex;
                }
            }
        }

        public object ExecuteScalar(string commandName, CommandType commandType = CommandType.StoredProcedure)
        {
            OpenConnection();
            using (SqlCommand sqlCmd = new SqlCommand(commandName, _sqlCon))
            {
                try
                {
                    if (_sqlTransaction != null)
                        sqlCmd.Transaction = _sqlTransaction;

                    if (sqlCmd.CommandTimeout != 0)
                        sqlCmd.CommandTimeout = CommandTimeout;

                    sqlCmd.CommandType = commandType;

                    if (Parameters != null)
                    {
                        sqlCmd.Parameters.AddRange(Parameters.ToArray());
                    }

                    object obj = sqlCmd.ExecuteScalar();
                    OutputParameters = sqlCmd.Parameters;
                    return obj;

                }
                catch (SqlException ex)
                {
                    //ExceptionLog.Write(ex, sqlCmd);
                    throw ex;
                }
                finally
                {
                    CloseConnection();
                }
            }
        }

        public int ExecCommand(string commandName, CommandType commandType = CommandType.StoredProcedure)
        {
            int effectedRows = 0;
            OpenConnection();
            using (SqlCommand sqlCmd = new SqlCommand(commandName, _sqlCon))
            {
                try
                {
                    sqlCmd.CommandType = commandType;

                    if (_sqlTransaction != null)
                        sqlCmd.Transaction = _sqlTransaction;

                    if (sqlCmd.CommandTimeout != 0)
                        sqlCmd.CommandTimeout = CommandTimeout;

                    if (Parameters != null)
                        sqlCmd.Parameters.AddRange(Parameters.ToArray());

                    effectedRows = sqlCmd.ExecuteNonQuery();

                    //output values
                    OutputParameters = sqlCmd.Parameters;
                }
                catch (SqlException ex)
                {
                    //ExceptionLog.Write(ex, sqlCmd);
                    throw ex;
                }
                finally
                {
                    CloseConnection();
                }
                return effectedRows;
            }
        }

        public void Dispose()
        {
            if (_sqlTransaction != null)
            {
                _sqlTransaction.Dispose();
                _sqlTransaction = null;
            }

            if (_sqlCon != null)
            {
                if (_sqlCon.State != ConnectionState.Closed)
                    _sqlCon.Close();

                _sqlCon.Dispose();
                _sqlCon = null;
            }

            GC.SuppressFinalize(this);
        }

    }
}