using Harvzor.FromCookieAttribute;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddMvc(options =>
{
    options.ValueProviderFactories.Add(new CookieValueProviderFactory());
});

builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();

app.MapGet("/minimal", (
    HttpContext context,
    [FromQuery] string query,
    // Does not work with minimal APIs.
    [FromCookie(Name = "Cookie")] string cookie
    ) => {
        context.Response.Cookies.Append(
            "cookie",
            "value",
            new CookieOptions
            {
                MaxAge = new TimeSpan(1000, 0, 0)
            }
        );
        
        return Results.Json(new ResponseDto
        {
            Query = query,
            Cookies =
            [
                new CookieDto
                {
                    Name = "Cookie",
                    Value = cookie
                }
            ]
        }); 
    });

app.Run();

[ApiController]
public class TestController : ControllerBase
{
    [HttpGet("/test")]
    public ActionResult<ResponseDto> Get([FromQuery] string query, [FromCookie(Name = "Cookie")] string cookie)
    {
        return new ResponseDto
        {
            Query = query,
            Cookies =
            [
                new CookieDto
                {
                    Name = "Cookie",
                    Value = cookie
                }
            ]
        };
    }
}

public record CookieDto
{
    public required string Name { get; set; }
    public required string Value { get; set; }
}

public record ResponseDto
{
    public required string Query { get; set; }
    public required IEnumerable<CookieDto> Cookies { get; set; }
}
