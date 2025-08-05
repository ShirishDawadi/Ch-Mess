using JetBrains.Annotations;
using UnityEngine;

public class PieceDragAndDrop : MonoBehaviour
{

    private Vector3 startPosition;
    private Vector3 offset;
    private bool isDraggable = true;
    private float tileSize = 1.0f; // Adjust based on your grid size
    void OnMouseDown()
    {
        if (!this.enabled || !isDraggable) return;
        startPosition = transform.position;
        offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
    void OnMouseDrag()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.nearClipPlane;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        if (NotCollision(worldPosition))
        {
            transform.position = worldPosition + offset;
        } 
    }
    void OnMouseUp()
    {
        if (!this.enabled || !isDraggable) return;
        // Snap to Grid logic here
        if (IsValidPlacement(transform.position) && NotCollision(transform.position))
        {
            SnapToGrid();
        }
        else
        {
            transform.position = startPosition; // Return to original position if placement is invalid
        }
    }
    private bool NotCollision(Vector3 position)
    {
        // Check for collisions with other pieces (you'll need to implement this)
        Collider2D[] colliders = Physics2D.OverlapPointAll(position);
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject != gameObject)
            {
                return false;
            }
        }
        return true;
    }
    private bool IsValidPlacement(Vector3 position)
    {
            if (gameObject.CompareTag("blackpiece"))
            {
                if (position.x < -0.3 || position.x > 4.3 || position.y < 4.7 || position.y > 5.3)
                {
                    return false;
                }
            }
            else if (gameObject.CompareTag("whitepiece"))
            {
                if (position.x < -0.3 || position.x > 4.3 || position.y < -0.7 || position.y > 0.3)
                {
                    return false;
                }
            }
            else if (gameObject.CompareTag("blackpawn"))
            {
                if (position.x < -0.3 || position.x > 4.3 || position.y < 3.7 || position.y > 4.3)
                {
                    return false;
                }
            }
            else if (gameObject.CompareTag("whitepawn"))
            {

                if (position.x < -0.3 || position.x > 4.3 || position.y < 0.7 || position.y > 1.3)
                {
                    return false;
                }
            }
        return true;
    }
    private void SnapToGrid()
    {
        float gridX = Mathf.Round(transform.position.x / tileSize) * tileSize;
        float gridY = Mathf.Round(transform.position.y / tileSize) * tileSize;

        transform.position = new Vector3(gridX, gridY, 0);
        
    }
    public void DisableDragging()
    {
        isDraggable = false; // Externally disable dragging
    }
}
