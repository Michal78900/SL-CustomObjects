using UnityEngine;

public class SerializableVector
{
    public SerializableVector()
    {
    }

    public SerializableVector(float x, float y)
    {
        this.x = x;
        this.y = y;
        this.z = null;
    }

    public SerializableVector(float x, float y, float? z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public float x { get; set; }

    public float y { get; set; }

    public float? z { get; set; }


    public static implicit operator SerializableVector(Vector3 vector) => new SerializableVector(vector.x, vector.y, vector.z);

    public static implicit operator Vector3(SerializableVector vector) => new Vector3(vector.x, vector.y, vector.z.Value);


    public static implicit operator SerializableVector(Vector2 vector) => new SerializableVector(vector.x, vector.y);

    public static implicit operator Vector2(SerializableVector vector) => new Vector2(vector.x, vector.y);
}
