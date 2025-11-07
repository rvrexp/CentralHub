using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralHub.Application.Features.Jobs.Commands.CreateJob
{
    public class CreateJobCommand : IRequest<Guid> // Returns the new Job's ID
    {
        public Guid ClientId { get; set; }
        public Guid PropertyId { get; set; }
        public DateTime ScheduledStartTime { get; set; }
        public DateTime? ScheduledEndTime { get; set; }
        public string? Notes { get; set; }
    }
}
