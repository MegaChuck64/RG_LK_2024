namespace GameCode.MapSystem;

public class MapItem : Tile
{
    public int X { get; set; }
    public int Y { get; set; }
    public bool Collectable { get; set; }
}