using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CircleCollider : MonoBehaviour
{
    public float radius = 1f;
    public CircleCollider[] circleColliders;
    public BoxCollider2D[] boxColliders;
    public TriangleCollider obstacle;
    public FishController fishController;

    private void OnEnable()
    {
        circleColliders = FindObjectsOfType<CircleCollider>();
        boxColliders = FindObjectsOfType<BoxCollider2D>();
        obstacle = FindObjectOfType<TriangleCollider>();
    }

    private void Start()
    {
        circleColliders = FindObjectsOfType<CircleCollider>();
        boxColliders = FindObjectsOfType<BoxCollider2D>();
    }

    public void Update()
    {
        if (fishController != null)
        {
            OnFishStaticCollision();
        }
    }

    //We include the static collision handling in the circle collider code since the environment colliders don't interact with each other
    //The interaction between the circle colliders is done within the gameManager script
    public void OnFishStaticCollision()
    {
        // Check if not already bouncing
        if (!fishController.isBouncing)
        {
            foreach (var target in boxColliders)
            {
                if (target != this)
                {
                    if(this.gameObject.layer != 3)
                    {
                        Debug.Log("VERTEX COLLIDER!!");
                    }
                    CollisionInfo2D collisionInfo = target.IsPointInBox(new Vector2(transform.position.x, transform.position.y - radius), radius);
                    if (collisionInfo.hasCollided)
                    {
                        Vector2 impactDirection = new Vector2(fishController.Velocity.x, -fishController.Velocity.y);
                        if (target.gameObject.layer == 4)
                        {
                            // land on water
                            transform.position = collisionInfo.contactPoint;
                            fishController.Bounce(impactDirection.normalized);
                            Destroy(transform.root.gameObject);

                        }
                        else // landed on ground
                        {
                            transform.position = collisionInfo.contactPoint;
                            fishController.Bounce(impactDirection.normalized);
                        }
                    }
                }
            }

            if (obstacle != null)
            {
                CollisionInfo2D rightSideCollision = obstacle.IsInRightSubtriangle(transform.position, radius);
                CollisionInfo2D leftSideCollision = obstacle.IsInLeftSubtriangle(transform.position, radius);
                float randomBounceAngle = Random.Range(0.25f, 1.5f);
                float impactAngle = Mathf.Atan(fishController.Velocity.x / fishController.Velocity.y);
                float theta = Mathf.Atan(fishController.Velocity.y / fishController.Velocity.x);
                float bounceAngle = 2 * Mathf.Abs(theta) + Mathf.Abs(impactAngle);
                Vector2 impactDirection;
                if (rightSideCollision.hasCollided)
                {
                    Quaternion rotation = Quaternion.AngleAxis(Mathf.Sign(fishController.Velocity.y) * Mathf.Rad2Deg * bounceAngle, Vector3.forward);
                    impactDirection = rotation * -Vector2.right;
                    transform.position = rightSideCollision.contactPoint;
                    fishController.Bounce(impactDirection.normalized);
                    Debug.Log("HITTING OBSTACLE !!!");
                }
                else if (leftSideCollision.hasCollided)
                {
                    Quaternion rotation = Quaternion.AngleAxis(Mathf.Sign(fishController.Velocity.y) * Mathf.Rad2Deg * bounceAngle, Vector3.forward);
                    impactDirection = rotation * Vector2.right;
                    fishController.Bounce(impactDirection.normalized);
                    transform.position = leftSideCollision.contactPoint;
                    Debug.Log("HITTING OBSTACLE !!!");
                }
            }
        }
    }
}
