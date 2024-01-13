using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxCollider2D : MonoBehaviour
{
    public Transform[] corners;

    private void Start()
    {
        Debug.Log(corners.Length);
    }

    public CollisionInfo2D IsPointInBox(Vector2 point, float radius)
    {
        if (point.y < corners[0].position.y && point.y < corners[1].position.y
            && point.x > corners[2].position.x && point.x < corners[3].position.x)
        {
            return new CollisionInfo2D
            {
                hasCollided = true,
                contactPoint = new Vector2(point.x, corners[0].position.y + radius)

            };
        }
        return new CollisionInfo2D
        {
            hasCollided = false,
            contactPoint = Vector2.zero

        };
    }
}
