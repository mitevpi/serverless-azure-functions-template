using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AzureFunctions
{
    public static class SayHello
    {
        [FunctionName("SayHello")]
        // https://testazurefunctionspm.azurewebsites.net/api/sayhello?name=Peter
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            string name = await GreetingUtils.GetName(req);
            return name != null
                ? (ActionResult) new OkObjectResult($"Hello, {name}")
                : GreetingUtils.Reject();
        }
    }

    public static class SayHi
    {
        [FunctionName("SayHi")]
        // https://testazurefunctionspm.azurewebsites.net/api/sayhi?name=Peter
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            string name = await GreetingUtils.GetName(req);
            return name != null
                ? (ActionResult) new OkObjectResult($"Hi, {name}")
                : GreetingUtils.Reject();
        }
    }

    public static class GreetingUtils
    {
        public static async Task<string> GetName(HttpRequest req)
        {
            string name = req.Query["name"]; // get from query var
            if (name != null) return name; // return if found

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync(); // get from body
            dynamic data = JsonConvert.DeserializeObject(requestBody); // parse
            name = data?.name;

            return name;
        }

        public static BadRequestObjectResult Reject()
        {
            return new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }
    }
}