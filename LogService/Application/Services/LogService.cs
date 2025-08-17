using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using LogService.Infrastructure.Data;
using Shared.Models;
using Shared.DTOs;

namespace LogService.Application.Services;

public class LogService : ILogService
{
    private readonly LogDbContext _context;
    private readonly ILogger<LogService> _logger;

    public LogService(LogDbContext context, ILogger<LogService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<LogResult> CreateLogEntryAsync(LogEntry logEntry)
    {
        try
        {
            _logger.LogDebug("Creating log entry: {ServiceName} - {Level} - {Message}", 
                logEntry.ServiceName, logEntry.Level, logEntry.Message);
            
            logEntry.CreatedAt = DateTime.UtcNow;
            logEntry.IsActive = true;
            
            _context.LogEntries.Add(logEntry);
            await _context.SaveChangesAsync();
            
            _logger.LogDebug("Log entry created successfully: {LogId}", logEntry.Id);
            return LogResult.Success(logEntry.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating log entry");
            return LogResult.Failure("An error occurred while creating the log entry");
        }
    }

    public async Task<LogResult<LogListResponse>> GetLogEntriesAsync(GetLogsRequest request)
    {
        try
        {
            _logger.LogDebug("Getting log entries with filters: Service={Service}, Level={Level}, Page={Page}", 
                request.ServiceName, request.Level, request.PageNumber);
            
            var query = _context.LogEntries.AsNoTracking().Where(l => l.IsActive);
            
            if (!string.IsNullOrEmpty(request.ServiceName))
            {
                query = query.Where(l => l.ServiceName == request.ServiceName);
            }
            
            if (!string.IsNullOrEmpty(request.Level))
            {
                query = query.Where(l => l.Level == request.Level);
            }
            
            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                query = query.Where(l => l.Message.Contains(request.SearchTerm) || 
                                        (l.ContextData != null && l.ContextData.Contains(request.SearchTerm)));
            }
            
            if (request.FromDate.HasValue)
            {
                query = query.Where(l => l.CreatedAt >= request.FromDate.Value);
            }
            
            if (request.ToDate.HasValue)
            {
                query = query.Where(l => l.CreatedAt <= request.ToDate.Value);
            }
            
            if (!string.IsNullOrEmpty(request.CorrelationId))
            {
                query = query.Where(l => l.CorrelationId == request.CorrelationId);
            }
            
            var totalCount = await query.CountAsync();
            
            var logEntries = await query
                .OrderByDescending(l => l.CreatedAt)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(l => new LogEntryResponse
                {
                    Id = l.Id,
                    ServiceName = l.ServiceName,
                    Level = l.Level,
                    Message = l.Message,
                    ContextData = l.ContextData,
                    Exception = l.Exception,
                    UserId = l.UserId,
                    CorrelationId = l.CorrelationId,
                    IpAddress = l.IpAddress,
                    UserAgent = l.UserAgent,
                    CreatedAt = l.CreatedAt
                })
                .ToListAsync();
            
            var result = new LogListResponse
            {
                LogEntries = logEntries,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
            };
            
            _logger.LogDebug("Log entries retrieved successfully: {Count} entries, Total: {TotalCount}", 
                logEntries.Count, totalCount);
            
            return LogResult<LogListResponse>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting log entries");
            return LogResult<LogListResponse>.Failure("An error occurred while retrieving log entries");
        }
    }

    public async Task<LogResult<LogStatisticsResponse>> GetLogStatisticsAsync(string serviceName, DateTime? fromDate = null, DateTime? toDate = null)
    {
        try
        {
            _logger.LogDebug("Getting log statistics for service: {ServiceName}", serviceName);
            
            var query = _context.LogEntries.AsNoTracking().Where(l => l.IsActive && l.ServiceName == serviceName);
            
            if (fromDate.HasValue)
            {
                query = query.Where(l => l.CreatedAt >= fromDate.Value);
            }
            
            if (toDate.HasValue)
            {
                query = query.Where(l => l.CreatedAt <= toDate.Value);
            }
            
            var statistics = await query
                .GroupBy(l => l.Level)
                .Select(g => new LogLevelCount
                {
                    Level = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();
            
            var totalLogs = statistics.Sum(s => s.Count);
            var errorRate = totalLogs > 0 ? (double)statistics.Where(s => s.Level == "ERROR" || s.Level == "CRITICAL").Sum(s => s.Count) / totalLogs * 100 : 0;
            
            var result = new LogStatisticsResponse
            {
                ServiceName = serviceName,
                TotalLogs = totalLogs,
                ErrorRate = Math.Round(errorRate, 2),
                LevelCounts = statistics,
                FromDate = fromDate,
                ToDate = toDate
            };
            
            _logger.LogDebug("Log statistics retrieved successfully for service: {ServiceName}", serviceName);
            return LogResult<LogStatisticsResponse>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting log statistics for service: {ServiceName}", serviceName);
            return LogResult<LogStatisticsResponse>.Failure("An error occurred while retrieving log statistics");
        }
    }
}
