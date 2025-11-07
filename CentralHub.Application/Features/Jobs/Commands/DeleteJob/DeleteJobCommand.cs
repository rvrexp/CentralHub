using MediatR;

namespace CentralHub.Application.Features.Jobs.Commands.DeleteJob
{
    public record DeleteJobCommand(Guid JobId) : IRequest;
}
