using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Transactions;

namespace GameCode.Scenes;

public class Map : IScene
{
    public const int Width = 20;
    public const int Height = 20;
    public Tile[,] Ground { get; private set; }
    public List<MapItem> MapItems { get; private set; }
    public Actor Player { get; private set; }

    private Rectangle _boundsRect;
    private MouseState _mouseState;
    private MouseState _lastMouseState;
    public Map()
    {
        _boundsRect = new Rectangle(0, 0, GameSettings.TileSize, GameSettings.TileSize);
        Player = new Actor
        {
            X = 10,
            Y = 10,
            Tint = new Color(100, 149, 237),
            Type = SpriteType.Player,
            DrawPath = true,
            IsSolid = true,
            Collectable = false,
        };

        Ground = new Tile[Width, Height];
        MapItems = new List<MapItem>();
        var floorCol = new Color(160, 82, 65);
        var wallCol = new Color(152, 251, 152);
        var rand = new System.Random();
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                var typ = SpriteType.Floor;
                var col = floorCol;
                var sld = false;
                if (x == 0 || x == Width - 1 || y == 0 || y == Height - 1 || rand.NextDouble() > 0.9f)
                {
                    typ = SpriteType.Wall;
                    col = wallCol;
                    sld = true;
                }
                Ground[x, y] = new Tile
                {
                    Type = typ,
                    Tint = col,
                    IsSolid = sld
                };

                if (rand.NextDouble() > 0.9f && typ != SpriteType.Wall)
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

            }
        }
    }

    public void Update()
    {
        _lastMouseState = _mouseState;
        _mouseState = Mouse.GetState();


        //todo: this is probably really slow
        var mouseTilePos = new Point(
        _mouseState.Position.X / GameSettings.TileSize,
        _mouseState.Position.Y / GameSettings.TileSize);

        if (mouseTilePos.X >= 0 && mouseTilePos.Y >= 0 && 
            mouseTilePos.X < Width - 1 && mouseTilePos.Y < Height - 1)
            Player.Target(mouseTilePos, GetCollisionMap());

        if (_mouseState.LeftButton == ButtonState.Pressed && 
            _lastMouseState.LeftButton == ButtonState.Released)
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
                //todo: apply to player inventory
                MapItems.RemoveAt(cl);
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
                mp[x, y] = !Ground[x, y].IsSolid;
            }
        }

        return mp;
    }

    public void Draw(SpriteBatch sb, Texture2D texture)
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
        
        Player.Draw(sb, texture, ref _boundsRect);
    }

}

public class Tile
{
    public SpriteType Type { get; set; }
    public Color Tint { get; set; }
    public bool IsSolid { get; set; }
}

public class MapItem : Tile
{
    public int X { get; set; }
    public int Y { get; set; }

    public bool Collectable { get; set; }
}

public class Actor : MapItem
{
    public bool DrawPath { get; set; } = false;
    public List<Point> Path { get; private set; } = new List<Point>();
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

        if (DrawPath)
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