using Microsoft.Xna.Framework;

namespace GameCode.MapSystem;

public class Tile
{
    public SpriteType Type { get; set; }
    public Color Tint { get; set; }
    public bool IsSolid { get; set; }
}
