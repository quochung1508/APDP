using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIMS.DatabaseContext;

public class DebugController : Controller
{
    private readonly SimDbContext _db;
    public DebugController(SimDbContext db) => _db = db;
    public IActionResult WhichDb() => Content(_db.Database.GetDbConnection().ConnectionString);
}