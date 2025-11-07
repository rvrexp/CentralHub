using CentralHub.Core.Domain.Aggregates.JobAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralHub.Application.Features.Jobs.DTOs
{
    public record JobSummaryDto(
        Guid Id,
        Guid ClientId,
        Guid PropertyId,
        JobStatus Status,
        DateTime ScheduledStartTime,
        DateTime? ScheduledEndTime);
}
