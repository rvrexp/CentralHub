using CentralHub.Application.Features.Jobs.Commands.CreateJob;
using CentralHub.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CentralHub.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] 
    public class JobsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public JobsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // POST /api/jobs
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateJob([FromBody] CreateJobCommand command)
        {
            var jobId = await _mediator.Send(command);

            // We'll build the "GetJobById" endpoint next
            return CreatedAtAction("GetJobById", new { id = jobId }, new { id = jobId });
        }

        // Placeholder for the CreatedAtAction
        [HttpGet("{id:guid}", Name = "GetJobById")]
        public IActionResult GetJobById(Guid id)
        {
            return Ok($"Placeholder for GetJobById with ID: {id}");
        }
    }
}
