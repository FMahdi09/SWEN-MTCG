using SWEN.MTCG.Models.DataModels;

namespace SWEN.MTCG.Models.Base;

public abstract class BattleAble
{
    private readonly EventWaitHandle _ewh = new(false, EventResetMode.AutoReset);
    private readonly Random _rng = new();

    private List<Card> _deck = [];

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

    public Card DrawRandomCard()
    {
        return _deck[_rng.Next(_deck.Count)];
    }

    public bool SetDeck(List<Card> deck)
    {
        deck = deck.Distinct().ToList();

        if(deck.Count != 4)
            return false;

        _deck = deck;
        
        return true;
    }

    public string WaitForBattleLog()
    {
        // wait until the battle log has been set
        _ewh.WaitOne();

        return _log;
    }
}