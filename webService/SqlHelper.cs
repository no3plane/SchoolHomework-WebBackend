using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace webService
{
    /// <summary>
    /// 数据库访问类
    /// </summary>
    public class SqlHelper
    {
        static SqlHelper(){
            connString = "Data source=WANDERING\\SQLEXPRESS;Initial Catalog=DeviceAlert;Integrated Security=true";
        }
        /// <summary>
        /// 连接字符串字段
        /// </summary>
        private static string _connString;

        /// <summary>
        /// 数据库连接字符串
        /// <remarks>
        /// </remarks>
        /// </summary>
        public static string connString
        {
            get { return _connString; }
            set { _connString = value; }
        }

        /// <summary>
        /// 执行命令返回DataTable数据集
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="query">执行文本：SQL语句/存储过程</param>
        /// <param name="parameters">执行参数</param>
        /// <returns>返回DataTable数据集</returns>
        public static DataTable ExecuteDataTable(string query)
        {
            try
            {
                using (DataTable tempdt = new DataTable())
                {
                    using (SqlConnection conn = new SqlConnection(_connString))
                    {
                        using (SqlCommand comm = conn.CreateCommand())
                        {
                            //if (query.ToLower().Trim().StartsWith("select"))
                            //{
                                //comm.CommandType = CommandType.Text;
                            //}
                            //else
                            //{
                            //    comm.CommandType = CommandType.StoredProcedure;
                            //}
                            comm.CommandType = CommandType.Text;
                            comm.CommandText = query;

                            using (SqlDataAdapter sda = new SqlDataAdapter(comm))
                            {
                                sda.Fill(tempdt);
                            }
                        }
                    }
                    return tempdt;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 执行命令返回DataTable数据集
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="query">执行文本：SQL语句/存储过程</param>
        /// <param name="parameters">执行参数</param>
        /// <returns>返回DataTable数据集</returns>
        public static DataTable ExecuteDataTable(string query, params SqlParameter[] parameters)
        {
            try
            {
                using (DataTable tempdt = new DataTable())
                {
                    using (SqlConnection conn = new SqlConnection(_connString))
                    {
                        using (SqlCommand comm = conn.CreateCommand())
                        {
                            if (query.ToLower().Trim().StartsWith("select"))
                            {
                                comm.CommandType = CommandType.Text;
                            }
                            else
                            {
                                comm.CommandType = CommandType.StoredProcedure;
                            }

                            comm.CommandText = query;
                            for (int i = 0; i <= parameters.Length - 1; i++)
                            {
                                comm.Parameters.Add(parameters[i]);
                            }
                            using (SqlDataAdapter sda = new SqlDataAdapter(comm))
                            {
                                sda.Fill(tempdt);
                            }
                        }
                    }
                    return tempdt;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 执行命令返回受影响的行数
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="query">执行文本：SQL语句/存储过程</param>
        /// <returns>返回执行受影响的行数</returns>
        public static int ExecuteNonQuery(string query)
        {
            try
            {
                int rtn = 0;
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    using (SqlCommand comm = conn.CreateCommand())
                    {
                        if (query.ToLower().Trim().StartsWith("insert")
                            || query.ToLower().Trim().StartsWith("update") || query.ToLower().Trim().StartsWith("delete"))
                        {
                            comm.CommandType = CommandType.Text;
                        }
                        else
                        {
                            comm.CommandType = CommandType.StoredProcedure;
                        }

                        comm.CommandText = query;
                        rtn = comm.ExecuteNonQuery();
                    }
                    conn.Close();
                }
                return rtn;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 执行命令返回受影响的行数
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="query">执行文本：SQL语句/存储过程</param>
        /// <param name="parameters">执行参数</param>
        /// <returns>返回执行受影响的行数</returns>
        public static int ExecuteNonQuery(string query, params SqlParameter[] parameters)
        {
            try
            {
                int rtn = 0;
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    using (SqlCommand comm = conn.CreateCommand())
                    {
                        if (query.ToLower().Trim().StartsWith("insert")
                            || query.ToLower().Trim().StartsWith("update") || query.ToLower().Trim().StartsWith("delete"))
                        {
                            comm.CommandType = CommandType.Text;
                        }
                        else
                        {
                            comm.CommandType = CommandType.StoredProcedure;
                        }

                        comm.CommandText = query;
                        for (int i = 0; i <= parameters.Length - 1; i++)
                        {
                            comm.Parameters.Add(parameters[i]);
                        }
                        rtn = comm.ExecuteNonQuery();
                    }
                    conn.Close();
                }
                return rtn;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 执行事务返回执行成功数
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="list">执行事务</param>
        /// <returns>返回执行成功数</returns>
        public static int ExecuteSqlTran(List<string> list)
        {
            int num = 0;
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.Transaction = trans;
                    try
                    {
                        //循环
                        for (int i = 0; i < list.Count; i++)
                        {
                            cmd.CommandText = list[i];
                            cmd.ExecuteNonQuery();
                            num++;
                        }
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        num = 0;
                        trans.Rollback();
                        throw ex;
                    }
                }
                conn.Close();
                return num;
            }
        }
    }
}