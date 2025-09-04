using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using SubscriptionTracker.Application.DTOs;
using SubscriptionTracker.Application.Services;

namespace SubscriptionTracker.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubscriptionCategoriesController : ControllerBase
    {
        private readonly SubscriptionCategoryService _service;

        public SubscriptionCategoriesController(SubscriptionCategoryService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("{subscriptionId:guid}/{categoryId:guid}")]
        public async Task<IActionResult> GetById(Guid subscriptionId, Guid categoryId)
        {
            var item = await _service.GetByIdAsync(subscriptionId, categoryId);
            if (item is null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SubscriptionCategoryDto dto)
        {
            await _service.AddAsync(dto);
            return Created("", dto);
        }

        [HttpPut("{subscriptionId:guid}/{categoryId:guid}")]
        public async Task<IActionResult> Update(Guid subscriptionId, Guid categoryId, [FromBody] SubscriptionCategoryDto dto)
        {
            var updated = dto with { SubscriptionId = subscriptionId, CategoryId = categoryId };
            await _service.UpdateAsync(updated);
            return NoContent();
        }

        [HttpDelete("{subscriptionId:guid}/{categoryId:guid}")]
        public async Task<IActionResult> Delete(Guid subscriptionId, Guid categoryId)
        {
            await _service.DeleteAsync(subscriptionId, categoryId);
            return NoContent();
        }
    }
}
