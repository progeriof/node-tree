using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
public class ErrorController : ControllerBase
{
    private ApplicationDbContext dbContext;

    public ErrorController(ApplicationDbContext _dbContext)
    {
        this.dbContext = _dbContext;
    }

    [HttpGet("/error")]
    [HttpPost("/error")]
    public async Task<IActionResult> Error()
    {
        var context = HttpContext.Features.Get<IExceptionHandlerFeature>();

        string queryParams = JsonConvert.SerializeObject(HttpContext.Request.Query.AsEnumerable().ToDictionary(x => x.Key, x => x.Value.FirstOrDefault()));
        string bodyParams = string.Empty;
        try
        {
            bodyParams = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        }
        catch (System.Exception)
        {
            //
        }

        var exception = context?.Error;

        string exceptionType = exception?.GetType().Name ?? "Internal Server Error";
        string errorMessage = exception?.Message ?? "An unknown error has occurred";

        var eventId = DateTime.Now.Ticks;

        dbContext.Journals.Add(new Journal
        {
            eventId = eventId,
            text = $"{exceptionType} - {errorMessage} ID = {eventId}",
            queryParams = queryParams,
            bodyParams = bodyParams,
            stackTrace = exception?.StackTrace ?? string.Empty,
            createdAt = DateTime.UtcNow
        });
        await dbContext.SaveChangesAsync();

        var response = new
        {
            type = exceptionType,
            id = eventId,
            data = new
            {
                message = errorMessage
            }
        };

        return StatusCode(StatusCodes.Status500InternalServerError, response);
    }
}