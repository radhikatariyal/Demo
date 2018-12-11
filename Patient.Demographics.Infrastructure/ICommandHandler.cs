using System.Threading.Tasks;

namespace Patient.Demographics.Infrastructure
{
    /// <summary>
    /// Marker interface for a command handler
    /// </summary>
    public interface ICommandHandler
    {
    }

    /// <summary>
    /// A handler for the command
    /// </summary>
    /// <typeparam name="TCommand"></typeparam>
    public interface ICommandHandler<in TCommand> : ICommandHandler
    {
        /// <summary>
        /// Handles command exectution
        /// </summary>
        /// <param name="command"></param>
        Task HandleAsync(TCommand command);
    }
}
