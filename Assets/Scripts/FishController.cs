using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class FishController : MonoBehaviour
{
    [Range(-100f, 100f)]
    public float initialSpeed = 0.5f;
    public Vector2 launchAngleRandom;
    private Vector2 velocity2D = Vector2.zero;
    private float time = 0f;
    public float gravity = 9.8f;
    private Vector2 initialVelocity;
    public bool hasCollided = false;
    [Range(1.0f, 200f)]
    [SerializeField]
    private float orientationSpeed = 5f;
    public Renderer fishRenderer;
    public bool isBouncing = false;
    public bool isProjectile = true;
    [Range(0.1f, 5f)]
    [SerializeField]
    public float bounciness = 0.5f;
    GameManager gameManager;

    private void OnEnable()
    {
        // Set the initial eye position for rendering
        fishRenderer.material.SetVector("_EyePosition", transform.position);

        // Generate a random launch angle within the specified range
        float launchAngle = UnityEngine.Random.Range(launchAngleRandom.x, launchAngleRandom.y);

        // Calculate initial velocity based on the launch angle
        initialVelocity = new Vector2(initialSpeed * Mathf.Cos(Mathf.Deg2Rad * launchAngle),
            initialSpeed * Mathf.Sin(Mathf.Deg2Rad * launchAngle));

        // Schedule the destruction of this object after a delay
        OnTimerSelfDestroy();
    }

    private void Start()
    {
        // Find and reference the GameManager in the scene
        gameManager = FindObjectOfType<GameManager>();
    }

    private async void OnTimerSelfDestroy()
    {
        // Destroy this object after a delay
        await Task.Delay(10000);
        try
        {
            Destroy(transform.root.gameObject);
        }
        catch
        {
            // Ignore any errors
        }
    }

    public void Update()
    {
        if (isProjectile)
        {
            // Update velocity due to gravity if it's a projectile
            velocity2D = new Vector2(initialVelocity.x, initialVelocity.y - gravity * time);
        }

        // Check if the object is out of bounds and destroy it
        if (transform.position.x > gameManager.playgroundRightBound + 5 || transform.position.x < gameManager.playgroundLeftBound - 5)
        {
            Destroy(transform.root.gameObject);
        }

        // Update the object's position and eye position for rendering
        UpdatePosition(velocity2D);
        fishRenderer.material.SetVector("_EyePosition", transform.position);
    }

    public async void Bounce(Vector2 impactDirection)
    {
        isBouncing = true;
        Vector2 newInitialVelocity;

        if (initialVelocity.magnitude < 0.5f)
        {
            // If initial velocity is very low, disable gravity and use custom slope motion
            gravity = 0;
            newInitialVelocity = GetSlopeMotion(impactDirection);
        }
        else
        {
            // Otherwise, apply bounce using the specified bounciness
            gravity = 9.8f;
            newInitialVelocity = impactDirection * velocity2D.magnitude * bounciness;
        }

        // Reset the motion with the new initial velocity
        ResetMotion(transform.position, newInitialVelocity);

        // Delay for a short time
        await Task.Delay(100);

        // Reset bouncing flag
        isBouncing = false;
    }

    public Vector2 GetSlopeMotion(Vector2 velocity)
    {
        // Implement custom slope motion calculation here if needed
        return Vector2.zero;
    }

    private void ResetMotion(Vector2 newPosition, Vector2 newInitialVelocity)
    {
        // Reset motion parameters
        initialVelocity = newInitialVelocity;
        time = 0;
    }

    public void UpdatePosition(Vector2 newVelocity)
    {
        // Drawing a debug ray to visualize the velocity vector.
        Debug.DrawRay(transform.position, velocity2D, Color.yellow);

        // Calculate the rotation needed to point along the velocity vector.
        Quaternion targetRotation = Quaternion.FromToRotation(transform.right, newVelocity.normalized);

        // Interpolate the rotation
        transform.rotation *= targetRotation;

        // Calculate the new position based on velocity
        Vector2 newPosition = new Vector2(transform.position.x + newVelocity.x * Time.deltaTime,
            transform.position.y + newVelocity.y * Time.deltaTime);

        // Update the object's position
        transform.position = new Vector3(newPosition.x, newPosition.y, 0);

        // Increment time
        time += Time.deltaTime;
    }

    public Vector2 Velocity
    {
        get
        {
            return velocity2D;
        }
        set
        {
            velocity2D = value;
        }
    }
}
