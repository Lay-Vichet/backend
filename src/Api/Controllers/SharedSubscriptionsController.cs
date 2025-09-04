using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using SubscriptionTracker.Application.DTOs;
using SubscriptionTracker.Application.Services;

namespace SubscriptionTracker.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SharedSubscriptionsController : ControllerBase
    {
        private readonly SharedSubscriptionService _service;

        public SharedSubscriptionsController(SharedSubscriptionService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("{subscriptionId:guid}/{householdId:guid}")]
        public async Task<IActionResult> GetById(Guid subscriptionId, Guid householdId)
        {
            var item = await _service.GetByIdAsync(subscriptionId, householdId);
            if (item is null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SharedSubscriptionDto dto)
        {
            await _service.AddAsync(dto);
            return Created("", dto);
        }

        [HttpPut("{subscriptionId:guid}/{householdId:guid}")]
        public async Task<IActionResult> Update(Guid subscriptionId, Guid householdId, [FromBody] SharedSubscriptionDto dto)
        {
            var updated = dto with { SubscriptionId = subscriptionId, HouseholdId = householdId };
            await _service.UpdateAsync(updated);
            return NoContent();
        }

        [HttpDelete("{subscriptionId:guid}/{householdId:guid}")]
        public async Task<IActionResult> Delete(Guid subscriptionId, Guid householdId)
        {
            await _service.DeleteAsync(subscriptionId, householdId);
            return NoContent();
        }
    }
}
