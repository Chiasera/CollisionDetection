using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[ExecuteInEditMode]
public class BoxCollider2DGizmo : MonoBehaviour
{
    private BoxCollider2D boxCollider;

    private void OnEnable()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector2 centerPoint = (boxCollider.corners[0].position + boxCollider.corners[1].position 
            + boxCollider.corners[2].position + boxCollider.corners[3].position) / 4;
        Gizmos.DrawWireCube(centerPoint, new Vector2(boxCollider.corners[0].position.x - boxCollider.corners[1].position.x,
            boxCollider.corners[0].position.y - boxCollider.corners[2].position.y));
    }
}
