using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.Dtos;

namespace Play.Catalog.Service.Controllers;

[ApiController]
[Produces("application/json")]
[Route("items")]
public class ItemsController : ControllerBase
{
    public ItemsController()
    {
    }

    private static readonly List<ItemDto> items = new()
    {
        new ItemDto(Guid.NewGuid(), "Potion", "Restored small amount of HP", 5, DateTimeOffset.Now),
        new ItemDto(Guid.NewGuid(), "Antidote", "Cures potion", 7, DateTimeOffset.UtcNow),
        new ItemDto(Guid.NewGuid(), "Bronze sword", "Deals a small amout of damange", 5, DateTimeOffset.UtcNow),
    };

    [HttpGet]
    public IEnumerable<ItemDto> Get()
    {
        return items;
    }

    [HttpGet("{id}")]
    public ActionResult<ItemDto> GetById(Guid id)
    {
        var result = items.FirstOrDefault(i => i.Id == id);

        if(result is null){
            return NotFound();
        }

        return result;
    }

    [HttpPost]
    public ActionResult<ItemDto> CreateItem(CreateItemDto createItemDto)
    {
        var item = new ItemDto(Guid.NewGuid(), createItemDto.Name, createItemDto.Description,
        createItemDto.Price, DateTimeOffset.UtcNow);
        items.Add(item);

        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateItem(Guid id, UpdateItemDto updateItem)
    {
        var existingItem = items.SingleOrDefault(item => item.Id == id);

        var updatedItem = existingItem! with
        {
            Name = updateItem.Name,
            Description = updateItem.Description,
            Price = updateItem.Price
        };

        var index = items.FindIndex(existingItem => existingItem.Id == id);
        items[index] = updatedItem;

        return NoContent();
    }

    [HttpDelete("{id}")]
       public IActionResult DeleteItem(Guid id)
    {
        var existingItem = items.SingleOrDefault(item => item.Id == id);

        int indexToDelete = items.FindIndex(i => i.Id == id);

        items.RemoveAt(indexToDelete);

        return NoContent();
    }
}
