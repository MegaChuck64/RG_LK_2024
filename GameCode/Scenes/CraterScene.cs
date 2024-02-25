using GameCode.MapSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;


namespace GameCode.Scenes;

public class CraterScene : IScene
{
    public const int Width = 30;
    public const int Height = 30;

    public Tile[,] Ground { get; private set; }
    public List<MapItem> MapItems { get; private set; }
    public List<Actor> Actors { get; private set; }
    public Actor Player { get; private set; }
    public Ticker Ticker { get; private set; }


    private System.Random _rand;
    private Rectangle _boundsRect;
    private Color _dirtColor = new(205, 133, 63);
    private Color _rockColor = new(160, 82, 45);

    public CraterScene(int seed = -1)
    {
        _rand = seed == -1 ? new System.Random() : new System.Random(seed);
        _boundsRect = new Rectangle(0, 0, GameSettings.TileSize, GameSettings.TileSize);
        Ticker = new Ticker();
        Generate();
    }

    public void Generate()
    {

        Ground = new Tile[Width, Height];
        MapItems = new List<MapItem>();
        Actors = new List<Actor>();

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

                if (x == Width / 2 && y == Height / 2)
                {
                    Player = new Actor
                    {
                        IsSolid = true,
                        Tint = Color.LightBlue,
                        X = x,
                        Y = y,
                        Type = SpriteType.Player,
                    };

                    Player.ActorClass = new HeroClass(Player, Ticker);

                    Ground[x, y].Tint = _dirtColor;
                    Ground[x, y].Type = SpriteType.Dirt;
                    Ground[x, y].IsSolid = false;
                }
                else
                {
                    var distFromCenter = Vector2.Distance(new Vector2(Width / 2, Height / 2), new Vector2(x, y));

                    var offset = _rand.NextDouble() * 0.75;
                    if (offset < 0.35f)
                        offset = 0.35f;

                    if (distFromCenter < Width * offset)
                    {
                        Ground[x, y].Tint = _dirtColor;
                        Ground[x, y].Type = SpriteType.Dirt;
                        Ground[x, y].IsSolid = false;

                        if (_rand.NextDouble() > 0.97)
                        {
                            var skel = new Actor
                            {
                                IsSolid = true,
                                Tint = Color.White,
                                Type = SpriteType.Skeleton,
                                X = x,
                                Y = y,
                            };
                            skel.ActorClass = new UndeadClass(skel, Ticker);
                            Actors.Add(skel);
                        }
                    }

                    if (_rand.NextDouble() > 0.98f)
                    {
                        var coin = new MapItem
                        {
                            Collectable = true,
                            IsSolid = false,
                            Tint = Color.Yellow,
                            Type = SpriteType.Coin,
                            X = x,
                            Y = y
                        };
                        MapItems.Add(coin);
                    }
                }
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
                mp[x, y] = !Ground[x, y].IsSolid && !Actors.Any(t => t.X == x && t.Y == y) && !MapItems.Any(t=>t.X == x && t.Y == y && t.IsSolid);
            }
        }

        return mp;
    }

    public void Update(float dt)
    {
        if (Input.WasClicked(Input.MouseButton.Left))
        {
            var inRange = Actors
                .Where(t => 
                    t.ActorClass.Health > 0 && 
                    InRange(
                        Player.ActorClass.Range, 
                        new Point(Player.X, Player.Y), 
                        new Point(t.X, t.Y)));


            if (!inRange.Any())
            {
                var target = Input.MouseTilePosition;

                Player.Target(new Point(target.X, target.Y), GetCollisionMap());
            }
            else
            {
                Player.ActorClass.TryAttack(inRange.First());
            }

            foreach (var actor in Actors.Where(t => t.ActorClass.Health > 0))
            {
                if (InRange(actor.ActorClass.Range, new Point(actor.X, actor.Y), new Point(Player.X, Player.Y)))
                {
                    actor.ActorClass.TryAttack(Player);
                }
                else if (InRange(actor.ActorClass.Sight, new Point(actor.X, actor.Y), new Point(Player.X, Player.Y)))
                {
                    var target = new Point(Player.X, Player.Y);
                    actor.Target(target, GetCollisionMap());
                }
            }

            Ticker.TakeTurns();

            var collects = MapItems
                .Select((t, i) => (t, i))
                .Where(s => s.t.Collectable && s.t.X == Player.X && s.t.Y == Player.Y)
                .ToList();

            foreach (var (c, i) in collects)
            {
                if (Player.TryAddInventoryItem(c))
                {
                    MapItems.RemoveAt(i);
                }
            }

            var deadActors = Actors
                .Select((t, i) => (t, i))
                .Where(s => s.t.ActorClass.Health <= 0)
                .ToList();

            foreach (var (a,i) in deadActors)
            {
                Actors.RemoveAt(i);
            }
        }
    }

    private static bool InRange(int range, Point center, Point location) => 
        new Rectangle(center.X - range, center.Y - range, range * 2 + 1, range * 2 + 1).Contains(location);
    

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

        foreach (var actor in Actors)
        {
            //todo: temp to demonstrate they are dying
            if (actor.ActorClass.Health > 0)
                actor.Draw(sb, sheet, ref _boundsRect);
        }


        Player.Draw(sb, sheet, ref _boundsRect);

        DrawUI(sb, font, dt);
    }

    public void DrawUI(SpriteBatch sb, SpriteFont font, float dt)
    {
        var y = 4;
        var mapEdge = GameSettings.TileSize * Width;

        var hltTxt = $"HP: {Player.ActorClass.Health}";
        sb.DrawString(font, hltTxt, new Vector2(mapEdge + 6, y), Color.White);

        y += 32;

        var invLabel = "-Inventory-";
        var lblSize = font.MeasureString(invLabel);

        var x = mapEdge + ((GameSettings.WindowWidth - mapEdge) / 2);
        x -= (int)(lblSize.X / 2);
        sb.DrawString(font, invLabel, new Vector2(x, y), Color.White);

        foreach (var itm in Player.Inventory)
        {
            y += 32;
            var inf = $"{GameSettings.SpriteDescriptions[itm.Key].name}:    {itm.Value}";
            sb.DrawString(font, inf, new Vector2(mapEdge + 6, y), Color.White);
        }

        y = GameSettings.WindowHeight / 2;

        var logLabel = "-Log-";
        var logSize = font.MeasureString(invLabel);

        x = mapEdge + ((GameSettings.WindowWidth - mapEdge) / 2);
        x -= (int)(logSize.X / 2);
        sb.DrawString(font, logLabel, new Vector2(x, y), Color.White);

        foreach (var log in Logger.History.Reverse<LogEntry>())
        {
            y += 32;
            var lgStr = $"{log.Text}";
            sb.DrawString(font, lgStr, new Vector2(mapEdge + 6, y), log.Color);
        }

        if (GameSettings.DebugOn)
        {
            var dbLbl = $"FPS: {(1f / dt):00}";
            var dbSz = font.MeasureString(dbLbl);
            var dbX = GameSettings.WindowWidth - dbSz.X - 2;
            var dbY = GameSettings.WindowHeight - dbSz.Y - 2;
            sb.DrawString(font, dbLbl, new Vector2(dbX, dbY), Color.Yellow);
        }
    }

}