using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[ExecuteInEditMode]
public class CircleColliderGizmo : MonoBehaviour
{
    private CircleCollider circleCollider;
    public int segments = 40;
    public LineRenderer lineRenderer;

    private void OnEnable()
    {
        circleCollider = GetComponent<CircleCollider>();
        lineRenderer.enabled = true;
    }

    private void Update()
    {
        if (circleCollider != null)
        {
            DrawWireFrame();
        }
    }

    void DrawWireFrame()
    {
        lineRenderer.positionCount = segments + 1;
        float angle = 20f;
        for (int i = 0; i < (segments + 1); i++)
        {
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * circleCollider.radius + transform.position.x;
            float y = Mathf.Cos(Mathf.Deg2Rad * angle) * circleCollider.radius + transform.position.y;
            lineRenderer.SetPosition(i, new Vector3(x, y, 0));
            angle += (360f / segments);
        }
    }

    private void OnDisable()
    {
        lineRenderer.enabled = false;
    }
}
