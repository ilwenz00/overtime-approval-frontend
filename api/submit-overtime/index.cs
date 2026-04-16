using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

public class SubmitOvertime
{
    private readonly ILogger<SubmitOvertime> _logger;

    public SubmitOvertime(ILogger<SubmitOvertime> logger)
    {
        _logger = logger;
    }

    [Function("submit-overtime")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
    {
        string body = await new StreamReader(req.Body).ReadToEndAsync();
        dynamic data = JsonConvert.DeserializeObject(body);

        DateTime dateSubmitted = data?.dateSubmitted;
        int hours = data?.hours;
        string reason = data?.reason;

        string connString = Environment.GetEnvironmentVariable("SqlConnectionString");

        using (SqlConnection conn = new SqlConnection(connString))
        {
            await conn.OpenAsync();

            string sql = @"
                INSERT INTO OvertimeRequests (Id, DateSubmitted, Hours, Reason, Status)
                VALUES (NEWID(), @DateSubmitted, @Hours, @Reason, 'Pending')
            ";

            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@DateSubmitted", dateSubmitted);
                cmd.Parameters.AddWithValue("@Hours", hours);
                cmd.Parameters.AddWithValue("@Reason", reason);

                await cmd.ExecuteNonQueryAsync();
            }
        }

        var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
        await response.WriteStringAsync("Overtime request submitted.");
        return response;
    }
} 
