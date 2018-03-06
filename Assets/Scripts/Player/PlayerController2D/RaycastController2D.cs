using System.Collections;
using UnityEngine;

[RequireComponent(typeof (Collider2D))]
public class RaycastController2D : MonoBehaviour
{
    [Header("Raycasting Attributes")]
    public int playerCollisionHorizontalRayCount;
    public int playerCollisionVerticalRayCount;

    [HideInInspector]
    public float playerCollisionHorizontalRaySpacing;
    [HideInInspector]
    public float playerCollisionVerticalRaySpacing;

    public const float skinWidth = 0.025f;

    [HideInInspector]
    public BoxCollider2D playerCollider;

    [HideInInspector]
    public ObjectBounds objectBounds;

    private void Start()
    {
        RaycastSetup();
    }

    public virtual void RaycastSetup()
    {
        playerCollider = GetComponent<BoxCollider2D>();

        CalculateRaySpacing();
    }

    public void UpdateObjectBounds()
    {
        Bounds newBounds = playerCollider.bounds;
        newBounds.Expand(skinWidth * -2f);

        objectBounds.bottomLeft = new Vector2(newBounds.min.x, newBounds.min.y);
        objectBounds.bottomRight = new Vector2(newBounds.max.x, newBounds.min.y);
        objectBounds.topLeft = new Vector2(newBounds.min.x, newBounds.max.y);
        objectBounds.topRight = new Vector2(newBounds.max.x, newBounds.max.y);
    }

    public void CalculateRaySpacing()
    {
        Bounds bounds = playerCollider.bounds;
        bounds.Expand(skinWidth * -2f);

        playerCollisionHorizontalRayCount = Mathf.Clamp(playerCollisionHorizontalRayCount, 2, int.MaxValue);
        playerCollisionVerticalRayCount = Mathf.Clamp(playerCollisionHorizontalRayCount, 2, int.MaxValue);

        playerCollisionHorizontalRaySpacing = bounds.size.y / (playerCollisionHorizontalRayCount - 1);
        playerCollisionVerticalRaySpacing = bounds.size.x / (playerCollisionVerticalRayCount - 1);
    }

    public struct ObjectBounds
    {
        public Vector2 topLeft;
        public Vector2 topRight;
        public Vector2 bottomLeft;
        public Vector2 bottomRight;

        public ObjectBounds(Vector2 topLeftBound, Vector2 topRightBound, Vector2 bottomLeftBounds, Vector2 bottomRightBounds)
        {
            this.topLeft = topLeftBound;
            this.topRight = topRightBound;
            this.bottomLeft = bottomLeftBounds;
            this.bottomRight = bottomRightBounds;
        }
    }
}
