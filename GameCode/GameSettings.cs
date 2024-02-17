using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace GameCode;

public static class GameSettings
{
    public const int TileSize = 32;
    public const int WindowWidth = 960;
    public const int WindowHeight = 640;
    public const string WindowTtitle = "RG-LK ~ 2x24";

    public const int WindowTileWidth = WindowWidth / TileSize;
    public const int WindowTileHeight = WindowHeight / TileSize;

    public static string CurrentScene { get; set; }

    public static Dictionary<SpriteType, Rectangle> SourceAtlas { get; private set; } = new ()
    {
        { SpriteType.Empty,     new Rectangle(112, 112, 16, 16) },
        { SpriteType.Square,     new Rectangle(96, 112, 16, 16) },
        { SpriteType.Wall,      new Rectangle(0, 0, 16, 16) },
        { SpriteType.Floor,     new Rectangle(16, 0, 16, 16) },
        { SpriteType.Player,    new Rectangle(0, 96, 16, 16) },

    };


}

public enum SpriteType
{
    Empty,
    Square,
    Wall,
    Floor,
    Player
}