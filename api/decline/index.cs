using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

public static class Decline
{
    [FunctionName("Decline")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "decline")] HttpRequest req)
    {
        string body = await new StreamReader(req.Body).ReadToEndAsync();
        dynamic data = JsonConvert.DeserializeObject(body);
        string id = data.id;

        var reqItem = SubmitOvertime.Requests.FirstOrDefault(r => r.id == id);
        if (reqItem == null) return new NotFoundObjectResult("Not found");

        reqItem.status = "declined";
        return new OkObjectResult("Declined");
    }
}
