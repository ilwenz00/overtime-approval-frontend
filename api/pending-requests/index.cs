using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;

public static class PendingRequests
{
    [FunctionName("PendingRequests")]
    public static IActionResult Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "pending-requests")] HttpRequest req)
    {
        var pending = SubmitOvertime.Requests.Where(r => r.status == "pending").ToList();
        return new OkObjectResult(pending);
    }
}
