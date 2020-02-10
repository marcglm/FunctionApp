using System;
using MySql.Data.MySqlClient;

namespace ApiGrp4.Models
{
public class MySqlDatabase : IDisposable
  {
    public MySqlConnection Connection{get;}

    public MySqlDatabase(string connectionString)
    {
      Connection = new MySqlConnection(connectionString);
    }

    public void Dispose() => Connection.Dispose();
    
  }

}