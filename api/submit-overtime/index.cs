using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;

public static class SubmitOvertime
{
    public static List<OvertimeRequest> Requests = new List<OvertimeRequest>();

    [FunctionName("SubmitOvertime")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "submit-overtime")] HttpRequest req,
        ILogger log)
    {
        string body = await new StreamReader(req.Body).ReadToEndAsync();
        log.LogInformation($"Received body: {body}");

        if (string.IsNullOrWhiteSpace(body))
            return new BadRequestObjectResult("Request body is empty.");

        OvertimeRequest data;
        try
        {
            data = JsonConvert.DeserializeObject<OvertimeRequest>(body);
        }
        catch (Exception ex)
        {
            log.LogError($"JSON parse error: {ex.Message}");
            return new BadRequestObjectResult("Invalid JSON format.");
        }

        if (data == null || string.IsNullOrWhiteSpace(data.date) ||
            string.IsNullOrWhiteSpace(data.hours) || string.IsNullOrWhiteSpace(data.reason))
        {
            return new BadRequestObjectResult("Missing required fields: date, hours, reason.");
        }

        data.id = Guid.NewGuid().ToString();
        data.status = "pending";

        Requests.Add(data);

        log.LogInformation($"Stored overtime request ID: {data.id}");

        return new OkObjectResult("Overtime request submitted.");
    }
}

public class OvertimeRequest
{
    public string id { get; set; }
    public string date { get; set; }
    public string hours { get; set; }
    public string reason { get; set; }
    public string status { get; set; }
}
