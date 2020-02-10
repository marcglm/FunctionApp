using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Extensions.Storage;
using System.Net.Http;  

namespace Function
{
    public static class HttpTriggerGrp4
    {
        [FunctionName("HttpTriggerGrp4")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [Queue("outqueue"),StorageAccount("AzureWebJobsStorage")] ICollector<string> msg,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            DateTime thisDay = DateTime.Now;

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            log.LogInformation(requestBody);
 
            var data = (CloudItem) JsonConvert.DeserializeObject<CloudItem>(requestBody);
            
            // Add a message to the output collection.
            msg.Add(string.Format("{0},{1}", data.name, data.date));
            

            return data != null
                ? (ActionResult)new OkObjectResult($"{data.name} ajouté a la date : {data.date}")
                : new BadRequestObjectResult("Veuillez rentrer un item dans votre liste de courses");
        }

    }

}
