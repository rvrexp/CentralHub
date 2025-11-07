using CentralHub.Core.Domain.Aggregates.ClientAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralHub.Core.Domain.Aggregates.JobAggregate
{
    // Defines the status of a job
    public enum JobStatus
    {
        Scheduled,
        InProgress,
        Completed,
        Canceled
    }

    /// <summary>
    /// Represents the Job aggregate root. This is the core operational entity.
    /// </summary>
    public class Job
    {
        public Guid Id { get; private set; }
        public Guid TenantId { get; private set; }

        public Guid ClientId { get; private set; } // Foreign key to the Client
        public Guid PropertyId { get; private set; } // Foreign key to the Property

        public JobStatus Status { get; private set; }
        public DateTime ScheduledStartTime { get; private set; }
        public DateTime? ScheduledEndTime { get; private set; }
        public string? Notes { get; private set; }

        // Navigation properties (optional, but useful for EF Core)
        public Client Client { get; private set; }
        public Property Property { get; private set; }

        // Parameterless constructor for EF Core
        private Job() { }

        /// <summary>
        /// Factory method to create a new, valid Job.
        /// </summary>
        public static Job Create(
            Guid tenantId,
            Guid clientId,
            Guid propertyId,
            DateTime scheduledStartTime,
            DateTime? scheduledEndTime = null,
            string? notes = null)
        {
            // Enforce invariants (business rules)
            if (tenantId == Guid.Empty) throw new ArgumentException("TenantId is required.", nameof(tenantId));
            if (clientId == Guid.Empty) throw new ArgumentException("ClientId is required.", nameof(clientId));
            if (propertyId == Guid.Empty) throw new ArgumentException("PropertyId is required.", nameof(propertyId));
            if (scheduledEndTime.HasValue && scheduledEndTime < scheduledStartTime)
            {
                throw new InvalidOperationException("End time cannot be before start time.");
            }

            return new Job
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                ClientId = clientId,
                PropertyId = propertyId,
                ScheduledStartTime = scheduledStartTime,
                ScheduledEndTime = scheduledEndTime,
                Notes = notes,
                Status = JobStatus.Scheduled // Default status
            };
        }

        /// <summary>
        /// Updates the job's details.
        /// </summary>
        public void UpdateDetails(DateTime scheduledStartTime, DateTime? scheduledEndTime, string? notes)
        {
            if (scheduledEndTime.HasValue && scheduledEndTime < scheduledStartTime)
            {
                throw new InvalidOperationException("End time cannot be before start time.");
            }

            ScheduledStartTime = scheduledStartTime;
            ScheduledEndTime = scheduledEndTime;
            Notes = notes;
        }

        /// <summary>
        /// Updates the job's status.
        /// </summary>
        public void UpdateStatus(JobStatus newStatus)
        {
            // Add any status transition rules here if needed
            // e.g., if(Status == JobStatus.Completed) throw new InvalidOperationException(...)
            Status = newStatus;
        }
    }
}
