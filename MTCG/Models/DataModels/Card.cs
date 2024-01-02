using SWEN.MTCG.Models.Enums;

namespace SWEN.MTCG.Models.DataModels;

public class Card(int id, string guid, string name, int damage, CardElement element, Cardtype type)
{
    public int Id { get; set; } = id;
    public string Guid { get; set; } = guid;
    public string Name { get; set; } = name;
    public int Damage { get; set; } = damage;
    public CardElement Element { get; set; } = element;
    public Cardtype Type { get; set; } = type;

    public int CalculateDamage(Card enemy)
    {
        // super effective
        if((Element == CardElement.water && enemy.Element == CardElement.fire) ||
           (Element == CardElement.light && enemy.Element == CardElement.shadow) ||
           (Element == CardElement.earth && enemy.Element == CardElement.water))
            return Damage * 2;

        // not very effective
        if((Element == CardElement.fire && enemy.Element == CardElement.water) ||
           (Element == CardElement.air && enemy.Element == CardElement.fire))
            return Damage / 2;

        // immune
        if(Element == CardElement.earth && enemy.Element == CardElement.air)
            return 0;

        return Damage;
    }
}