using RouteXY.Api.Data;
using RouteXY.Api.Entities;
using RouteXY.Api.Requests;
using RouteXY.Api.Responses;

namespace RouteXY.Api.Services;

public class WarehouseService
{
    private readonly AppDbContext _db;

    public WarehouseService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<WarehouseResponse> AddWarehouse(CreateWarehouseRequest request)
    {
        var warehouse = new Warehouse
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Address = request.Address,
            City = request.City,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _db.Warehouses.Add(warehouse);
        await _db.SaveChangesAsync();

        return new WarehouseResponse
        {
            Id = warehouse.Id,
            Name = warehouse.Name,
            Address = warehouse.Address,
            City = warehouse.City,
            Latitude = warehouse.Latitude,
            Longitude = warehouse.Longitude,
            IsActive = warehouse.IsActive,
            CreatedAt = warehouse.CreatedAt
        };
    }

    public async Task<InventoryItemResponse> AddInventoryItem(CreateInventoryItemRequest request)
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