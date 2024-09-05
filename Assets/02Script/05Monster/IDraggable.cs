
public interface IDraggable
{
    bool IsDraggable { get; }
    bool TryDrag();
    bool TryFall();
}