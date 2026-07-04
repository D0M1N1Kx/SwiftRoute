using Microsoft.EntityFrameworkCore;
using RouteXY.Api.Data;

namespace RouteXY.Api.Services;

public class StatusService
{
    private readonly AppDbContext _db;
    
    public StatusService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IResult> GetWorkerCountsAsRoles()
    {
        var users = await _db.Users.AnyAsync();
        if (!users) return Results.NotFound();

        var roleCounts = await _db.Users
            .GroupBy(u => u.Role)
            .Select(group => new
            {
                Role = group.Key.ToString(),
                Count = group.Count()
            })
            .ToListAsync();
        
        return Results.Ok(roleCounts);
    }
}