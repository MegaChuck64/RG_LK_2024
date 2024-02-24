using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace GameCode;

public static class GameSettings
{
    public const int TileSize = 32;
    public const int WindowWidth = 1280;
    public const int WindowHeight = 960;
    public const string WindowTtitle = "RG-LK ~ 2x24";

    public const int WindowTileWidth = WindowWidth / TileSize;
    public const int WindowTileHeight = WindowHeight / TileSize;

    public static string CurrentScene { get; set; }
    public static bool DebugOn { get; set; }

    public static Dictionary<SpriteType, Rectangle> SourceAtlas { get; private set; } = new ()
    {
        { SpriteType.Empty,     new Rectangle(112, 112, 16, 16) },
        { SpriteType.Square,     new Rectangle(96, 112, 16, 16) },
        { SpriteType.Wall,      new Rectangle(0, 0, 16, 16) },
        { SpriteType.Floor,     new Rectangle(16, 0, 16, 16) },
        { SpriteType.Boulder,     new Rectangle(32, 0, 16, 16) },
        { SpriteType.Rock,     new Rectangle(48, 0, 16, 16) },
        { SpriteType.Dirt,     new Rectangle(64, 0, 16, 16) },
        { SpriteType.Camp,     new Rectangle(80, 0, 16, 16) },
        { SpriteType.Crater,     new Rectangle(96, 0, 16, 16) },
        { SpriteType.Car,     new Rectangle(0, 64, 16, 16) },
        { SpriteType.Player,    new Rectangle(0, 96, 16, 16) },
        { SpriteType.Skeleton,    new Rectangle(0, 112, 16, 16) },
        { SpriteType.Coin,    new Rectangle(0, 48, 16, 16) },
    };

    public static Dictionary<SpriteType, (string name, string description)> SpriteDescriptions
    { get; private set; } = new()
    {
        { SpriteType.Empty, (string.Empty, string.Empty) },
        { SpriteType.Square, (string.Empty, string.Empty) },
        { SpriteType.Wall, ("Wall", "Paneled...") },
        { SpriteType.Floor, ("Floor", "Wooden...") },
        { SpriteType.Boulder, ("Boulder", "Big rock...") },
        { SpriteType.Dirt, ("Dirt", "Dirty...") },
        { SpriteType.Rock, ("Rock", "Rocky...") },
        { SpriteType.Camp, ("Camp", "It's safe here...") },
        { SpriteType.Crater, ("Camp", "Big whole...") },
        { SpriteType.Car, ("Car", "Sedan...") },
        { SpriteType.Player, ("Player", "Hi /you...") },
        { SpriteType.Skeleton, ("Skeleton", "Undead...") },
        { SpriteType.Coin, ("Coin", "As good as gold...") },
    };

    public static Dictionary<SpriteType, int> InventoryStacks { get; private set; } = new()
    {
        { SpriteType.Coin, 5 },
        { SpriteType.Rock, 12 },
    };


}


public enum SpriteType
{
    Empty,
    Square,
    Wall,
    Floor,
    Rock,
    Boulder,
    Dirt,
    Camp,
    Crater,
    Car,
    Player,
    Skeleton,
    Coin
}