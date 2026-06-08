using Microsoft.EntityFrameworkCore;
using RouteXY.Api.Data;
using RouteXY.Api.Entities;
using RouteXY.Api.Requests;
using RouteXY.Api.Responses;

namespace RouteXY.Api.Services;

public class InventoryService
{
    private readonly AppDbContext _db;

    public InventoryService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<InventoryItemResponse> AddItemAsync(CreateInventoryItemRequest request)
    {
        var item = new InventoryItem
        {
            Id = Guid.NewGuid(),
            WarehouseId = request.WarehouseId,
            Name = request.Name,
            Description = request.Description,
            Quantity = request.Quantity,
            Unit = request.Unit,
            Category = request.Category,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.InventoryItems.Add(item);
        await _db.SaveChangesAsync();
        return MapToResponse(item);
    }

    public async Task<List<InventoryItemResponse>> GetInventoryByIdAsync(Guid id)
    {
        var w = await _db.Warehouses.FindAsync(id)
            ?? throw new KeyNotFoundException("Warehouse not found");
        
        return await _db.InventoryItems
            .Where(i => i.WarehouseId == id)
            .Select(i => MapToResponse(i))
            .ToListAsync();
    }

    private static InventoryItemResponse MapToResponse(InventoryItem i) => new()
    {
        Id = i.Id,
        WarehouseId = i.WarehouseId,
        Name = i.Name,
        Description = i.Description,
        Quantity = i.Quantity,
        Unit = i.Unit,
        Category = i.Category,
        UpdatedAt = i.UpdatedAt
    };
}