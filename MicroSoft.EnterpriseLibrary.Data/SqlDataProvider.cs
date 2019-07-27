using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Collections;
using System.Configuration;
using System.Text;

namespace MicroSoft.EnterpriseLibrary.Data
{
    /// <summary>
    /// 把 SQL 和当前委托的方法在同一个事务中执行
    /// </summary>
    /// <param name="param">委托的参数</param>
    public delegate void TransExecDelegate(object param);

    /// <summary>
    /// 数据库操作模块
    /// </summary>
    public static class SqlDataProvider
    {
        /// <summary>
        /// 数据库连接串
        /// </summary>
        private static string connectionString = System.Configuration.ConfigurationSettings.AppSettings.Get("connstr");

        /// <summary>
        /// 缓存存储过程的参数列表
        /// </summary>
        private static Hashtable paramCache = Hashtable.Synchronized(new Hashtable());

        /// <summary>
        /// 清空缓存中的存储过程参数列表
        /// </summary>
        public static void ClearCache()
        {
            paramCache.Clear();
        }

        /// <summary>
        /// 返回数据库连接对象
        /// </summary>
        /// <returns>返回数据库连接对象</returns>
        public static SqlConnection GetSqlConnection()
        {
            SqlConnection conn = new SqlConnection(connectionString);

            if (conn.State != ConnectionState.Open)
                conn.Open();

            return conn;
        }

        /// <summary>
        /// 创建SQL参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <param name="dbType">参数类型</param>
        /// <param name="value">参数值</param>
        /// <returns>返回创建得参数对象</returns>
        public static SqlParameter CreateSqlParameter(string parameterName, SqlDbType dbType, object value)
        {
            SqlParameter parameter = new SqlParameter(parameterName, dbType);
            parameter.Value = value;
            return parameter;
        }

        /// <summary>
        /// 把SQL参数列表转换成数组
        /// </summary>
        /// <param name="list">SQL参数列表</param>
        /// <returns>SQL参数数组</returns>
        public static SqlParameter[] GetParameterArray(SqlParameterCollection list)
        {
            if (list == null || list.Count == 0)
                return null;

            SqlParameter[] array = new SqlParameter[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                array[i] = (SqlParameter)((ICloneable)list[i]).Clone();
            }

            return array;
        }

        /// <summary>
        /// 判断是否存在
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public static bool Exists(string strSql, params SqlParameter[] cmdParms)
        {
            object obj = GetSingle(strSql, cmdParms);
            int cmdresult;
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                cmdresult = 0;
            }
            else
            {
                cmdresult = int.Parse(obj.ToString());
            }
            if (cmdresult == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static object GetSingle(string SQLString, params SqlParameter[] cmdParms)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                        object obj = cmd.ExecuteScalar();
                        cmd.Parameters.Clear();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        throw e;
                    }
                }
            }
        }

        private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, string cmdText, SqlParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
                cmd.Transaction = trans;
            cmd.CommandType = CommandType.Text;//cmdType;
            if (cmdParms != null)
            {


                foreach (SqlParameter parameter in cmdParms)
                {
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
                        (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    cmd.Parameters.Add(parameter);
                }
            }
        }

        #region 本地存储过程、SQL语句操作和查询

        /// <summary>
        /// 根据SQL语句返回DataSet
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="commandParameters">可变个数的参数</param>
        /// <returns>根据SQL语句返回DataSet</returns>
        public static DataSet GetResultBySql(string sql, params SqlParameter[] commandParameters)
        {
            return SqlHelper.ExecuteDataset(GetSqlConnection(), CommandType.Text, sql, commandParameters);
        }

        /// <summary>
        /// 根据存储过程语句返回DataSet
        /// </summary>
        /// <param name="proc">存储过程名称</param>
        /// <param name="commandParameters">可变个数的参数</param>
        /// <returns>根据存储过程语句返回DataSet</returns>
        public static DataSet GetResultByProc(string proc, params SqlParameter[] commandParameters)
        {
            using (SqlConnection conn = GetSqlConnection())
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                SqlCommand cmd = new SqlCommand();

                // 获取数据集的存储过程暂时关闭事务控制, 因为基本上返回数据集的过程都没有执行SQL语句

                SqlTransaction transaction = conn.BeginTransaction();

                SqlHelper.PrepareCommand(cmd, conn, transaction, CommandType.StoredProcedure, proc, commandParameters);

                try
                {
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);

                    DataSet ds = new DataSet();

                    sda.Fill(ds);

                    transaction.Commit();

                    return ds;
                }
                catch (Exception e)
                {
                    transaction.Rollback();

                    throw e;
                }
            }
        }
        /// <summary>
        /// 根据存储过程语句返回DataSet
        /// </summary>
        /// <param name="proc">存储过程名称</param>
        /// <param name="parameters">可变个数的参数</param>
        /// <returns>根据存储过程语句返回DataSet</returns>
        public static DataSet GetResultByProc(string proc, params object[] parameters)
        {
            if ((parameters != null) && (parameters.Length > 0))
            {
                SqlParameter[] commandParameters = GetSpParameterSet(proc);

                AssignParameterValues(commandParameters, parameters);

                return GetResultByProc(proc, commandParameters);
            }
            else
            {
                return GetResultByProc(proc, (SqlParameter[])null);
            }
        }

        /// <summary>
        /// 根据SQL返回数据集合第一行第一列
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="commandParameters">可变个数的参数</param>
        /// <returns>根据SQL返回数据集合第一行第一列</returns>
        public static object GetScalarBySql(string sql, params SqlParameter[] commandParameters)
        {
            return SqlHelper.ExecuteScalar(GetSqlConnection(), CommandType.Text, sql, commandParameters);
        }

        /// <summary>
        /// 根据存储过程返回数据集合第一行第一列
        /// </summary>
        /// <param name="proc">存储过程名称</param>
        /// <param name="commandParameters">可变个数的参数</param>
        /// <returns>根据存储过程返回数据集合第一行第一列</returns>
        public static object GetScalarByProc(string proc, params SqlParameter[] commandParameters)
        {
            return GetScalarByProc(proc, null, null, commandParameters);
        }

        /// <summary>
        /// 根据存储过程返回数据集合第一行第一列
        /// </summary>
        /// <param name="proc">存储过程名称</param>
        /// <param name="func">需要包含在事务中方法委托</param>
        /// <param name="param">委托方法的参数</param>
        /// <param name="commandParameters">可变个数的参数</param>
        /// <returns>根据存储过程返回数据集合第一行第一列</returns>
        public static object GetScalarByProc(string proc, TransExecDelegate func, object param, params SqlParameter[] commandParameters)
        {
            using (SqlConnection conn = GetSqlConnection())
            {
                SqlCommand cmd = new SqlCommand();

                SqlTransaction transaction = conn.BeginTransaction();

                SqlHelper.PrepareCommand(cmd, conn, transaction, CommandType.StoredProcedure, proc, commandParameters);

                try
                {
                    object obj = cmd.ExecuteScalar();

                    if (func != null)
                    {
                        func(param);
                    }

                    transaction.Commit();

                    return (obj == null) ? null : obj;
                }
                catch (Exception e)
                {
                    transaction.Rollback();

                    throw e;
                }
            }
        }
        /// <summary>
        /// 根据存储过程返回数据集合第一行第一列
        /// </summary>
        /// <param name="proc">存储过程名称</param>
        /// <param name="parameters">可变个数的参数</param>
        /// <returns>根据存储过程返回数据集合第一行第一列</returns>
        public static object GetScalarByProc(string proc, params object[] parameters)
        {
            return GetScalarByProc(proc, null, null, parameters);
        }

        /// <summary>
        /// 根据存储过程返回数据集合第一行第一列
        /// </summary>
        /// <param name="proc">存储过程名称</param>
        /// <param name="func">需要包含在事务中方法委托</param>
        /// <param name="param">委托方法的参数</param>
        /// <param name="parameters">可变个数的参数</param>
        /// <returns>根据存储过程返回数据集合第一行第一列</returns>
        public static object GetScalarByProc(string proc, TransExecDelegate func, object param, params object[] parameters)
        {
            if ((parameters != null) && (parameters.Length > 0))
            {
                SqlParameter[] commandParameters = GetSpParameterSet(proc);

                AssignParameterValues(commandParameters, parameters);

                return GetScalarByProc(proc, func, param, commandParameters);
            }
            else
            {
                return GetScalarByProc(proc, func, param, (SqlParameter[])null);
            }
        }

        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="commandParameters">可变个数的参数</param>
        /// <returns>返回影响的记录行数</returns>
        public static int ExecuteBySql(string sql, params SqlParameter[] commandParameters)
        {
            return SqlHelper.ExecuteNonQuery(GetSqlConnection(), CommandType.Text, sql, commandParameters);
        }
       

        /// <summary>
        /// 执行存储过程语句
        /// </summary>
        /// <param name="proc">存储过程名称</param>
        /// <param name="commandParameters">可变个数的参数</param>
        /// <returns>返回影响的记录行数</returns>
        public static int ExecuteByProc(string proc, params SqlParameter[] commandParameters)
        {
            return ExecuteByProc(proc, null, null, commandParameters);
        }

        /// <summary>
        /// 执行存储过程语句
        /// </summary>
        /// <param name="proc">存储过程名称</param>
        /// <param name="func">需要包含在事务中方法委托</param>
        /// <param name="param">委托方法的参数</param>
        /// <param name="commandParameters">可变个数的参数</param>
        /// <returns>返回影响的记录行数</returns>
        public static int ExecuteByProc(string proc, TransExecDelegate func, object param, params SqlParameter[] commandParameters)
        {
            using (SqlConnection conn = GetSqlConnection())
            {
                SqlCommand cmd = new SqlCommand();

                SqlTransaction transaction = conn.BeginTransaction();

                SqlHelper.PrepareCommand(cmd, conn, transaction, CommandType.StoredProcedure, proc, commandParameters);

                try
                {
                    int rows = cmd.ExecuteNonQuery();

                    if (func != null)
                    {
                        func(param);
                    }

                    transaction.Commit();

                    return rows;
                }
                catch (Exception e)
                {
                    transaction.Rollback();

                    throw e;
                }
            }
        }

        /// <summary>
        /// 执行存储过程语句
        /// </summary>
        /// <param name="proc">存储过程名称</param>
        /// <param name="parameters">可变个数的参数</param>
        /// <returns>返回影响的记录行数</returns>
        public static int ExecuteByProc(string proc, params object[] parameters)
        {
            return ExecuteByProc(proc, null, null, parameters);
        }

        /// <summary>
        /// 执行存储过程语句
        /// </summary>
        /// <param name="proc">存储过程名称</param>
        /// <param name="func">需要包含在事务中方法委托</param>
        /// <param name="param">委托方法的参数</param>
        /// <param name="parameters">可变个数的参数</param>
        /// <returns>返回影响的记录行数</returns>
        public static int ExecuteByProc(string proc, TransExecDelegate func, object param, params object[] parameters)
        {
            if ((parameters != null) && (parameters.Length > 0))
            {
                SqlParameter[] commandParameters = GetSpParameterSet(proc);

                AssignParameterValues(commandParameters, parameters);

                return ExecuteByProc(proc, func, param, commandParameters);
            }
            else
            {
                return ExecuteByProc(proc, func, param, (SqlParameter[])null);
            }
        }

        #endregion

        #region 根据存储过程名称缓存参数

        /// <summary>
        /// 获取参数数组
        /// </summary>
        /// <param name="proc">存储过程名</param>
        /// <returns>返回参数数组</returns>
        public static SqlParameter[] GetSpParameterSet(string proc)
        {
            return GetSpParameterSet(proc, false);
        }

        /// <summary>
        /// 获取参数数组
        /// </summary>
        /// <param name="proc">存储过程名</param>
        /// <param name="includeReturnValueParameter">是否包含返回值</param>
        /// <returns>返回参数数组</returns>
        public static SqlParameter[] GetSpParameterSet(string proc, bool includeReturnValueParameter)
        {
            string hashKey = proc + (includeReturnValueParameter ? ":include ReturnValue Parameter" : "");

            SqlParameter[] cachedParameters;

            cachedParameters = (SqlParameter[])paramCache[hashKey];

            if (cachedParameters == null)
            {
                cachedParameters = (SqlParameter[])(paramCache[hashKey] = DiscoverSpParameterSet(proc, includeReturnValueParameter));
            }

            return CloneParameters(cachedParameters);
        }

        /// <summary>
        /// 通过存储过程获取参数列表
        /// </summary>
        /// <param name="proc">存储过程名称</param>
        /// <param name="includeReturnValueParameter">是否包含返回值</param>
        /// <returns>SQL参数数组</returns>
        private static SqlParameter[] DiscoverSpParameterSet(string proc, bool includeReturnValueParameter)
        {
            using (SqlConnection conn = GetSqlConnection())
            {
                using (SqlCommand cmd = new SqlCommand(proc, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlCommandBuilder.DeriveParameters(cmd);

                    if (!includeReturnValueParameter)
                    {
                        cmd.Parameters.RemoveAt(0);
                    }

                    SqlParameter[] discoveredParameters = new SqlParameter[cmd.Parameters.Count]; ;

                    cmd.Parameters.CopyTo(discoveredParameters, 0);

                    return discoveredParameters;
                }
            }
        }

        /// <summary>
        /// 复制参数数组
        /// </summary>
        /// <param name="originalParameters">源数组</param>
        /// <returns>返回数组</returns>
        private static SqlParameter[] CloneParameters(SqlParameter[] originalParameters)
        {
            SqlParameter[] clonedParameters = new SqlParameter[originalParameters.Length];

            for (int i = 0, j = originalParameters.Length; i < j; i++)
            {
                clonedParameters[i] = (SqlParameter)((ICloneable)originalParameters[i]).Clone();
            }

            return clonedParameters;
        }

        /// <summary>
        /// 参数赋值
        /// </summary>
        /// <param name="commandParameters">参数数组</param>
        /// <param name="parameterValues">参数值数组</param>
        private static void AssignParameterValues(SqlParameter[] commandParameters, object[] parameterValues)
        {
            if ((commandParameters == null) || (parameterValues == null))
            {
                return;
            }

            if (commandParameters.Length != parameterValues.Length)
            {
                throw new ArgumentException("存储过程中参数个数跟提供的参数个数不一致");
            }

            for (int i = 0, j = commandParameters.Length; i < j; i++)
            {
                commandParameters[i].Value = parameterValues[i];
            }
        }

        public static SqlParameter GetOutputParameter(string parameterName, SqlDbType dbType)
        {
            return new SqlParameter(parameterName, dbType) { Direction = ParameterDirection.Output };
        }

        /// <summary>
        /// 创建参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static SqlParameter GetParameterFromString(string parameterName, string value)
        {
            if (string.IsNullOrEmpty(value))
                return new SqlParameter(parameterName, DBNull.Value);
            else
                return new SqlParameter(parameterName, value);
        }

        public static bool ExecuteTransWithArrayList(ArrayList lstComm)
        {
            //创建连接
            using (SqlConnection conn = GetSqlConnection())
            {
                SqlTransaction transaction = null;
                try
                {
                    transaction = conn.BeginTransaction();
                    int count = lstComm.Count;
                    //设置参数
                    if (lstComm != null && count > 0)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            SqlCommand comm = (SqlCommand)lstComm[i];
                            //设置命令数据库连接
                            comm.Connection = conn;
                            comm.Transaction = transaction;
                            //执行数据库操作
                            comm.ExecuteNonQuery();
                        }
                    }
                    //提交事务
                    transaction.Commit();

                }
                catch (Exception e)
                {
                    //如果开始了事务，则回滚事务
                    if (transaction != null)
                    {
                        transaction.Rollback();
                    }

                    throw e;
                }
                finally
                {
                    //连接打开时，关闭连接
                    if (conn != null)
                    {
                        conn.Close();
                    }
                }
            }

            return true;
        }
        /**/
        /// <summary>
        /// 返回一个SqlDataReader集合
        /// </summary>
        /// <param name="sqlStr">要执行查询的SQL语句</param>
        public static SqlDataReader GetDataReader(string sqlStr)
        {
            SqlConnection conn = new SqlConnection(connectionString);
            if (conn.State != System.Data.ConnectionState.Open)
            {
                conn.Open();
            }
            SqlCommand MyCmd = new SqlCommand(sqlStr, conn);
            SqlDataReader dataReader = MyCmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
            return dataReader;
        }
        #endregion
    }
}