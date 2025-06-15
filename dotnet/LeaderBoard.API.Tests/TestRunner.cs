using LeaderBoard.API.Tests.Benchmarks;
using LeaderBoard.API.Tests.LoadTests;
using BenchmarkDotNet.Running;

namespace LeaderBoard.API.Tests;

public class TestRunner
{
  public static void RunAllTests()
  {
    Console.WriteLine("开始运行基准测试...");
    var summary = BenchmarkRunner.Run<LeaderBoardBenchmarks>();
    Console.WriteLine("基准测试完成！");

    Console.WriteLine("\n开始运行负载测试...");
    var config = new LoadTestConfig
    {
      BaseUrl = "http://localhost:5000/",
      WarmUpDurationSeconds = 3,
      TestDurationSeconds = 30,
      
      // 配置各个场景的请求速率
      UpdateScore = new LoadTestConfig.ScenarioConfig 
      { 
        Rate = 100,
        IntervalSeconds = 1
      },
      GetLeaderboard = new LoadTestConfig.ScenarioConfig 
      { 
        Rate = 200,
        IntervalSeconds = 1
      },
      GetCustomerRank = new LoadTestConfig.ScenarioConfig 
      { 
        Rate = 100,
        IntervalSeconds = 1
      },
      
      // 配置客户ID范围
      CustomerIds = new LoadTestConfig.CustomerIdConfig
      {
        MinId = 1,
        MaxId = 1000  // 使用较小的范围以减少NotFound错误
      }
    };

    var loadTest = new LeaderBoardLoadTests(config);
    loadTest.RunLoadTest();
    Console.WriteLine("负载测试完成！");
  }

  public static void Main(string[] args)
  {
    RunAllTests();
  }
}