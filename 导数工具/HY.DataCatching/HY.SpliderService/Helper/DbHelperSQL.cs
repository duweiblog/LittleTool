using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;
using System.Reflection;

namespace HY.SpliderService //可以修改成实际项目的命名空间名称
{
    /// <summary>
    /// Copyright (C) 2007-2010 
    /// 数据访问抽象基础类(基于SQLServer)
    /// 用户可以修改满足自己项目的需要。
    /// </summary>
    public abstract class DbHelperSQL
    {
        public static SqlConnection conn = new SqlConnection();
        public static SqlCommand comm = new SqlCommand();
        public static string connectionString = GetConnection();

        public DbHelperSQL()
        {
        }

        /// <summary> 
        /// 关闭当前数据库连接 
        /// </summary> 
        public static void closeConnection()
        {
            if (conn.State == ConnectionState.Open)
                conn.Close();
            conn.Dispose();
            comm.Dispose();
        }
        /// <summary>
        /// 连接标准数据库,连接字符串在config文件中
        /// </summary>
        /// <returns></returns>
        public static string GetConnection()
        {
            return GetConnection(false);
        }
        /// <summary>
        /// 连接标准数据库,连接字符串在config文件中
        /// 参数：IsLockDB 等于ture连接锁库，false连接标准库
        /// </summary>
        /// <param name="IsLockDB">ture连接锁库，false连接标准库</param>
        /// <returns></returns>
        public static string GetConnection(bool IsLockDB)
        {
            return ConfigurationOperator.GetValue("connectionstring");
        }

        #region 公用方法
        /// <summary>
        /// 得到数据库某字段最大ID
        /// </summary>
        /// <param name="FieldName"></param>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public static int GetMaxID(string FieldName, string TableName)
        {
            string strsql = "select max(" + FieldName + ")+1 from " + TableName;
            object obj = GetSingle(strsql);
            if (obj == null)
            {
                return 1;
            }
            else
            {
                return int.Parse(obj.ToString());
            }
        }

        /// <summary>
        /// 有条件得到数据库某字段最大ID
        /// </summary>
        /// <param name="FieldName"></param>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public static int GetMaxID(string FieldName, string TableName, string Where)
        {
            string strsql = "select max(" + FieldName + ")+1 from " + TableName + " where " + Where;
            object obj = GetSingle(strsql);
            if (obj == null)
            {
                return 1;
            }
            else
            {
                return int.Parse(obj.ToString());
            }
        }

        public static bool Exists(string strSql)
        {
            object obj = GetSingle(strSql.ToString());
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
        #endregion

        #region  执行简单SQL语句

        /// <summary>
		/// 执行SQL语句，返回影响的记录数
		/// </summary>
		/// <param name="SQLString">SQL语句</param>
		/// <returns>影响的记录数</returns>
		public static int ExecuteSql(string SQLString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (System.Data.SqlClient.SqlException E)
                    {
                        connection.Close();
                        throw new Exception(E.Message);
                    }
                }
            }
        }

        public static int ExecuteSql(string SQLString, bool IsLockDB)
        {
            using (SqlConnection connection = new SqlConnection(GetConnection(IsLockDB)))
            {
                using (SqlCommand cmd = new SqlCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (System.Data.SqlClient.SqlException E)
                    {
                        connection.Close();
                        throw new Exception(E.Message);
                    }
                }
            }
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">多条SQL语句</param>		
        public static void ExecuteSqlTran(ArrayList SQLStringList)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                SqlTransaction tx = conn.BeginTransaction();
                cmd.Transaction = tx;
                try
                {
                    for (int n = 0; n < SQLStringList.Count; n++)
                    {
                        string strsql = SQLStringList[n].ToString();
                        if (strsql.Trim().Length > 1)
                        {
                            cmd.CommandText = strsql;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    tx.Commit();
                }
                catch (System.Data.SqlClient.SqlException E)
                {
                    tx.Rollback();
                    throw new Exception(E.Message);
                }
            }
        }

        public static void BulkToDB(string TableName, DataTable dt)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                SqlTransaction tx = conn.BeginTransaction();
                cmd.Transaction = tx;
                try
                {
                    SqlBulkCopy bulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.CheckConstraints, tx);
                    bulkCopy.DestinationTableName = TableName;
                    bulkCopy.BatchSize = dt.Rows.Count;
                    if (dt != null && dt.Rows.Count != 0)
                        bulkCopy.WriteToServer(dt);
                    tx.Commit();
                }
                catch (System.Data.SqlClient.SqlException E)
                {
                    tx.Rollback();
                    throw new Exception(E.Message);
                } 
            }
        }
 
        /// <summary>
        /// 执行带一个存储过程参数的的SQL语句。
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <param name="content">参数内容,比如一个字段是格式复杂的文章，有特殊符号，可以通过这个方式添加</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteSql(string SQLString, string content)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(SQLString, connection);
                System.Data.SqlClient.SqlParameter myParameter = new System.Data.SqlClient.SqlParameter("@content", SqlDbType.NText);
                myParameter.Value = content;
                cmd.Parameters.Add(myParameter);
                try
                {
                    connection.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows;
                }
                catch (System.Data.SqlClient.SqlException E)
                {
                    throw new Exception(E.Message);
                }
                finally
                {
                    cmd.Dispose();
                    connection.Close();
                }
            }
        }
        /// <summary>
        /// 向数据库里插入图像格式的字段(和上面情况类似的另一种实例)
        /// </summary>
        /// <param name="strSQL">SQL语句</param>
        /// <param name="fs">图像字节,数据库的字段类型为image的情况</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteSqlInsertImg(string strSQL, byte[] fs)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(strSQL, connection);
                System.Data.SqlClient.SqlParameter myParameter = new System.Data.SqlClient.SqlParameter("@fs", SqlDbType.Image);
                myParameter.Value = fs;
                cmd.Parameters.Add(myParameter);
                try
                {
                    connection.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows;
                }
                catch (System.Data.SqlClient.SqlException E)
                {
                    throw new Exception(E.Message);
                }
                finally
                {
                    cmd.Dispose();
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="SQLString">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        public static object GetSingle(string SQLString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        object obj = cmd.ExecuteScalar();
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
                        connection.Close();
                        throw new Exception(e.Message);
                    }
                }
            }
        }

        public static object GetSingleVal(string SQLString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        object obj = cmd.ExecuteScalar();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return "-1";
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        connection.Close();
                        throw new Exception(e.Message);
                    }
                }
            }
        }

        /// <summary>
        /// 执行查询语句，返回SqlDataReader
        /// </summary>
        /// <param name="strSQL">查询语句</param>
        /// <returns>SqlDataReader</returns>
        public static SqlDataReader ExecuteReader(string strSQL)
        {
            if (conn.State == ConnectionState.Open)
                conn.Close();
            //SqlConnection connection = new SqlConnection(connectionString);
            conn.ConnectionString = connectionString;
            comm.CommandText = strSQL;
            comm.Connection = conn;

            //SqlCommand cmd = new SqlCommand(strSQL,connection);				
            try
            {
                conn.Open();
                SqlDataReader myReader = comm.ExecuteReader();
                return myReader;
            }
            catch (System.Data.SqlClient.SqlException e)
            {
                throw new Exception(e.Message);
            }

        }

        /// <summary>
        /// 执行查询语句，返回SqlDataReader
        /// </summary>
        /// <param name="strSQL">查询语句</param>
        /// <returns>SqlDataReader</returns>
        public static SqlDataReader ExecuteReader(string strSQL, bool IsLockDB)
        {
            //SqlConnection connection = new SqlConnection(GetConnection(IsLockDB));
            if (conn.State == ConnectionState.Open)
                conn.Close();
            conn.ConnectionString = GetConnection(IsLockDB);
            SqlCommand cmd = new SqlCommand(strSQL, conn);
            try
            {
                conn.Open();
                SqlDataReader myReader = cmd.ExecuteReader();
                return myReader;
            }
            catch (System.Data.SqlClient.SqlException e)
            {
                throw new Exception(e.Message);
            }

        }

        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public static DataSet Query(string SQLString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    SqlDataAdapter command = new SqlDataAdapter(SQLString, connection);
                    command.Fill(ds, "ds");
                }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
        }
        #endregion

        #region 执行带参数的SQL语句

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteSql(string SQLString, params SqlParameter[] cmdParms)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                        int rows = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        return rows;
                    }
                    catch (System.Data.SqlClient.SqlException E)
                    {
                        throw new Exception(E.Message);
                    }
                }
            }
        }


        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">SQL语句的哈希表（key为sql语句，value是该语句的SqlParameter[]）</param>
        public static int ExecuteSqlTran(Hashtable SQLStringList)
        {
            var val = 0;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    SqlCommand cmd = new SqlCommand();
                    try
                    {
                        //循环
                        foreach (DictionaryEntry myDE in SQLStringList)
                        {
                            string cmdText = myDE.Key.ToString();
                            SqlParameter[] cmdParms = (SqlParameter[])myDE.Value;
                            PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
                            val += cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();

                            trans.Commit();
                        }
                        return val;
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        public static void ExecuteSqlTran(string SQLStringList)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                SqlTransaction transaction = connection.BeginTransaction();
                command.Transaction = transaction;
                try
                {
                    foreach (string text in SQLStringList.Split(new char[] { ';' }))
                    {
                        if (text.Trim() != "")
                        {
                            command.CommandText = text;
                            command.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                }
                catch (SqlException exception)
                {
                    transaction.Rollback();
                    throw new Exception(exception.Message);
                }
            }
        }

        public static void ExecuteSqlTran(string SQLString1, string SQLString2)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                SqlTransaction transaction = connection.BeginTransaction();
                command.Transaction = transaction;
                try
                {
                    command.CommandText = SQLString1;
                    command.ExecuteNonQuery();
                    command.CommandText = SQLString2;
                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (SqlException exception)
                {
                    transaction.Rollback();
                    throw new Exception(exception.Message);
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }
            }
        }
        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="SQLString">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
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
                        throw new Exception(e.Message);
                    }
                }
            }
        }

        /// <summary>
        /// 执行查询语句，返回SqlDataReader
        /// </summary>
        /// <param name="strSQL">查询语句</param>
        /// <returns>SqlDataReader</returns>
        public static SqlDataReader ExecuteReader(string SQLString, params SqlParameter[] cmdParms)
        {
            if (conn.State == ConnectionState.Open)
                conn.Close();
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand();
            try
            {
                PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                SqlDataReader myReader = cmd.ExecuteReader();
                cmd.Parameters.Clear();
                return myReader;
            }
            catch (System.Data.SqlClient.SqlException e)
            {
                throw new Exception(e.Message);
            }

        }

        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public static DataSet Query(string SQLString, params SqlParameter[] cmdParms)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand();
                PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    try
                    {
                        da.Fill(ds, "ds");
                        cmd.Parameters.Clear();
                    }
                    catch (System.Data.SqlClient.SqlException ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    return ds;
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
                foreach (SqlParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }

        private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, string cmdText, CommandType cmdType, SqlParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
                cmd.Transaction = trans;
            cmd.CommandType = cmdType;
            if (cmdParms != null)
            {
                foreach (SqlParameter parm in cmdParms)
                {
                    if (parm != null)
                        cmd.Parameters.Add(parm);
                }
            }
        }

        #endregion

        #region 存储过程操作

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>SqlDataReader</returns>
        public static SqlDataReader RunProcedure(string storedProcName, IDataParameter[] parameters)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            SqlDataReader returnReader;
            connection.Open();
            SqlCommand command = BuildQueryCommand(connection, storedProcName, parameters);
            command.CommandType = CommandType.StoredProcedure;
            returnReader = command.ExecuteReader();
            return returnReader;
        }


        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <param name="tableName">DataSet结果中的表名</param>
        /// <returns>DataSet</returns>
        public static DataSet RunProcedure(string storedProcName, IDataParameter[] parameters, string tableName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                DataSet dataSet = new DataSet();
                connection.Open();
                SqlDataAdapter sqlDA = new SqlDataAdapter();
                sqlDA.SelectCommand = BuildQueryCommand(connection, storedProcName, parameters);
                sqlDA.Fill(dataSet, tableName);
                connection.Close();
                return dataSet;
            }
        }


        /// <summary>
        /// 构建 SqlCommand 对象(用来返回一个结果集，而不是一个整数值)
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>SqlCommand</returns>
        private static SqlCommand BuildQueryCommand(SqlConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            SqlCommand command = new SqlCommand(storedProcName, connection);
            command.CommandType = CommandType.StoredProcedure;
            foreach (SqlParameter parameter in parameters)
            {
                if (parameter != null)
                    command.Parameters.Add(parameter);
            }
            return command;
        }

        /// <summary>
        /// 执行存储过程，返回影响的行数		
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <param name="rowsAffected">影响的行数</param>
        /// <returns></returns>
        public static int RunProcedure(string storedProcName, IDataParameter[] parameters, out int rowsAffected)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                int result;
                connection.Open();
                SqlCommand command = BuildIntCommand(connection, storedProcName, parameters);
                rowsAffected = command.ExecuteNonQuery();
                result = (int)command.Parameters["ReturnValue"].Value;
                //Connection.Close();
                return result;
            }
        }

        /// <summary>
        /// 创建 SqlCommand 对象实例(用来返回一个整数值)	
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>SqlCommand 对象实例</returns>
        private static SqlCommand BuildIntCommand(SqlConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            SqlCommand command = BuildQueryCommand(connection, storedProcName, parameters);
            command.Parameters.Add(new SqlParameter("ReturnValue",
                SqlDbType.Int, 4, ParameterDirection.ReturnValue,
                false, 0, 0, string.Empty, DataRowVersion.Default, null));
            return command;
        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="cmdType">命令字符串类型</param>
        /// <param name="cmdText">存储过程名称</param>
        /// <param name="cmdParms">参数集合</param>
        /// <returns>查询结果集，类型为DataTable</returns>
        public static DataTable ExecuteProcedure(CommandType cmdType, string cmdText, params SqlParameter[] cmdParms)
        {

            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand();
            try
            {
                PrepareCommand(cmd, connection, null, cmdText, cmdType, cmdParms);
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adp.Fill(dt);
                cmd.Parameters.Clear();
                return dt;
            }
            catch
            {
                conn.Close();
                throw;
            }
        }
        #endregion

        #region DataTableToObjList
        /// <summary>
        /// DataTable数据转成Entity
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="ds">数据集</param>
        /// <param name="tbName">表名</param>
        /// <returns></returns>
        public static List<T> DataTableToObjList<T>(DataTable dt) where T : new()
        {
            List<T> list = new List<T>();
            PropertyInfo[] propinfos = null;

            foreach (DataRow dr in dt.Rows)
            {

                T entity = new T();
                //初始化propertyinfo
                if (propinfos == null)
                {
                    Type objtype = entity.GetType();
                    propinfos = objtype.GetProperties();
                }

                //填充entity类的属性

                foreach (PropertyInfo propinfo in propinfos)
                {
                    foreach (DataColumn dc in dt.Columns)
                    {
                        if (dc.ColumnName.ToUpper().Equals(propinfo.Name.ToUpper()))
                        {

                            string v = null;
                            v = dr[dc.ColumnName].ToString();
                            if (!String.IsNullOrEmpty(v))
                            {
                                if (propinfo.PropertyType.Equals(typeof(DateTime?)))
                                {
                                    propinfo.SetValue(entity, (System.Nullable<DateTime>)DateTime.Parse(v), null);
                                }
                                else if (propinfo.PropertyType.Equals(typeof(System.Boolean?)))
                                {
                                    propinfo.SetValue(entity, System.Boolean.Parse(v), null);
                                }
                                else if (propinfo.PropertyType.Equals(typeof(int?)))
                                {
                                    propinfo.SetValue(entity, Convert.ToInt32(v), null);

                                }
                                else
                                {
                                    propinfo.SetValue(entity, Convert.ChangeType(v, propinfo.PropertyType), null);

                                }
                                break;
                            }

                        }
                    }
                }
                list.Add(entity);
            }
            return list;
        }


        #endregion



    }
}
//sss