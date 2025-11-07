using CentralHub.Application.Features.Jobs.Commands.CreateJob;
using CentralHub.Application.Features.Jobs.Commands.DeleteJob;
using CentralHub.Application.Features.Jobs.Commands.UpdateJob;
using CentralHub.Application.Features.Jobs.Queries.GetJobById;
using CentralHub.Application.Features.Jobs.Queries.GetJobsForDateRange;
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

        // GET /api/jobs/{id}
        [HttpGet("{id:guid}", Name = "GetJobById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetJobById(Guid id)
        {
            var query = new GetJobByIdQuery(id);
            var job = await _mediator.Send(query);
            return Ok(job);
        }

        // GET /api/jobs
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllJobs([FromQuery] GetJobsForDateRangeQuery query)
        {
            // Binds from query string: ?startDate=...&endDate=...&pageNumber=1&pageSize=20
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // PUT /api/jobs/{id}
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateJob(Guid id, [FromBody] UpdateJobCommand command)
        {
            command.JobId = id;
            await _mediator.Send(command);
            return NoContent();
        }

        // DELETE /api/jobs/{id}
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteJob(Guid id)
        {
            await _mediator.Send(new DeleteJobCommand(id));
            return NoContent();
        }
    }
}
