using System.Threading.Tasks;

namespace Patient.Demographics.Commands
{
    /// <summary>
    /// Finds the correct handler for a command and executes it
    /// </summary>
    public interface ICommandExecutor
    {
        /// <summary>
        /// Asynchronous version of 
        /// </summary>
        /// <typeparam name="TCommand"></typeparam>
        /// <param name="command"></param>
        /// <returns></returns>
        Task ExecuteAsync<TCommand>(TCommand command) where TCommand : ICommand;
    }
}
