using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace ApiGrp4.Models
{
public class CloudItemQuery
  {
    public MySqlDatabase Db { get;}
    public CloudItemQuery(MySqlDatabase db)
    {
        Db = db;
    }
    public async Task<CloudItem> FindOneAsync(int id)
    {
        using (var cmd = Db.Connection.CreateCommand()){
          cmd.CommandText = @"SELECT `cloudId`, `name`, `date` FROM `listecourse` WHERE `cloudId` = @cloudId";
          cmd.Parameters.Add(new MySqlParameter
          {
            ParameterName = "@cloudId",
            DbType = DbType.Int32,
            Value = id,
          });
          var result = await ReadAllAsync(await cmd.ExecuteReaderAsync());
          return result.Count > 0 ? result[0] : null;
        }   
    }

    public async Task<List<CloudItem>> LatestPostsAsync()
    {
        using (var cmd = Db.Connection.CreateCommand()){
            cmd.CommandText = @"SELECT `cloudId`, `name`, `date` FROM `listecourse` ORDER BY `cloudId` DESC LIMIT 10;";
            return await ReadAllAsync(await cmd.ExecuteReaderAsync());
        }
    }

    public async Task DeleteAllAsync()
    {
        using (var txn = await Db.Connection.BeginTransactionAsync()){
            using (var cmd = Db.Connection.CreateCommand()){
                cmd.CommandText = @"DELETE FROM `listecourse`";
                await cmd.ExecuteNonQueryAsync();
                await txn.CommitAsync();
            }
        }
    }

    private async Task<List<CloudItem>> ReadAllAsync(DbDataReader reader)
    {
        var posts = new List<CloudItem>();
        using (reader)
        {
            while (await reader.ReadAsync())
            {
                var post = new CloudItem(Db)
                {
                    cloudId = reader.GetInt32(0),
                    name = reader.GetString(1),
                    date = reader.GetString(2),
                };
                posts.Add(post);
            }
        }
        return posts;
    }


  }

}