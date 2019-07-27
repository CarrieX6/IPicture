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
    /// �� SQL �͵�ǰί�еķ�����ͬһ��������ִ��
    /// </summary>
    /// <param name="param">ί�еĲ���</param>
    public delegate void TransExecDelegate(object param);

    /// <summary>
    /// ���ݿ����ģ��
    /// </summary>
    public static class SqlDataProvider
    {
        /// <summary>
        /// ���ݿ����Ӵ�
        /// </summary>
        private static string connectionString = System.Configuration.ConfigurationSettings.AppSettings.Get("connstr");

        /// <summary>
        /// ����洢���̵Ĳ����б�
        /// </summary>
        private static Hashtable paramCache = Hashtable.Synchronized(new Hashtable());

        /// <summary>
        /// ��ջ����еĴ洢���̲����б�
        /// </summary>
        public static void ClearCache()
        {
            paramCache.Clear();
        }

        /// <summary>
        /// �������ݿ����Ӷ���
        /// </summary>
        /// <returns>�������ݿ����Ӷ���</returns>
        public static SqlConnection GetSqlConnection()
        {
            SqlConnection conn = new SqlConnection(connectionString);

            if (conn.State != ConnectionState.Open)
                conn.Open();

            return conn;
        }

        /// <summary>
        /// ����SQL����
        /// </summary>
        /// <param name="parameterName">��������</param>
        /// <param name="dbType">��������</param>
        /// <param name="value">����ֵ</param>
        /// <returns>���ش����ò�������</returns>
        public static SqlParameter CreateSqlParameter(string parameterName, SqlDbType dbType, object value)
        {
            SqlParameter parameter = new SqlParameter(parameterName, dbType);
            parameter.Value = value;
            return parameter;
        }

        /// <summary>
        /// ��SQL�����б�ת��������
        /// </summary>
        /// <param name="list">SQL�����б�</param>
        /// <returns>SQL��������</returns>
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
        /// �ж��Ƿ����
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

        #region ���ش洢���̡�SQL�������Ͳ�ѯ

        /// <summary>
        /// ����SQL��䷵��DataSet
        /// </summary>
        /// <param name="sql">SQL���</param>
        /// <param name="commandParameters">�ɱ�����Ĳ���</param>
        /// <returns>����SQL��䷵��DataSet</returns>
        public static DataSet GetResultBySql(string sql, params SqlParameter[] commandParameters)
        {
            return SqlHelper.ExecuteDataset(GetSqlConnection(), CommandType.Text, sql, commandParameters);
        }

        /// <summary>
        /// ���ݴ洢������䷵��DataSet
        /// </summary>
        /// <param name="proc">�洢��������</param>
        /// <param name="commandParameters">�ɱ�����Ĳ���</param>
        /// <returns>���ݴ洢������䷵��DataSet</returns>
        public static DataSet GetResultByProc(string proc, params SqlParameter[] commandParameters)
        {
            using (SqlConnection conn = GetSqlConnection())
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                SqlCommand cmd = new SqlCommand();

                // ��ȡ���ݼ��Ĵ洢������ʱ�ر��������, ��Ϊ�����Ϸ������ݼ��Ĺ��̶�û��ִ��SQL���

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
        /// ���ݴ洢������䷵��DataSet
        /// </summary>
        /// <param name="proc">�洢��������</param>
        /// <param name="parameters">�ɱ�����Ĳ���</param>
        /// <returns>���ݴ洢������䷵��DataSet</returns>
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
        /// ����SQL�������ݼ��ϵ�һ�е�һ��
        /// </summary>
        /// <param name="sql">SQL���</param>
        /// <param name="commandParameters">�ɱ�����Ĳ���</param>
        /// <returns>����SQL�������ݼ��ϵ�һ�е�һ��</returns>
        public static object GetScalarBySql(string sql, params SqlParameter[] commandParameters)
        {
            return SqlHelper.ExecuteScalar(GetSqlConnection(), CommandType.Text, sql, commandParameters);
        }

        /// <summary>
        /// ���ݴ洢���̷������ݼ��ϵ�һ�е�һ��
        /// </summary>
        /// <param name="proc">�洢��������</param>
        /// <param name="commandParameters">�ɱ�����Ĳ���</param>
        /// <returns>���ݴ洢���̷������ݼ��ϵ�һ�е�һ��</returns>
        public static object GetScalarByProc(string proc, params SqlParameter[] commandParameters)
        {
            return GetScalarByProc(proc, null, null, commandParameters);
        }

        /// <summary>
        /// ���ݴ洢���̷������ݼ��ϵ�һ�е�һ��
        /// </summary>
        /// <param name="proc">�洢��������</param>
        /// <param name="func">��Ҫ�����������з���ί��</param>
        /// <param name="param">ί�з����Ĳ���</param>
        /// <param name="commandParameters">�ɱ�����Ĳ���</param>
        /// <returns>���ݴ洢���̷������ݼ��ϵ�һ�е�һ��</returns>
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
        /// ���ݴ洢���̷������ݼ��ϵ�һ�е�һ��
        /// </summary>
        /// <param name="proc">�洢��������</param>
        /// <param name="parameters">�ɱ�����Ĳ���</param>
        /// <returns>���ݴ洢���̷������ݼ��ϵ�һ�е�һ��</returns>
        public static object GetScalarByProc(string proc, params object[] parameters)
        {
            return GetScalarByProc(proc, null, null, parameters);
        }

        /// <summary>
        /// ���ݴ洢���̷������ݼ��ϵ�һ�е�һ��
        /// </summary>
        /// <param name="proc">�洢��������</param>
        /// <param name="func">��Ҫ�����������з���ί��</param>
        /// <param name="param">ί�з����Ĳ���</param>
        /// <param name="parameters">�ɱ�����Ĳ���</param>
        /// <returns>���ݴ洢���̷������ݼ��ϵ�һ�е�һ��</returns>
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
        /// ִ��SQL���
        /// </summary>
        /// <param name="sql">SQL���</param>
        /// <param name="commandParameters">�ɱ�����Ĳ���</param>
        /// <returns>����Ӱ��ļ�¼����</returns>
        public static int ExecuteBySql(string sql, params SqlParameter[] commandParameters)
        {
            return SqlHelper.ExecuteNonQuery(GetSqlConnection(), CommandType.Text, sql, commandParameters);
        }
       

        /// <summary>
        /// ִ�д洢�������
        /// </summary>
        /// <param name="proc">�洢��������</param>
        /// <param name="commandParameters">�ɱ�����Ĳ���</param>
        /// <returns>����Ӱ��ļ�¼����</returns>
        public static int ExecuteByProc(string proc, params SqlParameter[] commandParameters)
        {
            return ExecuteByProc(proc, null, null, commandParameters);
        }

        /// <summary>
        /// ִ�д洢�������
        /// </summary>
        /// <param name="proc">�洢��������</param>
        /// <param name="func">��Ҫ�����������з���ί��</param>
        /// <param name="param">ί�з����Ĳ���</param>
        /// <param name="commandParameters">�ɱ�����Ĳ���</param>
        /// <returns>����Ӱ��ļ�¼����</returns>
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
        /// ִ�д洢�������
        /// </summary>
        /// <param name="proc">�洢��������</param>
        /// <param name="parameters">�ɱ�����Ĳ���</param>
        /// <returns>����Ӱ��ļ�¼����</returns>
        public static int ExecuteByProc(string proc, params object[] parameters)
        {
            return ExecuteByProc(proc, null, null, parameters);
        }

        /// <summary>
        /// ִ�д洢�������
        /// </summary>
        /// <param name="proc">�洢��������</param>
        /// <param name="func">��Ҫ�����������з���ί��</param>
        /// <param name="param">ί�з����Ĳ���</param>
        /// <param name="parameters">�ɱ�����Ĳ���</param>
        /// <returns>����Ӱ��ļ�¼����</returns>
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

        #region ���ݴ洢�������ƻ������

        /// <summary>
        /// ��ȡ��������
        /// </summary>
        /// <param name="proc">�洢������</param>
        /// <returns>���ز�������</returns>
        public static SqlParameter[] GetSpParameterSet(string proc)
        {
            return GetSpParameterSet(proc, false);
        }

        /// <summary>
        /// ��ȡ��������
        /// </summary>
        /// <param name="proc">�洢������</param>
        /// <param name="includeReturnValueParameter">�Ƿ��������ֵ</param>
        /// <returns>���ز�������</returns>
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
        /// ͨ���洢���̻�ȡ�����б�
        /// </summary>
        /// <param name="proc">�洢��������</param>
        /// <param name="includeReturnValueParameter">�Ƿ��������ֵ</param>
        /// <returns>SQL��������</returns>
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
        /// ���Ʋ�������
        /// </summary>
        /// <param name="originalParameters">Դ����</param>
        /// <returns>��������</returns>
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
        /// ������ֵ
        /// </summary>
        /// <param name="commandParameters">��������</param>
        /// <param name="parameterValues">����ֵ����</param>
        private static void AssignParameterValues(SqlParameter[] commandParameters, object[] parameterValues)
        {
            if ((commandParameters == null) || (parameterValues == null))
            {
                return;
            }

            if (commandParameters.Length != parameterValues.Length)
            {
                throw new ArgumentException("�洢�����в����������ṩ�Ĳ���������һ��");
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
        /// ��������
        /// </summary>
        /// <param name="parameterName">��������</param>
        /// <param name="value">ֵ</param>
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
            //��������
            using (SqlConnection conn = GetSqlConnection())
            {
                SqlTransaction transaction = null;
                try
                {
                    transaction = conn.BeginTransaction();
                    int count = lstComm.Count;
                    //���ò���
                    if (lstComm != null && count > 0)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            SqlCommand comm = (SqlCommand)lstComm[i];
                            //�����������ݿ�����
                            comm.Connection = conn;
                            comm.Transaction = transaction;
                            //ִ�����ݿ����
                            comm.ExecuteNonQuery();
                        }
                    }
                    //�ύ����
                    transaction.Commit();

                }
                catch (Exception e)
                {
                    //�����ʼ��������ع�����
                    if (transaction != null)
                    {
                        transaction.Rollback();
                    }

                    throw e;
                }
                finally
                {
                    //���Ӵ�ʱ���ر�����
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
        /// ����һ��SqlDataReader����
        /// </summary>
        /// <param name="sqlStr">Ҫִ�в�ѯ��SQL���</param>
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