using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Entities;
using Play.Common;
using Play.Catalog.Contracts;

namespace Play.Catalog.Service.Controllers;

[ApiController]
[Produces("application/json")]
[Route("items")]
public class ItemsController : ControllerBase
{
    private readonly IRepository<Item> itemRepository;
    private readonly IPublishEndpoint publishEndpoint;

    public ItemsController(IRepository<Item> itemRepository,
        IPublishEndpoint publishEndpoint)
    {
        this.publishEndpoint = publishEndpoint;
        this.itemRepository = itemRepository;
    }

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
        var result = await itemRepository.GetAsync(id);

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

        await publishEndpoint.Publish(new CatalogItemCreated(item.Id, item.Name, item.Description));

        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateItem(Guid id, UpdateItemDto updateItem)
    {
        var existingItem = await itemRepository.GetAsync(id);

        if (existingItem is null)
        {
            return NotFound();
        }

        existingItem.Name = updateItem.Name;
        existingItem.Description = updateItem.Description;
        existingItem.Price = updateItem.Price;

        await itemRepository.UpdateAsync(existingItem);

        await publishEndpoint.Publish(new CatalogItemUpdated(existingItem.Id, existingItem.Name, existingItem.Description));

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteItem(Guid id)
    {
        var existingItem = await itemRepository.GetAsync(id);

        if (existingItem is null)
        {
            return NotFound();
        }

        await itemRepository.RemoveAsync(id);

        await publishEndpoint.Publish(new CatalogItemDeleted(existingItem.Id));

        return NoContent();
    }
}
