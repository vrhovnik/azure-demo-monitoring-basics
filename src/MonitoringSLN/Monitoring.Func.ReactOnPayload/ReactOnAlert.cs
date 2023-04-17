using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Monitoring.Func.ReactOnPayload;

public class ReactOnAlert
{
    [FunctionName("ReactOnAlert")]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]
        HttpRequest req, ILogger log)
    {
        log.LogInformation("Received Payload");

        using StreamReader streamReader = new StreamReader(req.Body);
        var requestBody = await streamReader.ReadToEndAsync();

        if (string.IsNullOrEmpty(requestBody))
        {
            log.LogError("Body is empty");
            return new BadRequestResult();
        }

        log.LogInformation("Body: {RequestBody}", requestBody);

        var alertPayload = JsonConvert.DeserializeObject<AlertPayload>(requestBody);

        log.LogInformation("Description: {AlertDescription}", alertPayload.data.essentials.description);

        return new OkObjectResult("Received body and perform an action");
    }
}