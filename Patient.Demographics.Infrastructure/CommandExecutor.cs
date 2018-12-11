using System;
using System.Threading.Tasks;
using Patient.Demographics.CrossCutting;
using Patient.Demographics.Commands;

namespace Patient.Demographics.Infrastructure
{
    public class CommandExecutor : ICommandExecutor
    {
        private readonly ICommandHandlerFactory _commandHandlerFactory;

        public CommandExecutor(ICommandHandlerFactory commandHandlerFactory)
        {
            _commandHandlerFactory = commandHandlerFactory;
        }

        /// <summary>
        ///     Executes the command invoking a handler
        /// </summary>
        /// <typeparam name="TCommand">the command being executed</typeparam>
        /// <param name="command">the command being executed</param>
        public async Task ExecuteAsync<TCommand>(TCommand command) where TCommand : ICommand
        {
            if (command.CommandIssuedByUserId == Guid.Empty)
            {
                throw new ArgumentException($"Command {command.GetType().Name} must be linked to a user Id", nameof(command.CommandIssuedByUserId));
            }

            ICommandHandler<TCommand> handler = _commandHandlerFactory.Create<TCommand>();

            try
            {
                using (var scope = TransactionUtils.CreateTransactionScope())
                {
                    await handler.HandleAsync(command);
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                throw new CommandException("Error in command", ex);
            }
            finally
            {
                _commandHandlerFactory.Destroy(handler);
            }
        }
    }
}
