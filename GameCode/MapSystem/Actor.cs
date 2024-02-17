using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace GameCode.MapSystem;

public class Actor : MapItem
{
    public ActorClass ActorClass { get; set; }
    public bool DrawPath { get; set; } = false;
    public List<Point> Path { get; private set; } = new List<Point>();
    public Dictionary<SpriteType, int> Inventory { get; private set; } = new Dictionary<SpriteType, int>();

    public bool TryAddInventoryItem(MapItem item)
    {
        if (!GameSettings.InventoryStacks.ContainsKey(item.Type))
            return false;

        if (Inventory.ContainsKey(item.Type))
        {
            if (Inventory[item.Type] + 1 > GameSettings.InventoryStacks[item.Type])
                return false;

            Inventory[item.Type] += 1;
            return true;
        }

        Inventory.Add(item.Type, 1);
        return true;
    }

    public void Target(Point dest, bool[,] collisionMap)
    {
        var pathFinder = new PathFinder(new(new Point(X, Y), dest, collisionMap));
        Path = pathFinder.FindPath();
    }

    public void TakeStep()
    {
        if (Path.Count > 0)
        {
            X = Path[0].X;
            Y = Path[0].Y;
            Path.RemoveAt(0);
        }
    }

    public void Draw(SpriteBatch sb, Texture2D texture, ref Rectangle _boundsRect)
    {
        _boundsRect.X = X * GameSettings.TileSize;
        _boundsRect.Y = Y * GameSettings.TileSize;
        sb.Draw(
            texture: texture,
             destinationRectangle: _boundsRect,
             sourceRectangle: GameSettings.SourceAtlas[Type],
             color: Tint,
             rotation: 0f,
             origin: Vector2.Zero,
             effects: SpriteEffects.None,
             0.2f);

        if (DrawPath || GameSettings.DebugOn)
        {
            var pathColor = new Color(1f, 1f, 0f, 0.2f);
            foreach (var stp in Path)
            {
                _boundsRect.X = stp.X * GameSettings.TileSize;
                _boundsRect.Y = stp.Y * GameSettings.TileSize;
                sb.Draw(
                    texture: texture,
                    destinationRectangle: _boundsRect,
                    sourceRectangle: GameSettings.SourceAtlas[SpriteType.Square],
                    color: pathColor,
                    rotation: 0f,
                    origin: Vector2.Zero,
                    effects: SpriteEffects.None,
                    0.3f);
            }
        }
    }
}