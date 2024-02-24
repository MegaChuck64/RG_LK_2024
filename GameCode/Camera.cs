
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameCode;

public class Camera
{
    public float Zoom { get; private set; }
    public float Rotation { get; set; }
    public Vector2 Position { get; private set; }

    public void Move(Vector2 amount)
    {
        Position += amount;
    }

    public void PerformZoom(float amount)
    {
        Zoom += amount;
    }

    public void Rotate(float angle)
    {
        Rotation += angle;
    }

    public Matrix Transform(GraphicsDevice gd)
    {
        //t r s 
        return Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0)) *
            Matrix.CreateRotationZ(Rotation) *
            Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
            Matrix.CreateTranslation(new Vector3(gd.Viewport.Width * 0.5f, gd.Viewport.Height * 0.5f, 0));
    }
}