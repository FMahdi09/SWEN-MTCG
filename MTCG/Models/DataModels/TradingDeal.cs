using SWEN.MTCG.Models.Enums;

namespace SWEN.MTCG.Models.DataModels;

public class TradingDeal(string tradeId, int userId, Card card, int minDamage, Cardtype cardType)
{
    public string TradeId { get; set; } = tradeId;
    public int UserId { get; set; } = userId;
    public Card Card { get; set; } = card;
    public int MinDamage { get; set; } = minDamage;
    public Cardtype CardType { get; set; } = cardType;
}