using LeaderBoard.Core;
using LeaderBoard.Models;
using Microsoft.AspNetCore.Mvc;

namespace LeaderBoard.API.Controllers;

[ApiController]
[Route("api/v1")]
public class LeaderBoardController : ControllerBase
{
    private readonly LeaderBoardEngine _engine;
    private readonly ILogger<LeaderBoardController> _logger;
    
    public LeaderBoardController(
        LeaderBoardEngine engine,
        ILogger<LeaderBoardController> logger)
    {
        _engine = engine;
        _logger = logger;
    }
    
    [HttpPost("customer/{customerId}/score/{score}")]
    public async Task<ActionResult<decimal>> UpdateScore(
        long customerId, decimal score)
    {
        try
        {
            if (score < -1000 || score > 1000)
                return BadRequest("分数必须在 -1000 到 1000 之间");
            
            var newScore = await _engine.UpdateScoreAsync(customerId, score);
            return Ok(newScore);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新用户 {CustomerId} 分数时发生错误", customerId);
            return StatusCode(500, "服务器内部错误");
        }
    }
    
    [HttpGet("leaderboard")]
    public async Task<ActionResult<List<CustomerRankInfo>>> GetLeaderboard(
        [FromQuery] int start = 1, [FromQuery] int end = 10)
    {
        try
        {
            if (start < 1 || end < start || end - start > 1000)
                return BadRequest("无效的范围参数");
            
            var customers = await _engine.GetCustomersByRankRangeAsync(start, end);
            return Ok(customers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取排行榜范围 {Start}-{End} 时发生错误", start, end);
            return StatusCode(500, "服务器内部错误");
        }
    }
    
    [HttpGet("leaderboard/{customerId}")]
    public async Task<ActionResult<List<CustomerRankInfo>>> GetCustomerNeighbors(
        long customerId, [FromQuery] int high = 0, [FromQuery] int low = 0)
    {
        try
        {
            if (high < 0 || low < 0 || high + low > 100)
                return BadRequest("无效的邻居参数");
            
            var result = await _engine.GetCustomerNeighborsAsync(
                customerId, high, low);
            
            if (result == null)
                return NotFound($"未找到用户 {customerId} 的排行榜信息");
            
            var response = new List<CustomerRankInfo>();
            response.AddRange(result.Neighbors);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取用户 {CustomerId} 的邻居信息时发生错误", customerId);
            return StatusCode(500, "服务器内部错误");
        }
    }
} 