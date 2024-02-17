using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameCode;

public class Map : IScene
{
    public const int Width = 20;
    public const int Height = 20;
    public Tile[,] Ground { get; private set; }
    public MapItem Player { get; private set; }

    private Rectangle _boundsRect;
    private KeyboardState _keyState;
    private KeyboardState _lastKeyState;
    public Map()
    {
        _boundsRect = new Rectangle(0, 0, GameSettings.TileSize, GameSettings.TileSize);
        Player = new MapItem
        {
            X = 10,
            Y = 10,
            Tint = new Color(100, 149, 237),
            Type = SpriteType.Player
        };

        Ground = new Tile[Width, Height];
        var floorCol = new Color(160, 82, 65);
        var wallCol = new Color(152, 251, 152);
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                var typ = SpriteType.Floor;
                var col = floorCol;
                if (x == 0 || x == Width - 1 || y == 0 || y == Height - 1)
                {
                    typ = SpriteType.Wall;
                    col = wallCol;
                }
                Ground[x, y] = new Tile
                {
                    Type = typ,
                    Tint = col
                };


            }
        }
    }

    public void Update()
    {
        _lastKeyState = _keyState;
        _keyState = Keyboard.GetState();

        if (_keyState.IsKeyDown(Keys.W) && _lastKeyState.IsKeyUp(Keys.W))
        {
            Player.Y--;
            if (Player.Y < 1)
                Player.Y = 1;
        }

        if (_keyState.IsKeyDown(Keys.D) && _lastKeyState.IsKeyUp(Keys.D))
        {
            Player.X++;
            if (Player.X > Width - 2)
                Player.X = Width - 2;
        }

        if (_keyState.IsKeyDown(Keys.S) && _lastKeyState.IsKeyUp(Keys.S))
        {
            Player.Y++;
            if (Player.Y > Height - 2)
                Player.Y = Height - 2;
        }

        if (_keyState.IsKeyDown(Keys.A) && _lastKeyState.IsKeyUp(Keys.A))
        {
            Player.X--;
            if (Player.X < 1)
                Player.X = 1;
        }
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
                    sourceRectangle: GameSettings.SourceAtlas[Ground[x,y].Type],
                    color: Ground[x,y].Tint);
            }
        }

        _boundsRect.X = Player.X * GameSettings.TileSize;
        _boundsRect.Y = Player.Y * GameSettings.TileSize;
        sb.Draw(
            texture: texture,
            destinationRectangle: _boundsRect,
            sourceRectangle: GameSettings.SourceAtlas[Player.Type],
            color: Player.Tint, 
            rotation: 0f,
            origin: Vector2.Zero,
            effects: SpriteEffects.None,
            0.2f);
    }

}

public class Tile
{
    public SpriteType Type { get; set; }
    public Color Tint { get; set; }
}

public class MapItem : Tile
{
    public int X { get; set; }
    public int Y { get; set; }
}