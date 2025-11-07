using CentralHub.Core.Domain.Aggregates.JobAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralHub.Application.Features.Jobs.DTOs
{
    public class JobDto
    {
        public Guid Id { get; set; }
        public Guid ClientId { get; set; }
        public Guid PropertyId { get; set; }
        public JobStatus Status { get; set; }
        public DateTime ScheduledStartTime { get; set; }
        public DateTime? ScheduledEndTime { get; set; }
        public string? Notes { get; set; }

    }
}
