using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralHub.Application.Interfaces
{
    public interface ICurrentUserService
    {
        Guid TenantId { get; }
        Guid UserId { get; }
        string? Email { get; }
    }

}
