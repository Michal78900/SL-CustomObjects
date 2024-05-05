/// <summary>
/// Enum flags that are used to determine primitive extra properties.
/// </summary>
[System.Flags]
public enum PrimitiveFlags : byte
{
    /// <summary>
    /// No extra properties.
    /// </summary>
    None = 0,

    /// <summary>
    /// Primitive has its collisions enabled, meaning players and objects can collide with it.
    /// </summary>
    Collidable = 1,

    /// <summary>
    /// Primitive has its renderer enabled and players can see it.
    /// </summary>
    Visible = 2,
}