using SWEN.MTCG.Businesslogic.Battle;

namespace SWEN.MTCG.Models.DataModels;

public class MatchHistory(string description, BattleResult result)
{
    public string Description { get; set; } = description;
    public BattleResult Result { get; set; } = result;
}