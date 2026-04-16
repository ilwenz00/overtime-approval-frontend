using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;

public static class SubmitOvertime
{
    public static List<dynamic> Requests = new List<dynamic>();

    [FunctionName("SubmitOvertime")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "submit-overtime")] HttpRequest req)
    {
        string body = await new StreamReader(req.Body).ReadToEndAsync();
        dynamic data = JsonConvert.DeserializeObject(body);

        var request = new {
            id = Guid.NewGuid().ToString(),
            date = (string)data.date,
            hours = (string)data.hours,
            reason = (string)data.reason,
            status = "pending"
        };

        Requests.Add(request);

        return new OkObjectResult("Overtime request submitted.");
    }
}
