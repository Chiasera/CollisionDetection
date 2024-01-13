using System.Threading.Tasks;
using UnityEngine;

public class Vertex : MonoBehaviour
{
    private FishBody mainBody;
    public Vector2 currentPosition;
    public Vector2 previousPosition;
    private Vector2 initialPosition;
    private const float vertexRadius = 0.1f;
    GameManager gameManager;

    private void Start()
    {
        // Initialize variables and references
        currentPosition = transform.position;
        mainBody = GetComponentInParent<FishBody>();
        initialPosition = transform.position - mainBody.eye.position;
        gameManager = FindObjectOfType<GameManager>();
    }

    private void OnEnable()
    {
        // Reinitialize variables and references when object is enabled
        mainBody = GetComponentInParent<FishBody>();
        initialPosition = transform.position - mainBody.eye.position;
    }

    private void OnDrawGizmos()
    {
        // Draw a yellow sphere gizmo for visualization
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, vertexRadius);
    }

    private void Update()
    {
        // Apply constraints and handle ground collision
        ApplyConstraints();
        HandleGroundCollision();
    }

    public void UpdateVelocity(Vector2 velocity)
    {
        // Update vertex position based on velocity
        Vector2 newPosition = new Vector2(transform.position.x + velocity.x * Time.deltaTime,
            transform.position.y + velocity.y * Time.deltaTime);
        transform.position = newPosition;
    }

    public void ApplyConstraints()
    {
        if (mainBody == null)
        {
            mainBody = GetComponentInParent<FishBody>();
        }

        Vector2 eyePosition = mainBody.eye.position;
        Vector2 rotationOffset = GetRotationOffset();
        Vector2 offset = new Vector2(eyePosition.x + rotationOffset.x,
            eyePosition.y + Wobble(20f, 0.2f, 0.5f) + rotationOffset.y);

        // Apply the constrained displacement to the current position
        transform.position = offset;
    }

    public float Wobble(float frequency, float amplitude, float wavelength)
    {
        float distanceFromTail = transform.position.x - mainBody.tail.position.x;
        float t = distanceFromTail / (mainBody.head.position.x - mainBody.tail.position.x);
        float l = transform.root.lossyScale.x < 0 ? Mathf.Lerp(1, 0, t) : Mathf.Lerp(1, 0, t);
        float w = wavelength * l + Time.time * frequency;
        float wave = Mathf.Sin(w) * l * amplitude;
        return wave;
    }

    public Vector2 GetRotationOffset()
    {
        // Calculate the new position along the circular path
        float angleRadians = mainBody.eye.rotation.eulerAngles.z * Mathf.Deg2Rad;

        // Create the 2x2 rotation matrix
        float cosTheta = Mathf.Cos(angleRadians);
        float sinTheta = Mathf.Sin(angleRadians);

        // Apply the rotation transformation to the original point
        float newX = cosTheta * initialPosition.x - sinTheta * initialPosition.y;
        float newY = sinTheta * initialPosition.x + cosTheta * initialPosition.y;
        return new Vector2(newX, newY);
    }

    private void HandleGroundCollision()
    {
        if (gameManager.obstacle == null)
        {
            gameManager.obstacle = FindObjectOfType<TriangleCollider>();
        }

        if (transform.position.y < gameManager.terrainHeight && transform.position.x > gameManager.playgroundLeftBound
            && transform.position.x < gameManager.playgroundRightBound)
        {
            // Adjust the vertex position to stay inside the terrain
            transform.position = new Vector2(transform.position.x, gameManager.terrainHeight - 0.15f);
        }

        // Check for collision with the obstacle and adjust position if necessary
        CollisionInfo2D rightSideCollision = gameManager.obstacle.IsInRightSubtriangle(transform.position, 0.15f);
        CollisionInfo2D leftSideCollision = gameManager.obstacle.IsInLeftSubtriangle(transform.position, 0.15f);

        if (rightSideCollision.hasCollided)
        {
            transform.position = rightSideCollision.contactPoint;
        }
        else if (leftSideCollision.hasCollided)
        {
            transform.position = leftSideCollision.contactPoint;
        }
    }
}
