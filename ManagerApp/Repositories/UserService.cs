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

        public UserService(string connectionStringName)
        {
            _baseConnectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
        }

        public string ConnectionStringUser => _connectionStringUser;

        public bool CheckOracleSession(string username, string hashedPassword)
        {
            try
            {
                var builder = new OracleConnectionStringBuilder(_baseConnectionString)
                {
                    UserID = username.ToUpper(),
                    Password = hashedPassword,
                };

                using (var conn = new OracleConnection(builder.ConnectionString))
                using (var cmd = new OracleCommand("SP_CHECK_SESSION", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_username", OracleDbType.Varchar2).Value = username.ToUpper();
                    cmd.Parameters.Add("p_result", OracleDbType.Int32).Direction = ParameterDirection.Output;

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    int result = Convert.ToInt32(cmd.Parameters["p_result"].Value.ToString());
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

                using (var conn = new OracleConnection(_connectionStringUser))
                {
                    conn.Open();
                }

                return true; 
            }
            catch
            {
                return false;
            }
        }

    }

}