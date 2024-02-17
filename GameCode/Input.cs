using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GameCode;

public static class Input
{
    public static KeyboardState KeyState { get; set; }
    public static KeyboardState LastKeyState { get; set; }
    public static MouseState MouseState { get; set; }
    public static MouseState LastMouseState { get; set; }

    public static void Update()
    {
        LastKeyState = KeyState;
        KeyState = Keyboard.GetState();

        LastMouseState = MouseState;
        MouseState = Mouse.GetState();
    }

    public static Point MousePosition => MouseState.Position;
    public static Point MouseTilePosition => new(
        MousePosition.X / GameSettings.TileSize, 
        MousePosition.Y / GameSettings.TileSize);

    public static bool WasClicked(Keys key)
    {
        return KeyState.IsKeyDown(key) && LastKeyState.IsKeyUp(key);
    }

    public static bool WasClicked(MouseButton mouseButton)
    {
        return mouseButton switch
        {
            MouseButton.Left => MouseState.LeftButton == ButtonState.Pressed &&
                                LastMouseState.LeftButton == ButtonState.Released,
            MouseButton.Middle => MouseState.MiddleButton == ButtonState.Pressed &&
                                LastMouseState.MiddleButton == ButtonState.Released,
            MouseButton.Right => MouseState.RightButton == ButtonState.Pressed &&
                                LastMouseState.RightButton == ButtonState.Released,
            _ => false,
        };
    }

    public static bool IsDown(Keys key) => KeyState.IsKeyDown(key);
    public static bool IsDown(MouseButton mouseButton)
    {
        return mouseButton switch
        {
            MouseButton.Left => MouseState.LeftButton == ButtonState.Pressed,                  
            MouseButton.Middle => MouseState.MiddleButton == ButtonState.Pressed,
            MouseButton.Right => MouseState.RightButton == ButtonState.Pressed,
            _ => false,
        };
    }

    public enum MouseButton
    {
        Left,
        Middle,
        Right
    }
}
    