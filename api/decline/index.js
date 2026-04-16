using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;

public static class DeclineRequest
{
    [FunctionName("decline")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
    {
        string id = req.Query["id"];
        if (string.IsNullOrEmpty(id))
            return new BadRequestObjectResult("Missing id parameter.");

        string connString = Environment.GetEnvironmentVariable("SqlConnectionString");

        using (SqlConnection conn = new SqlConnection(connString))
        {
            await conn.OpenAsync();

            string sql = @"
                UPDATE OvertimeRequests
                SET Status = 'Declined'
                WHERE Id = @Id
            ";

            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                await cmd.ExecuteNonQueryAsync();
            }
        }

        return new OkObjectResult("Request declined.");
    }
}
