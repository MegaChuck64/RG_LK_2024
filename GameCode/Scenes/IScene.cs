using Microsoft.Xna.Framework.Graphics;

namespace GameCode.Scenes;

public interface IScene
{
    void Update(float dt);
    void Draw(SpriteBatch sb, Texture2D sheet, SpriteFont font, float dt);
}