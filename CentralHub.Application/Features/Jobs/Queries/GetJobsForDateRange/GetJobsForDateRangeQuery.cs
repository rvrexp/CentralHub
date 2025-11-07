using CentralHub.Application.Common.Models;
using CentralHub.Application.Features.Jobs.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralHub.Application.Features.Jobs.Queries.GetJobsForDateRange
{
    public class GetJobsForDateRangeQuery : IRequest<PagedResult<JobSummaryDto>>
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
