using Shared.DTOs;
using Shared.Models;

namespace LogService.Application.Services;

public interface ILogService
{
    Task<LogResult> CreateLogEntryAsync(LogEntry logEntry);
    
    Task<LogResult<LogListResponse>> GetLogEntriesAsync(GetLogsRequest request);
    
    Task<LogResult<LogStatisticsResponse>> GetLogStatisticsAsync(string serviceName, DateTime? fromDate = null, DateTime? toDate = null);
}
