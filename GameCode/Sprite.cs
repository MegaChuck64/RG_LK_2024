using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameCode;

public class Sprite
{
    public int X { get; set; }
    public int Y { get; set; }
    public SpriteType SpriteType { get; set; }

    private Rectangle _dest;

    public Sprite(SpriteType spriteType, int x, int y)
    {
        X = x;
        Y = y;
        SpriteType = spriteType;
        _dest = new Rectangle(X * GameSettings.TileSize, Y * GameSettings.TileSize, GameSettings.TileSize, GameSettings.TileSize);
    }

    public void Draw(SpriteBatch sb, Texture2D texture)
    {
        _dest.X = X * GameSettings.TileSize;
        _dest.Y = Y * GameSettings.TileSize;

        sb.Draw(
            texture: texture,
            destinationRectangle: _dest,
            sourceRectangle: GameSettings.SourceAtlas[SpriteType.Player],
            color: Color.White);
    }
}