using Microsoft.EntityFrameworkCore;
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

    public async Task<WarehouseResponse> AddWarehouseAsync(CreateWarehouseRequest request)
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

        return MapToResponse(warehouse);
    }

    public async Task<List<WarehouseResponse>> GetAllAsync()
    {
        var warehouses = await _db.Warehouses
            .Select(w => MapToResponse(w)).ToListAsync();
        
        return warehouses;
    }

    public async Task<WarehouseResponse?> GetByIdAsync(Guid id)
    {
        var warehouse = await _db.Warehouses.FindAsync(id);
        
        return warehouse == null ? null : MapToResponse(warehouse);
    }

    public async Task UpdateWarehouseAsync(Guid id, UpdateWarehouseRequest request)
    {
        var warehouse = await _db.Warehouses.FindAsync(id)
            ?? throw new KeyNotFoundException("Warehouse not found");
        
        if (request.Name != null) warehouse.Name = request.Name;
        if (request.Address != null) warehouse.Address = request.Address;
        if (request.City != null) warehouse.City = request.City;
        if (request.Latitude != null) warehouse.Latitude = request.Latitude;
        if (request.Longitude != null) warehouse.Longitude = request.Longitude;
        if (request.IsActive != null) warehouse.IsActive = (bool)request.IsActive;

        await _db.SaveChangesAsync();
    }

    public async Task DeleteWarehouseAsync(Guid id)
    {
        var warehouse = await _db.Warehouses.FindAsync(id)
            ?? throw new KeyNotFoundException("Warehouse not found");
        
        _db.Remove(warehouse);
        await _db.SaveChangesAsync();
    }

    public async Task<InventoryItemResponse> AddInventoryItemAsync(CreateInventoryItemRequest request)
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

    private static WarehouseResponse MapToResponse(Warehouse w) => new()
    {
        Id = w.Id,
        Name = w.Name,
        Address = w.Address,
        City = w.City,
        Latitude = w.Latitude,
        Longitude = w.Longitude,
        IsActive = w.IsActive,
        CreatedAt = w.CreatedAt
    };
}