using GameCode.MapSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;


namespace GameCode.Scenes;

public class CraterScene : IScene
{
    public const int Width = 30;
    public const int Height = 30;

    public Tile[,] Ground { get; private set; }

    public List<MapItem> MapItems { get; private set; }

    public Actor Player { get; private set; }

    private System.Random _rand;
    private Rectangle _boundsRect;
    private Color _dirtColor = new(205, 133, 63);
    private Color _rockColor = new(160, 82, 45);
    public CraterScene(int seed = -1)
    {
        _rand = seed == -1 ? new System.Random() : new System.Random(seed);
        _boundsRect = new Rectangle(0, 0, GameSettings.TileSize, GameSettings.TileSize);

        Ground = new Tile[Width, Height];
        MapItems = new List<MapItem>();

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Ground[x, y] = new Tile
                {
                    IsSolid = true,
                    Tint = _rockColor,
                    Type = SpriteType.Rock
                };

                var distFromCenter = Vector2.Distance(new Vector2(Width / 2, Height / 2), new Vector2(x, y));

                var offset = _rand.NextDouble() * 0.75f;
                if (offset < 0.35f)
                    offset = 0.35f;

                if (distFromCenter < Width * offset)
                {
                    Ground[x, y].Tint = _dirtColor;
                    Ground[x, y].Type = SpriteType.Dirt;
                    Ground[x, y].IsSolid = false;
                }
            }
        }
    }

    public void Update(float dt)
    {
    }

    public void Draw(SpriteBatch sb, Texture2D sheet, SpriteFont font, float dt)
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                _boundsRect.X = x * GameSettings.TileSize;
                _boundsRect.Y = y * GameSettings.TileSize;

                sb.Draw(
                    texture: sheet,
                    destinationRectangle: _boundsRect,
                    sourceRectangle: GameSettings.SourceAtlas[Ground[x, y].Type],
                    color: Ground[x, y].Tint);
            }
        }


        foreach (var mi in MapItems)
        {
            _boundsRect.X = mi.X * GameSettings.TileSize;
            _boundsRect.Y = mi.Y * GameSettings.TileSize;
            sb.Draw(
                texture: sheet,
                destinationRectangle: _boundsRect,
                sourceRectangle: GameSettings.SourceAtlas[mi.Type],
                color: mi.Tint,
                rotation: 0f,
                origin: Vector2.Zero,
                effects: SpriteEffects.None,
                layerDepth: 0.1f);
        }

    }

}