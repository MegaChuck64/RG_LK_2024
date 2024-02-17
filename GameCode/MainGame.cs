using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace GameCode
{
    public class MainGame : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D _texture;

        private Dictionary<string, IScene> _scenes;

        public MainGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = GameSettings.WindowWidth;
            _graphics.PreferredBackBufferHeight = GameSettings.WindowHeight;

            Window.AllowUserResizing = false;
            Window.Title = GameSettings.WindowTtitle;

            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _texture = Content.Load<Texture2D>(System.IO.Path.Combine("Sprites", "sheet"));

            _scenes = new Dictionary<string, IScene>()
            {
                { "menu", new MainMenu(Content) },
                { "map", new Map() }
            };

            GameSettings.CurrentScene = "menu";
        }

        protected override void Update(GameTime gameTime)
        {
            _scenes[GameSettings.CurrentScene].Update();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            
            _spriteBatch.Begin(
                sortMode: SpriteSortMode.FrontToBack,
                blendState: BlendState.AlphaBlend,
                samplerState: SamplerState.PointClamp,
                depthStencilState: DepthStencilState.DepthRead,
                rasterizerState: RasterizerState.CullCounterClockwise,
                effect: null,
                transformMatrix: null);

            _scenes[GameSettings.CurrentScene].Draw(_spriteBatch, _texture);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
