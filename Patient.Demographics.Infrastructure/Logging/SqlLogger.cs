using Patient.Demographics.Common;
using Patient.Demographics.CrossCutting.Logger;
using Patient.Demographics.Data;
using Patient.Demographics.Data.Entities;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Patient.Demographics.Infrastructure.Logging
{
    public class SqlLogger : ILogger
    {
        private readonly string _eventSource = "Application Error";
        //when the database call fails, the EF expection will be logged with this Id
        private readonly int _logExceptionEventId = Int16.MaxValue - 1;
        //if the EF call failed, the original exception will be logged using this Id
        private readonly int _applicationExceptionEventId = Int16.MaxValue;

        public async Task LogWarningAsync(string message) => await WriteToLogAsync("Warn", message);

        public async Task LogExceptionAsync(Exception ex)
        {
            try
            {
                await WriteToLogAsync("Error", ex.Message, ex.ToString(), suppressExceptions: false);
            }
            catch (Exception sqlEx)
            {
                //logger failed, store the log using windows Event Log
                WriteEventLog(ex, sqlEx);
            }
        }

        public void LogInfo(string message) => WriteToLog("Info", message);

        public async Task LogInfoAsync(string message) => await WriteToLogAsync("Info", message);

        public void LogException(Exception ex)
        {
            try
            {
                WriteToLog("Error", ex.Message, ex.ToString(), suppressExceptions: false);
            }
            catch (Exception sqlEx)
            {
                WriteEventLog(ex, sqlEx);
            }
        }

        public async Task LogExceptionAsync(string message, Exception ex)
        {
            try
            {
                await WriteToLogAsync("Error", message, ex.ToString(), suppressExceptions: false);
            }
            catch (Exception sqlEx)
            {
                //logger failed, store the log using windows Event Log
                WriteEventLog(ex, sqlEx);
            }
        }

        private void WriteEventLog(Exception ex, Exception sqlEx)
        {
            try
            { }
            catch
            {
            }
        }


        private async Task WriteToLogAsync(string type,  string message, string info = "", bool suppressExceptions = true)
        {
            try
            {
                using (var dbContext = new SqlDataContext())
                {
                    await dbContext.SaveChangesAsync();
                }
            }
            catch
            {
                if (!suppressExceptions)
                {
                    throw;
                }
            }
        }
        

        private void WriteToLog(string type, string message, string info = "", bool suppressExceptions = true)
        {
            try
            {
                using (var dbContext = new SqlDataContext())
                {
                    dbContext.SaveChanges();
                }
            }
            catch 
            {
                if (!suppressExceptions)
                {
                    throw;
                }
            }
        }
    }
}