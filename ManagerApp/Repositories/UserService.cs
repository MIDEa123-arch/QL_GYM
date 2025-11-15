using ManagerApp.Helpers;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.EnterpriseServices;
using System.Linq;
using System.Web;

namespace ManagerApp.Repositories
{
    public class UserService
    {
        private readonly string _baseConnectionString;
        private string _connectionStringUser;
        private readonly string _EntityConnectionString = ConfigurationManager.ConnectionStrings["QL_PHONGGYM"].ConnectionString;
        public UserService(string connectionStringName)
        {
            _baseConnectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;            
        }

        public string ConnectionStringUser => _EntityConnectionString;

        public bool CheckOracleSession(string username)
        {
            try
            {
                using (var conn = new OracleConnection(_baseConnectionString))
                using (var cmd = new OracleCommand("SP_CHECK_SESSION", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("p_username", OracleDbType.Varchar2).Value = username.ToUpper();
                    cmd.Parameters.Add("p_result", OracleDbType.Int32).Direction = ParameterDirection.Output;

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    var oracleResult = cmd.Parameters["p_result"].Value as OracleDecimal?;
                    int result = oracleResult.HasValue ? oracleResult.Value.ToInt32() : 0;
                    return result == 1;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public bool Logout(string username)
        {
            try
            {
                using (var conn = new OracleConnection(_baseConnectionString)) 
                using (var cmd = new OracleCommand("SP_LOGOUT_USER", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_username", OracleDbType.Varchar2).Value = username.ToUpper();

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    return true; 
                }
            }
            catch 
            {
                return false;
            }
        }

        public bool Login(string username, string password)
        {
            password = MaHoa.MaHoaNhan(password, 7);
            password = MaHoa.MaHoaNhan(password, 7);
            try
            {
                var builder = new OracleConnectionStringBuilder(_baseConnectionString)
                {
                    UserID = username.ToUpper(),
                    Password = password
                };
                _connectionStringUser = builder.ConnectionString;

                using (var connUser = new OracleConnection(_connectionStringUser))
                {
                    connUser.Open(); 
                }

                try
                {
                    using (var connAdmin = new OracleConnection(_EntityConnectionString))
                    using (var cmd = new OracleCommand("SP_MANAGE_SESSIONS", connAdmin))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("p_username", OracleDbType.Varchar2).Value = username.ToUpper();
                        cmd.Parameters.Add("p_max_sessions", OracleDbType.Int32).Value = 1;

                        connAdmin.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception mgmtEx)
                {
                    System.Diagnostics.Debug.WriteLine("Lỗi quản lý session khi login: " + mgmtEx.Message);
                }

                return true; 
            }
            catch (OracleException ex)
            {
                if (ex.Number == 1017)
                {
                    return false;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }       

    }

}