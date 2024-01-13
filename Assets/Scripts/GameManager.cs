using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject blueFishPrefab;
    public GameObject redFishPrefab;
    public Transform[] spawnAreaAnchors;
    public static CircleCollider[] circleColliders;
    public readonly float terrainHeight = 2.726324f;
    public TriangleCollider obstacle;
    public readonly float playgroundLeftBound = -3f;
    public readonly float playgroundRightBound = 15.5f;

    public void OnInputSpawnFish()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            float randomSpawnPosX = Random.Range(spawnAreaAnchors[0].position.x, spawnAreaAnchors[1].position.x);
            Instantiate(redFishPrefab, new Vector3(randomSpawnPosX, spawnAreaAnchors[0].position.y, 0), Quaternion.identity);

        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            float randomSpawnPosX = Random.Range(spawnAreaAnchors[2].position.x, spawnAreaAnchors[3].position.x);
            Instantiate(blueFishPrefab, new Vector3(randomSpawnPosX, spawnAreaAnchors[0].position.y, 0), Quaternion.identity);
        }
    }

    private void Start()
    {
        obstacle = FindObjectOfType<TriangleCollider>();
    }

    private void Update()
    {
        OnInputSpawnFish();
        HandleDynamicCollisions();
    }

    private void HandleDynamicCollisions()
    {
        circleColliders = FindObjectsOfType<CircleCollider>();
        for (int i = 0; i < circleColliders.Length; i++)
        {
            for (int j = i + 1; j < circleColliders.Length; j++)
            {
                CircleCollider collider1 = circleColliders[i];
                CircleCollider collider2 = circleColliders[j];

                if (collider1 != null && collider2 != null && collider1.gameObject.layer == 3 && collider2.gameObject.layer == 3)
                {
                    // Calculate relative velocity and position
                    Vector2 relativeVelocity = collider1.GetComponent<FishController>().Velocity - collider2.GetComponent<FishController>().Velocity;
                    Vector2 relativePosition = collider1.transform.position - collider2.transform.position;
                    float totalRadius = collider1.radius + collider2.radius;

                    // Calculate the dot product of relative position and relative velocity
                    //if less than 0, it means they are going into each other
                    float dotProduct = Vector2.Dot(relativePosition, relativeVelocity);

                    if (dotProduct < 0)
                    {
                        // Objects are moving towards each other
                        float distance = relativePosition.magnitude;

                        if (distance < totalRadius)
                        {
                            // Collision occurred
                            Vector2 collisionNormal = relativePosition.normalized;

                            // Separate the objects to avoid overlap
                            float separationDistance = (totalRadius - distance) * 0.5f;
                            Vector2 currentTransform1 = collider1.transform.position;
                            collider1.transform.position = currentTransform1 + separationDistance * collisionNormal;
                            Vector2 currentTransform2 = collider2.transform.position;
                            collider2.transform.position = currentTransform2 - separationDistance * collisionNormal;

                            // Apply impulses to change velocities
                            collider1.GetComponent<FishController>().Bounce(collisionNormal);
                            collider2.GetComponent<FishController>().Bounce(-collisionNormal);                        
                        }
                    }
                }
            }
        }
    }
}
