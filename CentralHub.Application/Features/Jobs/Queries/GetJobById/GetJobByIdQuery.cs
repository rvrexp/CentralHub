using CentralHub.Application.Features.Jobs.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralHub.Application.Features.Jobs.Queries.GetJobById
{
    public record GetJobByIdQuery(Guid JobId) : IRequest<JobDto>;
}
