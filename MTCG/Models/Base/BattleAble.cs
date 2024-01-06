using SWEN.MTCG.Businesslogic.Battle;
using SWEN.MTCG.Models.DataModels;

namespace SWEN.MTCG.Models.Base;

public abstract class BattleAble(string username)
{
    public string Username { get; set; } = username;
    public BattleResult BattleResult { get; set; } = BattleResult.lose;
    public string History { get; set; } = "";
    private readonly EventWaitHandle _ewh = new(false, EventResetMode.AutoReset);
    private List<Card> _deck = [];
    private readonly Random _rng = new();
    private string _log = "";

    public string BattleLog
    {
        get => _log;
        set
        {
            _log = value;
            _ewh.Set();
        }
    }

    public Card? DrawRandomCard()
    {
        if(_deck.Count == 0)
            return null;

        Card card = _deck[_rng.Next(_deck.Count)];

        _deck.Remove(card);

        return card;
    }

    public void InsertCard(Card card)
    {
        _deck.Add(card);
    }

    public bool SetDeck(List<Card> deck)
    {
        deck = deck.Distinct().ToList();

        if(deck.Count != 4)
            return false;

        _deck = deck;
        
        return true;
    }

    public int CardsLeft()
    {
        return _deck.Count;
    }

    public string WaitForBattleLog()
    {
        // wait until the battle log has been set
        _ewh.WaitOne();

        return _log;
    }
}