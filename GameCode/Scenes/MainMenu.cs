using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace GameCode.Scenes;

public class MainMenu : IScene
{
    private readonly SpriteFont _font;
    private readonly Button _playButton;
    private readonly string title = "Hollow World...";
    private MouseState mouseState;
    private MouseState lastMouseState;
    public MainMenu(ContentManager content)
    {
        _font = content.Load<SpriteFont>(System.IO.Path.Combine("Fonts", "font_22"));
        _playButton = new Button(
            new Rectangle(GameSettings.WindowWidth / 2 - 50, GameSettings.WindowTileHeight, 100, 50),
            "Play",
            () =>
            {
                GameSettings.CurrentScene = "map";
            });
    }

    public void Update()
    {
        lastMouseState = mouseState;
        mouseState = Mouse.GetState();

        _playButton.Update(
            mouseState.Position,
            mouseState.LeftButton == ButtonState.Pressed && lastMouseState.LeftButton == ButtonState.Released);
    }

    public void Draw(SpriteBatch sb, Texture2D sheet)
    {
        sb.DrawString(_font, title, new Vector2(200, 200), Color.White);
        _playButton.Draw(sb, _font, sheet);
    }
}

public class Button
{
    public Rectangle Bounds { get; set; }
    public Color BackgroundColor { get; set; } = new Color(0.2f, 0.2f, 0.2f, 1f);
    public Color HoverColor { get; set; } = new Color(0.2f, 0.2f, 0.5f, 1f);
    public Color TextColor { get; set; } = Color.White;
    public Color TextHoverColor { get; set; } = Color.White;
    public string Text { get; set; }
    public Action OnClick { get; set; }

    private Color currentColor;
    private Color currentTextColor;
    public Button(Rectangle bounds, string text, Action onClick)
    {
        OnClick = onClick;
        Bounds = bounds;
        Text = text;
        currentColor = BackgroundColor;
        currentTextColor = TextColor;
    }

    public void Update(Point mousePos, bool clicked)
    {
        if (Bounds.Contains(mousePos))
        {
            currentColor = HoverColor;
            currentTextColor = TextHoverColor;
            if (clicked)
            {
                OnClick?.Invoke();
            }
        }
        else
        {
            currentColor = BackgroundColor;
            currentTextColor = TextColor;
        }
    }

    public void Draw(SpriteBatch sb, SpriteFont font, Texture2D sheet)
    {
        //draw box
        sb.Draw(
            texture: sheet,
            destinationRectangle: Bounds,
            sourceRectangle: GameSettings.SourceAtlas[SpriteType.Square],
            color: currentColor,
            rotation: 0f,
            origin: Vector2.Zero,
            effects: SpriteEffects.None,
            layerDepth: 0f);

        var xpos = Bounds.X + Bounds.Width / 2;
        var strSize = font.MeasureString(Text);
        xpos -= (int)(strSize.X / 2);

        var ypos = Bounds.Y + Bounds.Height / 2;
        ypos -= (int)(strSize.Y / 2);

        sb.DrawString(
            spriteFont: font,
            text: Text,
            position: new Vector2(xpos, ypos),
            color: currentTextColor,
            rotation: 0f,
            origin: Vector2.Zero,
            scale: 1f,
            effects: SpriteEffects.None,
            0.1f);
    }


}