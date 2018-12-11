using System.Collections.Generic;
using System.Threading.Tasks;

namespace Patient.Demographics.Commands
{
    public interface ICommandValidator<in T> where T:ICommand
    {
        Task<List<string>> ValidateAsync(T command);
    }
    public interface ICommandValidatorFactory
    {
        ICommandValidator<T> FindValidatorFor<T>() where T : ICommand;
    }
}
