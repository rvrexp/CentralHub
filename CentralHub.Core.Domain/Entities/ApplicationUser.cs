using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralHub.Core.Domain.Entities
{
    /// <summary>
    /// Custom user class that extends the default IdentityUser.
    /// We add a TenantId to link every user to their specific business/tenant.
    /// </summary>
    public class ApplicationUser : IdentityUser<Guid> // We specify Guid as the primary key type
    {
        /// <summary>
        /// The unique identifier for the tenant (business) this user belongs to.
        /// </Shorter>
        public Guid TenantId { get; set; }

        // You could add other properties here later, like:
        // public string FirstName { get; set; }
        // public string LastName { get; set; }
    }
}
