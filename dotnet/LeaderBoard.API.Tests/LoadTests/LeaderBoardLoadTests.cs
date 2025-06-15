using NBomber.CSharp;
using NBomber.Http.CSharp;
using NBomber.Contracts;
using NBomber.Contracts.Stats;

namespace LeaderBoard.API.Tests.LoadTests;

public class LoadTestConfig
{
    public string BaseUrl { get; set; } = "http://localhost:5000/";
    public int WarmUpDurationSeconds { get; set; } = 3;
    public int TestDurationSeconds { get; set; } = 30;
    
    public class ScenarioConfig
    {
        public int Rate { get; set; }
        public int IntervalSeconds { get; set; } = 1;
    }
    
    public ScenarioConfig UpdateScore { get; set; } = new() { Rate = 100 };
    public ScenarioConfig GetLeaderboard { get; set; } = new() { Rate = 200 };
    public ScenarioConfig GetCustomerRank { get; set; } = new() { Rate = 100 };
    
    public class CustomerIdConfig
    {
        public long MinId { get; set; } = 1;
        public long MaxId { get; set; } = 1_000_000;
    }
    
    public CustomerIdConfig CustomerIds { get; set; } = new();
}

public class LeaderBoardLoadTests
{
    private readonly Random _random = new();
    private readonly LoadTestConfig _config;

    public LeaderBoardLoadTests(LoadTestConfig? config = null)
    {
        _config = config ?? new LoadTestConfig();
    }

    public void RunLoadTest()
    {
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(_config.BaseUrl);

        var updateScoreScenario = Scenario.Create("update_score_scenario", async context =>
        {
            var updateScoreStep = await Step.Run("update_score", context, async () =>
            {
                var customerId = _random.NextInt64(_config.CustomerIds.MinId, _config.CustomerIds.MaxId);
                var score = _random.Next(-1000, 1001);
                var request = Http.CreateRequest("POST", $"/api/v1/customer/{customerId}/score/{score}");
                var response = await Http.Send(httpClient, request);
                return response;
            });
            return Response.Ok();
        })
        .WithWarmUpDuration(TimeSpan.FromSeconds(_config.WarmUpDurationSeconds))
        .WithLoadSimulations(Simulation.Inject(
            rate: _config.UpdateScore.Rate,
            interval: TimeSpan.FromSeconds(_config.UpdateScore.IntervalSeconds),
            during: TimeSpan.FromSeconds(_config.TestDurationSeconds)));

        var getLeaderboardScenario = Scenario.Create("get_leaderboard_scenario", async context =>
        {
            var getLeaderboardStep = await Step.Run("get_leaderboard", context, async () =>
            {
                var start = _random.Next(1, 100);
                var end = start + _random.Next(1, 100);
                var request = Http.CreateRequest("GET", $"/api/v1/leaderboard?start={start}&end={end}");
                var response = await Http.Send(httpClient, request);
                return response;
            });
            return Response.Ok();
        })
        .WithWarmUpDuration(TimeSpan.FromSeconds(_config.WarmUpDurationSeconds))
        .WithLoadSimulations(Simulation.Inject(
            rate: _config.GetLeaderboard.Rate,
            interval: TimeSpan.FromSeconds(_config.GetLeaderboard.IntervalSeconds),
            during: TimeSpan.FromSeconds(_config.TestDurationSeconds)));

        var getCustomerRankScenario = Scenario.Create("get_customer_rank_scenario", async context =>
        {
            var getCustomerRankStep = await Step.Run("get_customer_rank", context, async () =>
            {
                var customerId = _random.NextInt64(_config.CustomerIds.MinId, _config.CustomerIds.MaxId);
                var high = _random.Next(0, 10);
                var low = _random.Next(0, 10);
                var request = Http.CreateRequest("GET", $"/api/v1/leaderboard/{customerId}?high={high}&low={low}");
                var response = await Http.Send(httpClient, request);
                return response;
            });
            return Response.Ok();
        })
        .WithWarmUpDuration(TimeSpan.FromSeconds(_config.WarmUpDurationSeconds))
        .WithLoadSimulations(Simulation.Inject(
            rate: _config.GetCustomerRank.Rate,
            interval: TimeSpan.FromSeconds(_config.GetCustomerRank.IntervalSeconds),
            during: TimeSpan.FromSeconds(_config.TestDurationSeconds)));

        NBomberRunner
            .RegisterScenarios(updateScoreScenario, getLeaderboardScenario, getCustomerRankScenario)
            .WithTestName("LeaderBoard Load Test")
            .WithTestSuite("API")
            .WithReportFolder("./reports")
            .WithReportFileName("leaderboard_load_test")
            .WithReportFormats(ReportFormat.Html, ReportFormat.Md)
            .Run();
    }
}