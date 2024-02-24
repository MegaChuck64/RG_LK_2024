using GameCode.Scenes;
using Microsoft.Xna.Framework;

namespace GameCode.MapSystem;

public abstract class ActorClass
{
    public int Strength { get; private set; }
    public int Health { get; protected set; }
    public Actor Actor { get; private set; }

    public ActorClass(Actor actor, int startingHealth, int strength)
    {
        Actor = actor;
        Health = startingHealth;
        Strength = strength;
    }

    public abstract bool TryAttack(Actor effected);

    public abstract void TakeDamage(int damage);
}

public class HeroClass : ActorClass
{
    public HeroClass(Actor actor) : base(actor, 20, 5)
    {
    }

    public override bool TryAttack(Actor effected)
    {
        if (Vector2.Distance(new Vector2(effected.X, effected.Y), new Vector2(Actor.X, Actor.Y)) < 1.5f)
        {
            Logger.Log("-------------------");
            Logger.Log($"{GameSettings.SpriteDescriptions[Actor.Type].name} attacks {GameSettings.SpriteDescriptions[effected.Type].name}", Color.Blue);
            effected.ActorClass.TakeDamage(Strength);
            return true;
        }

        return false;
    }

    public override void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health < 0)
        {
            Health = 0;
            Logger.Log("!- Player Dead -!", Color.Red);
            //todo: Game OVER!!
        }

        Logger.Log($"Damage: {damage}");
    }
}

public class UndeadClass : ActorClass
{
    public UndeadClass(Actor actor) : base(actor, 5, 3)
    {
    }

    public override bool TryAttack(Actor effected)
    {
        if (Vector2.Distance(new Vector2(effected.X, effected.Y), new Vector2(Actor.X, Actor.Y)) < 1.5f)
        {
            Logger.Log("-------------------");

            Logger.Log($"{GameSettings.SpriteDescriptions[Actor.Type].name} attacks {GameSettings.SpriteDescriptions[effected.Type].name}", Color.Blue);
            effected.ActorClass.TakeDamage(Strength);
            return true;
        }

        return false;
    }

    public override void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health < 0)
        {
            Health = 0;
            Logger.Log("- Critical Hit -", Color.Red);
        }

        Logger.Log($"Damage: {damage}");


    }
}

