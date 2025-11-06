using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralHub.Application.Features.Clients.Commands.UpdateProperty
{
    public class UpdatePropertyCommandValidator : AbstractValidator<UpdatePropertyCommand>
    {
        public UpdatePropertyCommandValidator()
        {
            // Ensure IDs are provided (they come from the route)
            RuleFor(x => x.ClientId).NotEmpty();
            RuleFor(x => x.PropertyId).NotEmpty();
        }
    }
}
