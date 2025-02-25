using UnityEngine;

public class CharacterBounds : MonoBehaviour
{
    // Reference to the camera
    public Camera mainCamera;

    // Optional: Allow setting the bounds with some padding (e.g., so the character doesn't touch the edge of the screen)
    public float padding = 0.5f;

    private void Start()
    {
        // Ensure we have a camera reference, if not automatically use the main camera
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private void Update()
    {
        // Clamp the character's position within the camera bounds
        ClampPosition();
    }

    private void ClampPosition()
    {
        // Get the camera's world position for the edges of the screen
        Vector3 minBounds = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane)); // Bottom-left
        Vector3 maxBounds = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.nearClipPlane)); // Top-right

        // Apply the padding to avoid the character touching the screen's edges
        minBounds.x += padding;
        minBounds.y += padding;
        maxBounds.x -= padding;
        maxBounds.y -= padding;

        // Get the current position of the character
        Vector3 currentPosition = transform.position;

        // Clamp the position to stay within the camera's bounds
        currentPosition.x = Mathf.Clamp(currentPosition.x, minBounds.x, maxBounds.x);
        currentPosition.y = Mathf.Clamp(currentPosition.y, minBounds.y, maxBounds.y);

        // Set the clamped position back to the character
        transform.position = currentPosition;
    }
}
