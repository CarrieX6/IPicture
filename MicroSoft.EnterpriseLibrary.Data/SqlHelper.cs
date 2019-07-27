//*********************************************************************
// Microsoft Data Access Application Block for .NET
// http://msdn.microsoft.com/library/en-us/dnbda/html/daab-rm.asp
//
// SQLHelper.cs
//
// This file contains the implementations of the SqlHelper and SqlHelperParameterCache
// classes.
//
// For more information see the Data Access Application Block Implementation Overview. 
// 
//*********************************************************************
// Copyright (C) 2000-2001 Microsoft Corporation
// All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
// FITNESS FOR A PARTICULAR PURPOSE.
//*********************************************************************

using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Web;

namespace MicroSoft.EnterpriseLibrary.Data
{
    //*********************************************************************
    //
    // The SqlHelper class is intended to encapsulate high performance, scalable best practices for 
    // common uses of SqlClient.
    //
    //*********************************************************************

    public sealed class SqlHelper
    {
        //*********************************************************************
        //
        // Since this class provides only static methods, make the default constructor private to prevent 
        // instances from being created with "new SqlHelper()".
        //
        //*********************************************************************

        private SqlHelper() {}

        //*********************************************************************
        //
        // This method is used to attach array of SqlParameters to a SqlCommand.
        // 
        // This method will assign a value of DbNull to any parameter with a direction of
        // InputOutput and a value of null.  
        // 
        // This behavior will prevent default values from being used, but
        // this will be the less common case than an intended pure output parameter (derived as InputOutput)
        // where the user provided no input value.
        // 
        // param name="command" The command to which the parameters will be added
        // param name="commandParameters" an array of SqlParameters tho be added to command
        //
        //*********************************************************************

        private static void AttachParameters(SqlCommand command, SqlParameter[] commandParameters)
        {
            foreach (SqlParameter p in commandParameters)
            {
                //check for derived output value with no value assigned
                if ((p.Direction == ParameterDirection.InputOutput) && (p.Value == null))
                {
                    p.Value = DBNull.Value;
                }
                //没有赋值的输入参数,赋上DBNULL
                if ((p.Direction == ParameterDirection.Input) && (p.Value == null))
                {
                    p.Value = DBNull.Value;
                }
                    
                command.Parameters.Add(p);
            }
        }
        //*********************************************************************
        //
        // This method assigns an array of values to an array of SqlParameters.
        // 
        // param name="commandParameters" array of SqlParameters to be assigned values
        // param name="parameterValues" array of objects holding the values to be assigned
        //
        //*********************************************************************

        private static void AssignParameterValues(SqlParameter[] commandParameters, object[] parameterValues)
        {
            if ((commandParameters == null) || (parameterValues == null)) 
            {
                //do nothing if we get no data
                return;
            }

            // we must have the same number of values as we pave parameters to put them in
            if (commandParameters.Length != parameterValues.Length)
            {
                throw new ArgumentException("Parameter count does not match Parameter Value count.");
            }

            //iterate through the SqlParameters, assigning the values from the corresponding position in the 
            //value array
            for (int i = 0, j = commandParameters.Length; i < j; i++)
            {
                commandParameters[i].Value = parameterValues[i];
            }
        }
        /// <summary>
        /// 将值数组与参数数据匹配
        /// </summary>
        /// <param name="commandParameters"></param>
        /// <param name="paras"></param>
        private static void AssignParameterValues(SqlParameter[] commandParameters, string[] paras)
        {
            if ((commandParameters == null) || (paras == null)) 
            {
                //do nothing if we get no data
                return;
            }

            // we must have the same number of values as we pave parameters to put them in
            if (commandParameters.Length != paras.Length)
            {
                throw new ArgumentException("Parameter count does not match Parameter Value count.");
            }

            //iterate through the SqlParameters, assigning the values from the corresponding position in the 
            //value array
            for (int i = 0, j = commandParameters.Length; i < j; i++)
            {
                commandParameters[i].Value = paras[i];
            }
        }

        //*********************************************************************
        //
        // This method opens (if necessary) and assigns a connection, transaction, command type and parameters 
        // to the provided command.
        // 
        // param name="command" the SqlCommand to be prepared
        // param name="connection" a valid SqlConnection, on which to execute this command
        // param name="transaction" a valid SqlTransaction, or 'null'
        // param name="commandType" the CommandType (stored procedure, text, etc.)
        // param name="commandText" the stored procedure name or T-SQL command
        // param name="commandParameters" an array of SqlParameters to be associated with the command or 'null' if no parameters are required
        //
        //*********************************************************************

        public static void PrepareCommand(SqlCommand command, SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, SqlParameter[] commandParameters)
        {
            //if the provided connection is not open, we will open it
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            //associate the connection with the command
            command.Connection = connection;

            // qianwt 2007-8-20 设置执行超时时间跟连接超时时间一致
            command.CommandTimeout = connection.ConnectionTimeout;

            //set the command text (stored procedure name or SQL statement)
            command.CommandText = commandText;

            //if we were provided a transaction, assign it.
            if (transaction != null)
            {
                command.Transaction = transaction;
            }

            //set the command type
            command.CommandType = commandType;

            //attach the command parameters if they are provided
            if (commandParameters != null)
            {
                AttachParameters(command, commandParameters);
            }

            return;
        }

        //*********************************************************************
        //
        // Execute a SqlCommand (that returns no resultset) against the database specified in the connection string 
        // using the provided parameters.
        //
        // e.g.:  
        //  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        //
        // param name="connectionString" a valid connection string for a SqlConnection
        // param name="commandType" the CommandType (stored procedure, text, etc.)
        // param name="commandText" the stored procedure name or T-SQL command
        // param name="commandParameters" an array of SqlParamters used to execute the command
        // returns an int representing the number of rows affected by the command
        //
        //*********************************************************************

        public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            //create & open a SqlConnection, and dispose of it after we are done.
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();

                //call the overload that takes a connection in place of the connection string
                return ExecuteNonQuery(cn, commandType, commandText, commandParameters);
            }
        }

        //*********************************************************************
        //
        // Execute a stored procedure via a SqlCommand (that returns no resultset) against the database specified in 
        // the connection string using the provided parameter values.  This method will query the database to discover the parameters for the 
        // stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        // 
        // This method provides no access to output parameters or the stored procedure's return value parameter.
        // 
        // e.g.:  
        //  int result = ExecuteNonQuery(connString, "PublishOrders", 24, 36);
        //
        // param name="connectionString" a valid connection string for a SqlConnection
        // param name="spName" the name of the stored prcedure
        // param name="parameterValues" an array of objects to be assigned as the input values of the stored procedure
        // returns an int representing the number of rows affected by the command
        //
        //*********************************************************************

        public static int ExecuteNonQuery(string connectionString, string spName, params object[] parameterValues)
        {
            //if we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0)) 
            {
                //pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

                //assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                //call the overload that takes an array of SqlParameters
                return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
                //otherwise we can just call the SP without params
            else 
            {
                return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName);
            }
        }
        /// <summary>
        /// 传入值数组时执行
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="spName"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string connectionString, string spName, string[] paras)
        {
            //if we receive parameter values, we need to figure out where they go
            if ((paras != null) && (paras.Length > 0)) 
            {
                //pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

                //assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, paras);

                //call the overload that takes an array of SqlParameters
                return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
                //otherwise we can just call the SP without params
            else 
            {
                return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName);
            }
        }
        //*********************************************************************
        //
        // Execute a SqlCommand (that returns no resultset) against the specified SqlConnection 
        // using the provided parameters.
        // 
        // e.g.:  
        //  int result = ExecuteNonQuery(conn, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        // 
        // param name="connection" a valid SqlConnection 
        // param name="commandType" the CommandType (stored procedure, text, etc.) 
        // param name="commandText" the stored procedure name or T-SQL command 
        // param name="commandParameters" an array of SqlParamters used to execute the command 
        // returns an int representing the number of rows affected by the command
        //
        //*********************************************************************

        public static int ExecuteNonQuery(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {    
            //create a command and prepare it for execution
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, connection, (SqlTransaction)null, commandType, commandText, commandParameters);
            
            //finally, execute the command.
            try
            {
                int retval = cmd.ExecuteNonQuery();
                return retval;
//            cmd.ExecuteNonQuery();
//            return 0;
            }
//            catch(Exception e)
//            {
//                HttpContext.Current.Response.Write(e.Message);
//                return 0;
//            }
            finally
            {
                // detach the SqlParameters from the command object, so they can be used again.            
                cmd.Parameters.Clear();
                //关闭连接
                if(connection.State==ConnectionState.Open)connection.Close();
            }    
        }

        //*********************************************************************
        //
        // Execute a SqlCommand (that returns a resultset) against the database specified in the connection string 
        // using the provided parameters.
        // 
        // e.g.:  
        //  DataSet ds = ExecuteDataset(connString, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24));
        // 
        // param name="connectionString" a valid connection string for a SqlConnection 
        // param name="commandType" the CommandType (stored procedure, text, etc.) 
        // param name="commandText" the stored procedure name or T-SQL command 
        // param name="commandParameters" an array of SqlParamters used to execute the command 
        // returns a dataset containing the resultset generated by the command
        //
        //*********************************************************************

        public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            //create & open a SqlConnection, and dispose of it after we are done.
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();

                //call the overload that takes a connection in place of the connection string
                return ExecuteDataset(cn, commandType, commandText, commandParameters);
            }
        }

        //*********************************************************************
        //
        // Execute a stored procedure via a SqlCommand (that returns a resultset) against the database specified in 
        // the connection string using the provided parameter values.  This method will query the database to discover the parameters for the 
        // stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        // 
        // This method provides no access to output parameters or the stored procedure's return value parameter.
        // 
        // e.g.:  
        //  DataSet ds = ExecuteDataset(connString, "GetOrders", 24, 36);
        // 
        // param name="connectionString" a valid connection string for a SqlConnection
        // param name="spName" the name of the stored procedure
        // param name="parameterValues" an array of objects to be assigned as the input values of the stored procedure
        // returns a dataset containing the resultset generated by the command
        //
        //*********************************************************************

        public static DataSet ExecuteDataset(string connectionString, string spName, params object[] parameterValues)
        {
            //if we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0)) 
            {
                //pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

                //assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                //call the overload that takes an array of SqlParameters
                return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
                //otherwise we can just call the SP without params
            else 
            {
                return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName);
            }
        }
        
        //*********************************************************************
        //
        // Execute a SqlCommand (that returns a resultset) against the specified SqlConnection 
        // using the provided parameters.
        // 
        // e.g.:  
        //  DataSet ds = ExecuteDataset(conn, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24));
        //
        // param name="connection" a valid SqlConnection
        // param name="commandType" the CommandType (stored procedure, text, etc.)
        // param name="commandText" the stored procedure name or T-SQL command
        // param name="commandParameters" an array of SqlParamters used to execute the command
        // returns a dataset containing the resultset generated by the command
        //
        //*********************************************************************

        public static DataSet ExecuteDataset(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            //create a command and prepare it for execution
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, connection, (SqlTransaction)null, commandType, commandText, commandParameters);
            
            //create the DataAdapter & DataSet
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();

            //fill the DataSet using default values for DataTable names, etc.
            try
            {
                da.Fill(ds);
                return ds;
            }
//            catch(Exception e)
//            {
//                HttpContext.Current.Response.Write(e.Message);
//                return null;
//            }
            finally
            {
                // detach the SqlParameters from the command object, so they can be used again.            
                cmd.Parameters.Clear();
                //关闭连接
                if(connection.State == ConnectionState.Open) connection.Close();
            }
                                    
        }

        //*********************************************************************
        //
        // Execute a SqlCommand (that returns a 1x1 resultset) against the database specified in the connection string 
        // using the provided parameters.
        // 
        // e.g.:  
        //  int orderCount = (int)ExecuteScalar(connString, CommandType.StoredProcedure, "GetOrderCount", new SqlParameter("@prodid", 24));
        // 
        // param name="connectionString" a valid connection string for a SqlConnection 
        // param name="commandType" the CommandType (stored procedure, text, etc.) 
        // param name="commandText" the stored procedure name or T-SQL command 
        // param name="commandParameters" an array of SqlParamters used to execute the command 
        // returns an object containing the value in the 1x1 resultset generated by the command
        //
        //*********************************************************************

        public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            //create & open a SqlConnection, and dispose of it after we are done.
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();

                //call the overload that takes a connection in place of the connection string
                return ExecuteScalar(cn, commandType, commandText, commandParameters);
            }
        }

        //*********************************************************************
        //
        // Execute a stored procedure via a SqlCommand (that returns a 1x1 resultset) against the database specified in 
        // the connection string using the provided parameter values.  This method will query the database to discover the parameters for the 
        // stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        // 
        // This method provides no access to output parameters or the stored procedure's return value parameter.
        // 
        // e.g.:  
        //  int orderCount = (int)ExecuteScalar(connString, "GetOrderCount", 24, 36);
        // 
        // param name="connectionString" a valid connection string for a SqlConnection 
        // param name="spName" the name of the stored procedure 
        // param name="parameterValues" an array of objects to be assigned as the input values of the stored procedure 
        // returns an object containing the value in the 1x1 resultset generated by the command
        //
        //*********************************************************************

        public static object ExecuteScalar(string connectionString, string spName, params object[] parameterValues)
        {
            //if we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0)) 
            {
                //pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

                //assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                //call the overload that takes an array of SqlParameters
                return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
                //otherwise we can just call the SP without params
            else 
            {
                return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName);
            }
        }

        //*********************************************************************
        //
        // Execute a SqlCommand (that returns a 1x1 resultset) against the specified SqlConnection 
        // using the provided parameters.
        // 
        // e.g.:  
        //  int orderCount = (int)ExecuteScalar(conn, CommandType.StoredProcedure, "GetOrderCount", new SqlParameter("@prodid", 24));
        // 
        // param name="connection" a valid SqlConnection 
        // param name="commandType" the CommandType (stored procedure, text, etc.) 
        // param name="commandText" the stored procedure name or T-SQL command 
        // param name="commandParameters" an array of SqlParamters used to execute the command 
        // returns an object containing the value in the 1x1 resultset generated by the command
        //
        //*********************************************************************

        public static object ExecuteScalar(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            //create a command and prepare it for execution
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, connection, (SqlTransaction)null, commandType, commandText, commandParameters);
            try
            {
                //execute the command & return the results
                object retval = cmd.ExecuteScalar();
                return retval;
            }
//            catch(Exception e)
//            {
//                HttpContext.Current.Response.Write(e.Message);
//                return null;
//            }
            finally
            {
                // detach the SqlParameters from the command object, so they can be used again.            
                cmd.Parameters.Clear();
                //关闭连接
                if(connection.State==ConnectionState.Open)connection.Close();
            }
        }
    }
    //*********************************************************************
    //
    // SqlHelperParameterCache provides functions to leverage a static cache of procedure parameters, and the
    // ability to discover parameters for stored procedures at run-time.
    //
    //*********************************************************************

    public sealed class SqlHelperParameterCache
    {
        //*********************************************************************
        //
        // Since this class provides only static methods, make the default constructor private to prevent 
        // instances from being created with "new SqlHelperParameterCache()".
        //
        //*********************************************************************

        private SqlHelperParameterCache() {}

        private static Hashtable paramCache = Hashtable.Synchronized(new Hashtable());

        //*********************************************************************
        //
        // resolve at run time the appropriate set of SqlParameters for a stored procedure
        // 
        // param name="connectionString" a valid connection string for a SqlConnection 
        // param name="spName" the name of the stored procedure 
        // param name="includeReturnValueParameter" whether or not to include their return value parameter 
        //
        //*********************************************************************

        private static SqlParameter[] DiscoverSpParameterSet(string connectionString, string spName, bool includeReturnValueParameter)
        {
            using (SqlConnection cn = new SqlConnection(connectionString)) 
            using (SqlCommand cmd = new SqlCommand(spName,cn))
            {
                cn.Open();
                cmd.CommandType = CommandType.StoredProcedure;

                SqlCommandBuilder.DeriveParameters(cmd);

                if (!includeReturnValueParameter) 
                {
                    cmd.Parameters.RemoveAt(0);
                }

                SqlParameter[] discoveredParameters = new SqlParameter[cmd.Parameters.Count];;

                cmd.Parameters.CopyTo(discoveredParameters, 0);

                return discoveredParameters;
            }
        }

        private static SqlParameter[] CloneParameters(SqlParameter[] originalParameters)
        {
            //deep copy of cached SqlParameter array
            SqlParameter[] clonedParameters = new SqlParameter[originalParameters.Length];

            for (int i = 0, j = originalParameters.Length; i < j; i++)
            {
                clonedParameters[i] = (SqlParameter)((ICloneable)originalParameters[i]).Clone();
            }

            return clonedParameters;
        }

        //*********************************************************************
        //
        // add parameter array to the cache
        //
        // param name="connectionString" a valid connection string for a SqlConnection 
        // param name="commandText" the stored procedure name or T-SQL command 
        // param name="commandParameters" an array of SqlParamters to be cached 
        //
        //*********************************************************************

        public static void CacheParameterSet(string connectionString, string commandText, params SqlParameter[] commandParameters)
        {
            string hashKey = connectionString + ":" + commandText;

            paramCache[hashKey] = commandParameters;
        }

        //*********************************************************************
        //
        // Retrieve a parameter array from the cache
        // 
        // param name="connectionString" a valid connection string for a SqlConnection 
        // param name="commandText" the stored procedure name or T-SQL command 
        // returns an array of SqlParamters
        //
        //*********************************************************************

        public static SqlParameter[] GetCachedParameterSet(string connectionString, string commandText)
        {
            string hashKey = connectionString + ":" + commandText;

            SqlParameter[] cachedParameters = (SqlParameter[])paramCache[hashKey];
            
            if (cachedParameters == null)
            {            
                return null;
            }
            else
            {
                return CloneParameters(cachedParameters);
            }
        }

        //*********************************************************************
        //
        // Retrieves the set of SqlParameters appropriate for the stored procedure
        // 
        // This method will query the database for this information, and then store it in a cache for future requests.
        // 
        // param name="connectionString" a valid connection string for a SqlConnection 
        // param name="spName" the name of the stored procedure 
        // returns an array of SqlParameters
        //
        //*********************************************************************

        public static SqlParameter[] GetSpParameterSet(string connectionString, string spName)
        {
            return GetSpParameterSet(connectionString, spName, false);
        }

        //*********************************************************************
        //
        // Retrieves the set of SqlParameters appropriate for the stored procedure
        // 
        // This method will query the database for this information, and then store it in a cache for future requests.
        // 
        // param name="connectionString" a valid connection string for a SqlConnection 
        // param name="spName" the name of the stored procedure 
        // param name="includeReturnValueParameter" a bool value indicating whether the return value parameter should be included in the results 
        // returns an array of SqlParameters
        //
        //*********************************************************************

        public static SqlParameter[] GetSpParameterSet(string connectionString, string spName, bool includeReturnValueParameter)
        {
            string hashKey = connectionString + ":" + spName + (includeReturnValueParameter ? ":include ReturnValue Parameter":"");

            SqlParameter[] cachedParameters;
            
            cachedParameters = (SqlParameter[])paramCache[hashKey];

            if (cachedParameters == null)
            {            
                cachedParameters = (SqlParameter[])(paramCache[hashKey] = DiscoverSpParameterSet(connectionString, spName, includeReturnValueParameter));
            }
            
            return CloneParameters(cachedParameters);
        }

        
    }
}
