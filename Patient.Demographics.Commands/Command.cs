using System;

namespace Patient.Demographics.Commands
{
    public abstract class Command : ICommand
    {
        public Guid CommandIssuedByUserId { get; set; }
    }
}