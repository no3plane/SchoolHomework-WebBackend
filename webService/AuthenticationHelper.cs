using System;
using System.Data;
using System.Linq;

namespace webService
{

    class AuthenticationHelper
    {
        public static User GetUser(string token)
        {
            DataTable userTable = SqlHelper.ExecuteDataTable($"SELECT * FROM [user] WHERE token = '{token}'");
            if (userTable.Rows.Count == 0)
            {
                return null;
            }
            User[] users = userTable.AsEnumerable().Select(SqlMapperCollection.user).ToArray();
            return users[0];
        }

        public static User GetUserById(int id)
        {
            var userTable = SqlHelper.ExecuteDataTable(SqlQueryCollection.GET_USERS_BY_ID(id));
            User[] users = userTable.AsEnumerable().Select(SqlMapperCollection.user).ToArray();
            if (users.Length == 0)
            {
                return null;
            }
            return users[0];
        }
    }
}