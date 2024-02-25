using System.Collections.Generic;
using System.Linq;

namespace GameCode.MapSystem;

public class Ticker
{
    public int Ticks { get; private set; } = 0;

    public Dictionary<int, List<Actor>> Schedule { get; private set; } = new Dictionary<int, List<Actor>>();

    public void ScheduleTurn(int interval, Actor actor)
    {
        if (!Schedule.ContainsKey(Ticks + interval))
            Schedule[Ticks + interval] = new List<Actor>();

        Schedule[Ticks + interval].Add(actor);
    }

    public void TakeTurns()
    {
        do
        {
            Ticks++;

            if (!Schedule.ContainsKey(Ticks))
                continue;

            var turns = Schedule[Ticks].ToList();
            for (int i = 0; i < turns.Count; i++)
            {
                var turn = turns[i];
                turn.ActorClass.TakeTurn(this);
            }

            Schedule.Remove(Ticks);
            break;

        } while (true);

    }
}

