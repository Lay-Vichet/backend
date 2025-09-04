using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using SubscriptionTracker.Application.DTOs;
using SubscriptionTracker.Application.Services;

namespace SubscriptionTracker.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubscriptionPaymentsController : ControllerBase
    {
        private readonly ISubscriptionPaymentService _service;

        public SubscriptionPaymentsController(ISubscriptionPaymentService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item is null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SubscriptionPaymentDto dto)
        {
            await _service.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = dto.PaymentId }, dto);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] SubscriptionPaymentDto dto)
        {
            var updated = dto with { PaymentId = id };
            await _service.UpdateAsync(updated);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }

        // Composite endpoint: add a payment and update the related subscription in one atomic operation
        [HttpPost("add-and-update")]
        public async Task<IActionResult> AddAndUpdate([FromBody] AddPaymentAndUpdateRequest req)
        {
            await _service.AddPaymentAndUpdateSubscriptionAsync(req.Payment, req.Subscription);
            return Ok();
        }
    }

    public record AddPaymentAndUpdateRequest(SubscriptionPaymentDto Payment, SubscriptionTracker.Application.DTOs.SubscriptionDto Subscription);
}
