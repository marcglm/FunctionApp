using System;
using System.Data;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace ApiGrp4.Models
{
public class CloudItem
    {
        public long cloudId { get; set; }
        public string name { get; set; }
        public string date { get; set; }
        internal MySqlDatabase database {get;set;}

        public CloudItem(){

        }
 
        public CloudItem(MySqlDatabase db){
            database = db;
        }

        public async Task InsertAsync()
        {
            using (var cmd = database.Connection.CreateCommand()){
                cmd.CommandText = @"INSERT INTO `listecourse` (`name`, `date`) VALUES (@name, @date);";
                BindParams(cmd);
                await cmd.ExecuteNonQueryAsync();
                cloudId = (int) cmd.LastInsertedId;
            }

        }

        public async Task UpdateAsync()
        {
            using (var cmd = database.Connection.CreateCommand()){
                cmd.CommandText = @"UPDATE `listecourse` SET `name` = @name, `date` = @date WHERE `cloudId` = @cloudId;";
                BindParams(cmd);
                BindId(cmd);
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteAsync()
        {
            using (var cmd = database.Connection.CreateCommand()){
                cmd.CommandText = @"DELETE FROM `listecourse` WHERE `cloudId` = @cloudId;";
                BindId(cmd);
                await cmd.ExecuteNonQueryAsync();
            }   
        }

        private void BindId(MySqlCommand cmd)
        {
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@cloudId",
                DbType = DbType.Int32,
                Value = cloudId,
            });
        }

        private void BindParams(MySqlCommand cmd)
        {
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@name",
                DbType = DbType.String,
                Value = name,
            });
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@date",
                DbType = DbType.String,
                Value = date,
            });
        }      
    }

}