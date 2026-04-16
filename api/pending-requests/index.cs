using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

public static class PendingRequests
{
    [FunctionName("pending-requests")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
    {
        string connString = Environment.GetEnvironmentVariable("SqlConnectionString");
        var results = new List<object>();

        using (SqlConnection conn = new SqlConnection(connString))
        {
            await conn.OpenAsync();

            string sql = @"
                SELECT Id, DateSubmitted, Hours, Reason, Status
                FROM OvertimeRequests
                WHERE Status = 'Pending'
                ORDER BY DateSubmitted DESC
            ";

            using (SqlCommand cmd = new SqlCommand(sql, conn))
            using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    results.Add(new
                    {
                        Id = reader["Id"],
                        DateSubmitted = reader["DateSubmitted"],
                        Hours = reader["Hours"],
                        Reason = reader["Reason"],
                        Status = reader["Status"]
                    });
                }
            }
        }

        return new OkObjectResult(results);
    }
}
