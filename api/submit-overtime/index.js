using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

public static class SubmitOvertime
{
    [FunctionName("submit-overtime")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
    {
        string requestBody = await req.ReadAsStringAsync();
        dynamic data = JsonConvert.DeserializeObject(requestBody);

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

        return new OkObjectResult("Overtime request submitted.");
    }
}
