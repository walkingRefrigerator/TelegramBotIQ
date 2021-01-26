using System.Data;
using System.Data.SqlClient;

namespace TelegramBotIQ
{
    internal class DB
    {

        private readonly string conStr = $@"Data Source=localhost\SQLEXPRESS; 
DataBase=BotDB; User ID=*; Password=*";

        public DataTable AutoSQL(UserDB userDB)
        {
            using (var connection = new SqlConnection(conStr))
            {
                var table = new DataTable();

                connection.Open();

                table = ExecutionCommandsSQL(connection, userDB);

                return table;
            }

        }

        public void RegSQL(UserDB userDB)
        {
            using (var connection = new SqlConnection(conStr))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand($"INSERT INTO Table_1 (UserName, Password) VALUES ('{userDB.GetUser()}', '{userDB.GetPass()}')", connection);

                int rowAffected = cmd.ExecuteNonQuery();
            }
        }

        private DataTable ExecutionCommandsSQL(SqlConnection connection, UserDB userDB)
        {
            var table = new DataTable();

            var cmd = new SqlCommand($"SELECT * FROM Table_1 WHERE UserName = '{userDB.GetUser()}' AND Password = '{userDB.GetPass()}' ", connection);

            var reader = cmd.ExecuteReader();

            table.Load(reader);

            return table;
        }

        public DB() { }
    }
}
