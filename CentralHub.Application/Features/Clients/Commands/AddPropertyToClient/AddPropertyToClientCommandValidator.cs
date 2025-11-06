using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralHub.Application.Features.Clients.Commands.AddPropertyToClient
{
    public class AddPropertyToClientCommandValidator : AbstractValidator<AddPropertyToClientCommand>
    {
        public AddPropertyToClientCommandValidator()
        {
            RuleFor(x => x.ClientId).NotEmpty();
            RuleFor(x => x.Street).NotEmpty().MaximumLength(100);
            RuleFor(x => x.City).NotEmpty().MaximumLength(50);
            RuleFor(x => x.State).NotEmpty().MaximumLength(50);
            RuleFor(x => x.ZipCode).NotEmpty().MaximumLength(10);
        }
    }
}
