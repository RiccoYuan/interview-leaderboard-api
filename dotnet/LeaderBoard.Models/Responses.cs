namespace LeaderBoard.Models;

public class ScoreUpdateResponse
{
    public long CustomerId { get; set; }
    public decimal CurrentScore { get; set; }
    public DateTime UpdateTime { get; set; }
}

public class CustomerRankInfo
{
    public long CustomerId { get; set; }
    public decimal Score { get; set; }
    public int Rank { get; set; }
}

public class LeaderboardResponse
{
    public List<CustomerRankInfo> Customers { get; set; } = new();
    public int StartRank { get; set; }
    public int EndRank { get; set; }
    public int TotalCount { get; set; }
}

public class CustomerNeighborsResponse
{
    public long CustomerId { get; set; }
    public int Rank { get; set; }
    public decimal Score { get; set; }
    public List<CustomerRankInfo> Neighbors { get; set; } = new();
}

public class RankingUpdateTask
{
    public CustomerNode Customer { get; }
    
    public RankingUpdateTask(CustomerNode customer)
    {
        Customer = customer;
    }
} 