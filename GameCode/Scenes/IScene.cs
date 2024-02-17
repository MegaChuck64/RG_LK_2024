using Microsoft.Xna.Framework.Graphics;

namespace GameCode.Scenes;

public interface IScene
{
    void Update();
    void Draw(SpriteBatch sb, Texture2D sheet);
}