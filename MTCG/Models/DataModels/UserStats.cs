namespace SWEN.MTCG.Models.DataModels;

public class UserStats(string name, int score, int wins, int losses)
{
    public string Name { get; set; } = name;
    public int Score { get; set; } = score;
    public int Wins { get; set; } = wins;
    public int Losses { get; set; } = losses;
}