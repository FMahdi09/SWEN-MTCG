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
}