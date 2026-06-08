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
        return new InventoryItemResponse
        {
            Id = item.Id,
            WarehouseId = item.WarehouseId,
            Name = item.Name,
            Description = item.Description,
            Quantity = item.Quantity,
            Unit = item.Unit,
            Category = item.Category
        };
    }
}