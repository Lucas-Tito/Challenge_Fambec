using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Challenge_Fambec.Server.Data;
using Challenge_Fambec.Shared.Models;

namespace Challenge_Fambec.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<WeatherForecastController> _logger;

    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    public WeatherForecastController(ApplicationDbContext context, ILogger<WeatherForecastController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WeatherForecast>>> Get()
    {
        var forecasts = await _context.WeatherForecasts.ToListAsync();
        
        if (!forecasts.Any())
        {
            // Seed data if empty
            var seedData = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            }).ToArray();

            _context.WeatherForecasts.AddRange(seedData);
            await _context.SaveChangesAsync();
            
            return Ok(seedData);
        }

        return Ok(forecasts);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WeatherForecast>> Get(int id)
    {
        var forecast = await _context.WeatherForecasts.FindAsync(id);
        
        if (forecast == null)
        {
            return NotFound();
        }

        return Ok(forecast);
    }

    [HttpPost]
    public async Task<ActionResult<WeatherForecast>> Post(WeatherForecast forecast)
    {
        _context.WeatherForecasts.Add(forecast);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = forecast.Id }, forecast);
    }
}
