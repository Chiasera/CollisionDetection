using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.HableCurve;

public struct CollisionInfo2D
{
    public bool hasCollided;
    public Vector2 contactPoint;
    public Vector2 slope;

}
public class TriangleCollider : MonoBehaviour
{
    public Transform[] positions;

    public CollisionInfo2D IsInRightSubtriangle(Vector2 point, float radius)
    {
        Vector2 AB = positions[0].position - positions[1].position;
        Vector2 A = positions[1].position;
        Vector2 AP = point - A;
        float closestDistance = Vector2.Dot(AB.normalized, AP);
        Vector2 closestPoint = A + closestDistance * AB.normalized;
        Debug.DrawLine(point, closestPoint);
        Vector2 impactDirection = (point - closestPoint);
        //Debug.Log(closestDistance);
        if (impactDirection.magnitude * Mathf.Sign(impactDirection.x) < radius
            && point.y < positions[0].position.y
            && point.x > positions[0].position.x)
        {
            return new CollisionInfo2D()
            {
                hasCollided = true,
                contactPoint = closestPoint + impactDirection.normalized * Mathf.Sign(impactDirection.x) * radius,
                slope = positions[1].position - positions[0].position   
            };
        }
        return new CollisionInfo2D()
        {
            hasCollided = false,
            contactPoint = Vector2.zero
        };
    }

    public CollisionInfo2D IsInLeftSubtriangle(Vector2 point, float radius)
    {
        //m is our slope and we add an initial offset to our origin which is the height of the top corner of our triangle
        //we subtract dy instead of adding because it is a deacreasing curve and the slope is negative
        Vector2 AB = positions[0].position - positions[2].position;
        Vector2 A = positions[2].position;
        Vector2 AP = point - A;
        float closestDistance = Vector2.Dot(AB.normalized, AP);
        Vector2 closestPoint = A + closestDistance * AB.normalized;
        Debug.DrawLine(point, closestPoint);
        Vector2 impactDirection = (point - closestPoint);
        if (-impactDirection.magnitude * Mathf.Sign(impactDirection.x) < radius
            && point.y < positions[0].position.y
            && point.x < positions[0].position.x)
        {
            return new CollisionInfo2D()
            {
                hasCollided = true,
                contactPoint = closestPoint - impactDirection.normalized * Mathf.Sign(impactDirection.x) * radius,
                slope = positions[2].position - positions[0].position
            };
        }
        return new CollisionInfo2D()
        {
            hasCollided = false,
            contactPoint = Vector2.zero
        };
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(positions[0].position, positions[1].position);
        Gizmos.DrawLine(positions[1].position, positions[2].position);
        Gizmos.DrawLine(positions[2].position, positions[0].position);
    }

    public bool isAboveBase(Vector2 point)
    {
        return point.y > positions[1].position.y && point.y > positions[2].position.y;
    }
}
