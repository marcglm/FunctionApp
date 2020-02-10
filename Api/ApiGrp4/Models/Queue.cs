using System.Threading.Tasks;
using System.Text;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Queue;

namespace ApiGrp4.Models
{
    public class Queue
    {
        public CloudStorageAccount _storageAccount { get; set; }
        public CloudQueueClient _queueClient {get; set;}
        public CloudQueue _queue{get;set;}

        public Queue(string connectionString){
            _storageAccount = CloudStorageAccount.Parse(connectionString);
            _queueClient = _storageAccount.CreateCloudQueueClient();
            _queue = _queueClient.GetQueueReference("outqueue");
       }

       public Queue(){
            string connectionString ="DefaultEndpointsProtocol=https;AccountName=ghalemelbarakapetit;AccountKey=gmnBmE6y4VQ5K4LOxnovsL2DdrlcDdFkLsAjwefugCvZv1H1jOrZQjRIyV8IzaO7ApXluxU8m6kSdWkSr7JltA==;EndpointSuffix=core.windows.net";
            _storageAccount = CloudStorageAccount.Parse(connectionString);
            _queueClient = _storageAccount.CreateCloudQueueClient();
            _queue = _queueClient.GetQueueReference("myStorageQueue");
       } 


        /*
         Cette méthode reçoit un message de la queue.
         After the message is received, delete it 
         from the queue by calling DeleteMessageAsync.
        */
       public async Task<string> ReceiveMessageAsync()
        {
            bool exists = await _queue.ExistsAsync();
            if (exists)
            {
                CloudQueueMessage retrievedMessage = await _queue.GetMessageAsync();
               

                if (retrievedMessage != null)
                {
                    string theMessage = retrievedMessage.AsString;
                    
                    await _queue.DeleteMessageAsync(retrievedMessage);
                    return theMessage;
                }
                else
                {
                    return"The queue is empty!";
                }
            }
            else
            {
                return "The queue does not exist.";
            }
        }
    }


}