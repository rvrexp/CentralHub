using CentralHub.Core.Domain.Aggregates.JobAggregate;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CentralHub.Application.Features.Jobs.Commands.UpdateJob
{
    public class UpdateJobCommand : IRequest
    {
        [JsonIgnore] // From the route
        public Guid JobId { get; set; }

        // From the body
        public Guid ClientId { get; set; }
        public Guid PropertyId { get; set; }
        public JobStatus Status { get; set; }
        public DateTime ScheduledStartTime { get; set; }
        public DateTime? ScheduledEndTime { get; set; }
        public string? Notes { get; set; }
    }
}
