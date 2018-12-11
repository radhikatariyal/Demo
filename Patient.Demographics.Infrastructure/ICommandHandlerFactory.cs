using System;

namespace Patient.Demographics.Infrastructure
{
    /// <summary>
    /// Creates and destorys command handlers
    /// </summary>
    public interface ICommandHandlerFactory : IDisposable
    {
        /// <summary>
        /// Creates a command handler for the type of command
        /// </summary>
        /// <typeparam name="TCommand">The type of command to create</typeparam>
        /// <returns>A command handler</returns>
        ICommandHandler<TCommand> Create<TCommand>();

        /// <summary>
        /// Destroys the command handler
        /// </summary>
        /// <typeparam name="TCommand">The type of command handler to destroy</typeparam>
        /// <param name="command">The instance of the command to destroy</param>
        void Destroy<TCommand>(TCommand command);
    }
}
