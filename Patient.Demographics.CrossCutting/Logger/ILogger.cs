using System;
using System.Threading.Tasks;

namespace Patient.Demographics.CrossCutting.Logger
{
    public interface ILogger
    {
        Task LogExceptionAsync(Exception ex);
        Task LogExceptionAsync(string message, Exception ex);

        void LogException(Exception ex);

        Task LogInfoAsync(string message);
        void LogInfo(string message);

        Task LogWarningAsync(string message);
    }
}