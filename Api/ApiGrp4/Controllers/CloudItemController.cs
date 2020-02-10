using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ApiGrp4.Models;
using System.Text;
using Microsoft.AspNetCore.Cors;

namespace ApiGrp4.Controllers
{
[Route("api/[controller]")]
    [ApiController]
    public class CloudItemsController : ControllerBase
    {
        private const string _connectionString ="DefaultEndpointsProtocol=https;AccountName=ghalemelbarakapetit;AccountKey=gmnBmE6y4VQ5K4LOxnovsL2DdrlcDdFkLsAjwefugCvZv1H1jOrZQjRIyV8IzaO7ApXluxU8m6kSdWkSr7JltA==;EndpointSuffix=core.windows.net";
        CloudItem cloudItem;
        Queue queueMessage;
        private MySqlDatabase MySqlDatabase { get; set; }

        public CloudItemsController(MySqlDatabase mySqlDatabase)
        {
            this.MySqlDatabase = mySqlDatabase;
            queueMessage  = new Queue(_connectionString);
            isQueueFull(queueMessage);
        }

        public async void isQueueFull(Queue queueMessage) {
            string msg; 
            CloudItem cloudItemInsert;
            await MySqlDatabase.Connection.OpenAsync();
           do{
                msg = queueMessage.ReceiveMessageAsync().Result;
                if(msg.Equals("The queue is empty!") || msg.Equals("The queue does not exist.")) break;
                cloudItemInsert = initCloudItem(msg);
                Console.WriteLine(cloudItemInsert.name+" ET "+ cloudItemInsert.date +"queue -> BDD");
                cloudItemInsert.database = MySqlDatabase;
                await cloudItemInsert.InsertAsync();
            } while( !msg.Equals("The queue is empty!") || !msg.Equals("The queue does not exist."));
        
            await MySqlDatabase.Connection.CloseAsync();
        }

        public CloudItem initCloudItem(string msg) {
            using (var cmd = MySqlDatabase.Connection.CreateCommand()){

                StringBuilder sbItem = new StringBuilder("");
                StringBuilder sbDate = new StringBuilder("");
                int count = msg.IndexOf(',')+1;

                CloudItem cloudItem = new CloudItem();

                for (int i = 0; i < msg.Length; i++)
                {
                    if( msg[i] == ','){
                        for (int j = count; j < msg.Length; j++)
                        {
                            sbDate.Append(msg[j]);
                        }
                        i = msg.Length;
                    } 
                    else {
                        sbItem.Append(msg[i]);
                    }  
                }

                cloudItem.cloudId = (int) cmd.LastInsertedId;
                cloudItem.name = sbItem.ToString();
                cloudItem.date = sbDate.ToString();

                return cloudItem;

            }             
        }
        

        // GET: api/CloudItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult> GetOne(int id)
        {
            await MySqlDatabase.Connection.OpenAsync();
            var query = new CloudItemQuery(MySqlDatabase);
            var result = await query.FindOneAsync(id);
            await MySqlDatabase.Connection.CloseAsync();

            if (result is null)
                return new NotFoundResult();
            return new OkObjectResult(result);
        }

        // GET: api/CloudItems
        [HttpGet]
        public async Task<IActionResult> GetLatest()
        {
            await MySqlDatabase.Connection.OpenAsync();
            var query = new CloudItemQuery(MySqlDatabase);
            var result = await query.LatestPostsAsync();   
            return new OkObjectResult(result);
        }    
        
        // POST: api/CloudItems
        [HttpPost]
        public async Task<ActionResult> PostCloudItem([FromBody] CloudItem _cloudItem)
        {
            await MySqlDatabase.Connection.OpenAsync();
            _cloudItem.database = MySqlDatabase;
            await _cloudItem.InsertAsync();
            await MySqlDatabase.Connection.CloseAsync();     
            return new OkObjectResult(cloudItem);
        }

        // DELETE: api/CloudItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOne(int id)
        {
            await MySqlDatabase.Connection.OpenAsync();
            var query = new CloudItemQuery(MySqlDatabase);
            var result = await query.FindOneAsync(id);
            if (result is null)
                return new NotFoundResult();
            await result.DeleteAsync();
            await MySqlDatabase.Connection.CloseAsync();     

            return new OkResult();
        }

    }

}
