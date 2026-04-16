using System;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

public class DeclineRequest
{
    private readonly ILogger<DeclineRequest> _logger;

    public DeclineRequest(ILogger<DeclineRequest> logger)
    {
        _logger = logger;
    }

    [Function("decline")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
    {
        var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
        string id = query["id"];

        if (string.IsNullOrEmpty(id))
        {
            var bad = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
            await bad.WriteStringAsync("Missing id parameter.");
            return bad;
        }

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

        var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
        await response.WriteStringAsync("Request declined.");
        return response;
    }
}
