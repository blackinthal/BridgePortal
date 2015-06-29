using System;

namespace Domain.Contracts
{
    public abstract class CommandBase
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public Guid ProcessId { get; set; } 
    }
}
