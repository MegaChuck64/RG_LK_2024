using GameCode.MapSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace GameCode.Scenes;

public class TravelScene : IScene
{
    public const int Width = 30;
    public const int Height = 30;

    public Tile[,] Ground { get; private set; }
    public List<MapItem> MapItems { get; private set; }

    public Actor Car { get; private set; }
    public List<Point> Path { get; private set; }

    private int _carPathIndex;
    private Rectangle _boundsRect;
    private Color _dirtColor = new(205, 133, 63);
    private Color _rockColor = new(160, 82, 45);
    private readonly System.Random _rand;
    public TravelScene(int seed = -1)
    {
        _boundsRect = new Rectangle(0, 0, GameSettings.TileSize, GameSettings.TileSize);

        _rand = seed == -1 ? new System.Random() : new System.Random(seed);

        Car = new Actor
        {
            Collectable = false,
            IsSolid = true,
            Tint = new Color(123, 104, 238),
            Type = SpriteType.Car,
            DrawPath = false,
        };

        MapItems = new List<MapItem>();

        Generate();
    }

    public void Generate()
    {
        Ground = new Tile[Width, Height];

        var baseCamp = new Point(_rand.Next(Width), _rand.Next(Height));

        var strikeCenter = new Point(_rand.Next(Width), _rand.Next(Height));

        var collisionMap = new bool[Width, Height];

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

                collisionMap[x, y] = true;

                if (_rand.NextDouble() > .95f)
                {
                    collisionMap[x, y] = false;
                    MapItems.Add(new MapItem
                    {
                        Collectable = false,
                        IsSolid = true,
                        Tint = new Color(139, 69, 19),
                        X = x,
                        Y = y,
                        Type = SpriteType.Boulder
                    });
                }
            }
        }

        var pf = new PathFinder(new PathFinderSearchParams(baseCamp, strikeCenter, collisionMap, false));
        Path = pf.FindPath();
        foreach (var p in Path)
        {
            Ground[p.X, p.Y].IsSolid = false;
            Ground[p.X, p.Y].Type = SpriteType.Dirt;
            Ground[p.X, p.Y].Tint = _dirtColor;
        }

        MapItems.Add(new MapItem()
        {
            Collectable = false,
            IsSolid = true,
            Type = SpriteType.Camp,
            X = baseCamp.X,//path[2].X,
            Y = baseCamp.Y,//path[2].Y,
            Tint = new Color(34, 139, 34)
        });

        MapItems.Add(new MapItem()
        {
            Collectable = false,
            IsSolid = true,
            Type = SpriteType.Crater,
            X = strikeCenter.X,
            Y = strikeCenter.Y,
            Tint = new Color(184, 134, 11)
        });

        _carPathIndex = 0;
        Car.X = Path[_carPathIndex].X;
        Car.Y = Path[_carPathIndex].Y;
    }

    public void Update(float dt)
    {
        if (Input.WasClicked(Input.MouseButton.Left))
        {
            TakeStep();
        }

        if (Input.WasClicked(Keys.Enter))
        {
            if (_carPathIndex >= Path.Count - 2)
                GameSettings.CurrentScene = "map";
        }
    }

    public void TakeStep()
    {
        if (_carPathIndex >= Path.Count - 2)
            return;

        _carPathIndex++;
        Car.X = Path[_carPathIndex].X;
        Car.Y = Path[_carPathIndex].Y;
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

        Car.Draw(sb, sheet, ref _boundsRect);
    }
}

