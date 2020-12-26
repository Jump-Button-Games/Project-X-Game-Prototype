using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area
{
    //public const float worldStartingPointOnYAxis = -0.5f; // The NW corner room will have the coordinate (0, -0.5f)
    public const float worldStartingPointOnYAxis = 0f;
    public const float roomLengthOnXAxis = 16f;
    public const float roomLengthOnYAxis = 9f;

    public Dictionary<Vector2, string> pointAndRoomName = new Dictionary<Vector2, string>();
}
