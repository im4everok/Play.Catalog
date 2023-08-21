using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Entities;
using Play.Catalog.Service.Repositories;

namespace Play.Catalog.Service.Controllers;

[ApiController]
[Produces("application/json")]
[Route("items")]
public class ItemsController : ControllerBase
{
    private readonly IRepository<Item> itemRepository;

    public ItemsController(IRepository<Item> itemRepository)
    {
        this.itemRepository = itemRepository;
    }

    private static readonly List<ItemDto> items = new()
    {
        new ItemDto(Guid.NewGuid(), "Potion", "Restored small amount of HP", 5, DateTimeOffset.Now),
        new ItemDto(Guid.NewGuid(), "Antidote", "Cures potion", 7, DateTimeOffset.UtcNow),
        new ItemDto(Guid.NewGuid(), "Bronze sword", "Deals a small amout of damange", 5, DateTimeOffset.UtcNow),
    };

    [HttpGet]
    public async Task<IEnumerable<ItemDto>> GetAsync()
    {
        var items = (await itemRepository.GetAllAsync())
            .Select(i => i.AsDto());

        return items;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ItemDto>> GetById(Guid id)
    {
        var result = await itemRepository.GetByIdAsync(id);

        if (result is null)
        {
            return NotFound();
        }

        return result.AsDto();
    }

    [HttpPost]
    public async Task<ActionResult<ItemDto>> CreateItem(CreateItemDto createItemDto)
    {
        var item = new Item
        {
            Name = createItemDto.Name,
            Description = createItemDto.Description,
            Price = createItemDto.Price,
            CreatedDate = DateTimeOffset.UtcNow
        };

        await itemRepository.CreateAsync(item);

        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateItem(Guid id, UpdateItemDto updateItem)
    {
        var existingItem = await itemRepository.GetByIdAsync(id);

        if (existingItem is null)
        {
            return NotFound();
        }

        existingItem.Name = updateItem.Name;
        existingItem.Description = updateItem.Description;
        existingItem.Price = updateItem.Price;

        await itemRepository.UpdateAsync(existingItem);        

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteItem(Guid id)
    {
        var existingItem = await itemRepository.GetByIdAsync(id);

        if (existingItem is null)
        {
            return NotFound();
        }

        await itemRepository.RemoveAsync(id);

        return NoContent();
    }
}
