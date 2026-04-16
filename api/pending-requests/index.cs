using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

public class PendingRequests
{
    private readonly ILogger<PendingRequests> _logger;

    public PendingRequests(ILogger<PendingRequests> logger)
    {
        _logger = logger;
    }

    [Function("pending-requests")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
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

        var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
        await response.WriteStringAsync(JsonConvert.SerializeObject(results));
        return response;
    }
} 
