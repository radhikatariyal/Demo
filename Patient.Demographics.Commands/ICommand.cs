using System;

namespace Patient.Demographics.Commands
{
    public interface ICommand
    {
        Guid CommandIssuedByUserId { get; }
    }
}
