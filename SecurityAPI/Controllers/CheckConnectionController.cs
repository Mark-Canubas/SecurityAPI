using SecurityAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SecurityAPI.Controllers;

[ApiController]
[Route("Connection")]
public class CheckConnectionController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        bool dbOk;
        try
        {
            dbOk = await db.Database.CanConnectAsync();
        }
        catch
        {
            dbOk = false;
        }

        return Ok(new
        {
            status = "ok",
            dbConnected = dbOk,
            serverTimeUtc = DateTime.UtcNow
        });
    }
}