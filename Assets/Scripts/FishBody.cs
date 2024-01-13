using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class FishBody : MonoBehaviour
{
    public Transform tail;
    public Transform head;
    public Vertex[] vertices;
    private LineRenderer lineRenderer;
    public Transform eye;
    [Category("Fish motion")]
    [Range(0.1f, 5f)]
    public float wobbleAmplitude;
    [Range(0.1f, 5f)]
    public float wobbleFrequency;

    void OnEnable()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        DrawWireFrame();
    }

    void DrawWireFrame()
    {
        lineRenderer.positionCount = vertices.Length + 1;
        for (int i = 0; i < vertices.Length; i++)
        {
            float x = vertices[i].transform.position.x;
            float y = vertices[i].transform.position.y;
            lineRenderer.SetPosition(i, new Vector3(x, y, 0));
        }
        lineRenderer.SetPosition(vertices.Length, vertices[0].transform.position);
    }
}
