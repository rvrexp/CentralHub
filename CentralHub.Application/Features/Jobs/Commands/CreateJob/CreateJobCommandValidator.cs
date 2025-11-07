using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralHub.Application.Features.Jobs.Commands.CreateJob
{
    public class CreateJobCommandValidator : AbstractValidator<CreateJobCommand>
    {
        public CreateJobCommandValidator()
        {
            RuleFor(x => x.ClientId).NotEmpty();
            RuleFor(x => x.PropertyId).NotEmpty();
            RuleFor(x => x.ScheduledStartTime).NotEmpty();

            RuleFor(x => x.ScheduledEndTime)
                .GreaterThan(x => x.ScheduledStartTime)
                .When(x => x.ScheduledEndTime.HasValue)
                .WithMessage("End time must be after start time.");
        }
    }
}
