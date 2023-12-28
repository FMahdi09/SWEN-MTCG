namespace SWEN.MTCG.Models.DataModels;

public class TradingDeal(string tradeId, int userId, Card card, int minDamage, string cardType)
{
    public string TradeId { get; set; } = tradeId;
    public int UserId { get; set; } = userId;
    public Card Card { get; set; } = card;
    public int MinDamage { get; set; } = minDamage;
    public string CardType { get; set; } = cardType;
}