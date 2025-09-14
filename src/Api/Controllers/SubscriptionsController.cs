using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using SubscriptionTracker.Application.DTOs;
using SubscriptionTracker.Application.Services;
using Microsoft.AspNetCore.Authorization;

namespace SubscriptionTracker.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class SubscriptionsController : ControllerBase
    {
        private readonly ISubscriptionService _service;

        public SubscriptionsController(ISubscriptionService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var user = HttpContext.User.Claims.First();
            var items = await _service.GetAllAsync(Guid.Parse(user.Value));
            return Ok(items);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = HttpContext.User.Claims.First();
            var item = await _service.GetByIdAsync(id, Guid.Parse(user.Value));
            if (item is null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SubscriptionDto dto)
        {
            var user = HttpContext.User.Claims.First();
            dto.UserId = Guid.Parse(user.Value);
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] SubscriptionDto dto)
        {
            var user = HttpContext.User.Claims.First();
            dto.UserId = Guid.Parse(user.Value);
            await _service.UpdateAsync(id, dto);
            return CreatedAtAction(nameof(GetById), new { id = id }, dto);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = HttpContext.User.Claims.First();
            await _service.DeleteAsync(id, Guid.Parse(user.Value));
            return NoContent();
        }
    }
}
