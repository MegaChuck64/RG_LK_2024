using GameCode.MapSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace GameCode.Scenes;

public class Map : IScene
{
    public const int Width = 20;
    public const int Height = 20;
    public Tile[,] Ground { get; private set; }
    public List<MapItem> MapItems { get; private set; }
    public List<Actor> Actors { get; private set; }
    public Actor Player { get; private set; }

    private readonly System.Random _rand;
    private Rectangle _boundsRect;
    private Color _floorColor = new (160, 82, 65);
    private Color _wallColor = new (152, 251, 152);

    public Map(int seed = -1)
    {        
        _rand = seed == -1 ? new System.Random() : new System.Random(seed);

        _boundsRect = new Rectangle(0, 0, GameSettings.TileSize, GameSettings.TileSize);
        Player = new Actor
        {
            X = 10,
            Y = 10,
            Tint = new Color(100, 149, 237),
            Type = SpriteType.Player,
            DrawPath = false,
            IsSolid = true,
            Collectable = false,
        };

        Ground = new Tile[Width, Height];
        MapItems = new List<MapItem>();
        Actors = new List<Actor>();

        Generate();
    }

    private void Generate()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                var typ = SpriteType.Floor;
                var col = _floorColor;
                var sld = false;
                if (x == 0 || x == Width - 1 || y == 0 || y == Height - 1 ||
                    (_rand.NextDouble() > 0.9f && !(x == Player.X && y == Player.Y)))
                {
                    typ = SpriteType.Wall;
                    col = _wallColor;
                    sld = true;
                }
                Ground[x, y] = new Tile
                {
                    Type = typ,
                    Tint = col,
                    IsSolid = sld
                };

                if (_rand.NextDouble() > 0.9f && typ != SpriteType.Wall)
                {
                    if (_rand.NextDouble() > 0.5f)
                    {
                        MapItems.Add(new MapItem
                        {
                            X = x,
                            Y = y,
                            Collectable = true,
                            IsSolid = false,
                            Tint = Color.Yellow,
                            Type = SpriteType.Coin
                        });
                    }
                    else
                    {
                        Actors.Add(new Actor
                        {
                            X = x,
                            Y = y,
                            Collectable = false,
                            IsSolid = true,
                            Tint = Color.GhostWhite,
                            DrawPath = false,
                            Type = SpriteType.Skeleton,
                        });
                    }
                }

            }
        }
    }

    public void Update(float dt)
    {
        //todo: this is probably really slow
        var mouseTilePos = Input.MouseTilePosition;


        if (mouseTilePos.X >= 0 && mouseTilePos.Y >= 0 && 
            mouseTilePos.X < Width - 1 && mouseTilePos.Y < Height - 1)
            Player.Target(mouseTilePos, GetCollisionMap());

        if (Input.WasClicked(Input.MouseButton.Left))
        {
            Player.TakeStep();
            var collectables = new List<int>();
            for (int i = 0; i < MapItems.Count; i++)
            {
                var mi = MapItems[i];

                if (!mi.Collectable)
                    continue;

                if (mi.X == Player.X && mi.Y == Player.Y)
                {
                    collectables.Add(i);
                }
            }
            foreach (var cl in collectables)
            {
                if (Player.TryAddInventoryItem(MapItems[cl]))
                {
                    MapItems.RemoveAt(cl);
                }
            }

            foreach (var actor in Actors)
            {
                var collisionMap = GetCollisionMap();
                actor.Target(new Point(Player.X, Player.Y), collisionMap);
                if (actor.Path.Count > 1)
                    actor.TakeStep();
            }
        }
    }

    

    private bool[,] GetCollisionMap()
    {
        var mp = new bool[Width, Height];

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                mp[x, y] = !Ground[x, y].IsSolid && !Actors.Any(t=>t.X == x && t.Y == y);
            }
        }

        return mp;
    }

    public void Draw(SpriteBatch sb, Texture2D texture, SpriteFont font, float dt)
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                _boundsRect.X = x * GameSettings.TileSize;
                _boundsRect.Y = y * GameSettings.TileSize;

                sb.Draw(
                    texture: texture,
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
                texture: texture,
                destinationRectangle: _boundsRect,
                sourceRectangle: GameSettings.SourceAtlas[mi.Type],
                color: mi.Tint,
                rotation: 0f,
                origin: Vector2.Zero,
                effects: SpriteEffects.None,
                layerDepth: 0.1f);
        }

        foreach (var actor in Actors)
        {
            actor.Draw(sb, texture, ref _boundsRect);
        }
        
        Player.Draw(sb, texture, ref _boundsRect);

        DrawUI(sb, font, dt);
    }

    public void DrawUI(SpriteBatch sb, SpriteFont font, float dt)
    {
        var invLabel = "-Inventory-";
        var lblSize = font.MeasureString(invLabel);
        var mapEdge = GameSettings.TileSize * Width;        
        
        var x = mapEdge + ((GameSettings.WindowWidth - mapEdge)/2);
        x -= (int)(lblSize.X / 2);
        var y = 4;
        sb.DrawString(font, invLabel, new Vector2(x, y), Color.White);

        foreach (var itm in Player.Inventory)
        {
            y += 32;
            var inf = $"{GameSettings.SpriteDescriptions[itm.Key].name}:    {itm.Value}";            
            sb.DrawString(font, inf, new Vector2(mapEdge + 6, y), Color.White);
        }

        if (GameSettings.DebugOn)
        {
            var dbLbl = $"FPS: {(1f/dt):00}";
            var dbSz = font.MeasureString(dbLbl);
            var dbX = GameSettings.WindowWidth - dbSz.X - 2;
            var dbY = GameSettings.WindowHeight - dbSz.Y - 2;
            sb.DrawString(font, dbLbl, new Vector2(dbX, dbY), Color.Yellow);
        }
    }

}




