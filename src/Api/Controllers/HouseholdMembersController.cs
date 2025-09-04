using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using SubscriptionTracker.Application.DTOs;
using SubscriptionTracker.Application.Services;

namespace SubscriptionTracker.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HouseholdMembersController : ControllerBase
    {
        private readonly HouseholdMemberService _service;

        public HouseholdMembersController(HouseholdMemberService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("{householdId:guid}/{userId:guid}")]
        public async Task<IActionResult> GetById(Guid householdId, Guid userId)
        {
            var item = await _service.GetByIdAsync(householdId, userId);
            if (item is null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] HouseholdMemberDto dto)
        {
            await _service.AddAsync(dto);
            return Created("", dto);
        }

        [HttpPut("{householdId:guid}/{userId:guid}")]
        public async Task<IActionResult> Update(Guid householdId, Guid userId, [FromBody] HouseholdMemberDto dto)
        {
            var updated = dto with { HouseholdId = householdId, UserId = userId };
            await _service.UpdateAsync(updated);
            return NoContent();
        }

        [HttpDelete("{householdId:guid}/{userId:guid}")]
        public async Task<IActionResult> Delete(Guid householdId, Guid userId)
        {
            await _service.DeleteAsync(householdId, userId);
            return NoContent();
        }
    }
}
