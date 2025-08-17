using Microsoft.AspNetCore.Mvc;
using LogService.Application.Services;
using Shared.DTOs;
using Shared.Models;

namespace LogService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class LogController : ControllerBase
{
    private readonly ILogService _logService;
    private readonly ILogger<LogController> _logger;

    public LogController(ILogService logService, ILogger<LogController> logger)
    {
        _logService = logService;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(typeof(LogResult), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateLog([FromBody] CreateLogRequest request)
    {
        try
        {
            _logger.LogInformation("CreateLog request received for service: {ServiceName}, Level: {Level}", 
                request.ServiceName, request.Level);
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var logEntry = new LogEntry
            {
                Id = Guid.NewGuid(),
                ServiceName = request.ServiceName,
                Level = request.Level,
                Message = request.Message,
                ContextData = request.ContextData,
                Exception = request.Exception,
                UserId = request.UserId,
                CorrelationId = request.CorrelationId,
                IpAddress = request.IpAddress,
                UserAgent = request.UserAgent
            };
            
            var result = await _logService.CreateLogEntryAsync(logEntry);
            
            if (result.IsSuccess)
            {
                _logger.LogInformation("Log entry created successfully: {LogId}", result.LogId);
                return CreatedAtAction(nameof(GetLogs), new { id = result.LogId }, result);
            }
            
            return BadRequest(result.ErrorMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating log entry for service: {ServiceName}", request.ServiceName);
            return StatusCode(500, "An internal server error occurred");
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(LogListResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetLogs(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] string? serviceName = null,
        [FromQuery] string? level = null,
        [FromQuery] string? searchTerm = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] string? correlationId = null)
    {
        try
        {
            _logger.LogInformation("GetLogs request received: Page={Page}, Size={Size}, Service={Service}, Level={Level}", 
                pageNumber, pageSize, serviceName, level);
            
            var request = new GetLogsRequest
            {
                PageNumber = Math.Max(1, pageNumber),
                PageSize = Math.Max(1, Math.Min(100, pageSize)),
                ServiceName = serviceName,
                Level = level,
                SearchTerm = searchTerm,
                FromDate = fromDate,
                ToDate = toDate,
                CorrelationId = correlationId
            };
            
            var result = await _logService.GetLogEntriesAsync(request);
            
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            
            return BadRequest(result.ErrorMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting log entries");
            return StatusCode(500, "An internal server error occurred");
        }
    }

    [HttpGet("statistics/{serviceName}")]
    [ProducesResponseType(typeof(LogStatisticsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetLogStatistics(
        string serviceName,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        try
        {
            _logger.LogInformation("GetLogStatistics request received for service: {ServiceName}", serviceName);
            
            if (string.IsNullOrWhiteSpace(serviceName))
            {
                return BadRequest("Service name is required");
            }
            
            var result = await _logService.GetLogStatisticsAsync(serviceName, fromDate, toDate);
            
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            
            return BadRequest(result.ErrorMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting log statistics for service: {ServiceName}", serviceName);
            return StatusCode(500, "An internal server error occurred");
        }
    }

    [HttpGet("levels")]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
    public IActionResult GetLogLevels()
    {
        var levels = new List<string> { "INFO", "WARNING", "ERROR", "CRITICAL" };
        return Ok(levels);
    }

    [HttpGet("services")]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetServiceNames()
    {
        try
        {
            var serviceNames = new List<string> { "AuthService", "ProductService", "LogService", "ApiGateway" };
            return Ok(serviceNames);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting service names");
            return StatusCode(500, "An internal server error occurred");
        }
    }
}
