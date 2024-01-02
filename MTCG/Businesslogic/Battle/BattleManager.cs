using System.Collections.Concurrent;
using System.Text;
using SWEN.MTCG.Models.Base;
using SWEN.MTCG.Models.DataModels;

namespace SWEN.MTCG.BusinessLogic.Battle;

public class BattleManager
{
    private readonly BlockingCollection<BattleAble> _queue = [];

    public void QueueUp(BattleAble toQueue)
    {
        _queue.Add(toQueue);
    }

    public void StartMatchmaking()
    {
        while(true)
        {
            // get 2 participants
            BattleAble participantA = _queue.Take();
            BattleAble participantB = _queue.Take();

           // start battle
           Task.Run(() => StartBattle(participantA, participantB));
        }
    }

    private void StartBattle(BattleAble participantA, BattleAble participantB)
    {
        // battle
        string battleLog = Battle(participantA, participantB);

        // send result to participants
        participantA.BattleLog = battleLog;
        participantB.BattleLog = battleLog;
    }

    private string Battle(BattleAble participantA, BattleAble participantB)
    {
        StringBuilder battleLog = new();
        battleLog.AppendLine("Battle started");

        for(int rounds = 0; rounds < 100; ++rounds)
        {
            // get random card from each participant
            if(participantA.DrawRandomCard() is not Card cardA)
            {
                battleLog.AppendLine($"{ participantA.Username } is out of cards. { participantB.Username } wins the game.");
                participantB.BattleWon = true;
                return battleLog.ToString();
            }
            
            if(participantB.DrawRandomCard() is not Card cardB)
            {
                battleLog.AppendLine($"{ participantB.Username } is out of cards. { participantA.Username } wins the game.");
                participantA.BattleWon = true;
                return battleLog.ToString();
            }
            
            battleLog.AppendLine($"{ participantA.Username } : { cardA.Name } ({ cardA.Damage } Damage) vs  { participantB.Username } : { cardB.Name } ({ cardB.Damage } Damage)");

            // calculate dealt damage
            int cardADmg = cardA.CalculateDamage(cardB);
            int cardBDmg = cardB.CalculateDamage(cardA);

            battleLog.AppendLine($"Calculated damage: { cardA.Name } : { cardADmg } vs { cardB.Name } : { cardBDmg }");

            // determine winner
            if(cardADmg > cardBDmg)
            {
                participantA.InsertCard(cardA);
                participantA.InsertCard(cardB);
                battleLog.AppendLine($"{ participantA.Username } wins");
            }
            else if(cardBDmg > cardADmg)
            {
                participantB.InsertCard(cardA);
                participantB.InsertCard(cardB);
                battleLog.AppendLine($"{ participantB.Username } wins");
            }
            else
            {
                participantA.InsertCard(cardA);
                participantB.InsertCard(cardB);
                battleLog.AppendLine($"The round ended in a draw");
            }
        }

        // get remaining cards
        int remainingA = participantA.CardsLeft();
        int remainingB = participantA.CardsLeft();

        battleLog.AppendLine("Battle ended, maximum number of rounds reached");
        battleLog.AppendLine($"{ participantA.Username } has { remainingA } cards left");
        battleLog.AppendLine($"{ participantB.Username } has { remainingB } cards left");

        if(remainingA < remainingB)
        {
            participantB.BattleWon = true;
            battleLog.AppendLine($"{ participantB.Username } won the battle!");
        }
        else if(remainingA > remainingB)
        {
            participantA.BattleWon = true;
            battleLog.AppendLine($"{ participantA.Username } won the battle!");
        }

        return battleLog.ToString();
    }
}