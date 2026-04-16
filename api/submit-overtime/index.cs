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
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "submit-overtime")] HttpRequest req,
        ILogger log)
    {
        // Read body
        string body = await new StreamReader(req.Body).ReadToEndAsync();
        log.LogInformation($"Received body: {body}");

        if (string.IsNullOrWhiteSpace(body))
        {
            return new BadRequestObjectResult("Request body is empty.");
        }

        dynamic data;
        try
        {
            data = JsonConvert.DeserializeObject(body);
        }
        catch (Exception ex)
        {
            log.LogError($"JSON parse error: {ex.Message}");
            return new BadRequestObjectResult("Invalid JSON format.");
        }

        if (data == null || data.date == null || data.hours == null || data.reason == null)
        {
            return new BadRequestObjectResult("Missing required fields: date, hours, reason.");
        }

        var request = new
        {
            id = Guid.NewGuid().ToString(),
            date = (string)data.date,
            hours = (string)data.hours,
            reason = (string)data.reason,
            status = "pending"
        };

        Requests.Add(request);

        log.LogInformation($"Overtime request stored with ID: {request.id}");

        return new OkObjectResult("Overtime request submitted.");
    }
}
