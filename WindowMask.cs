using UnityEngine;

public class WindowMask : MonoBehaviour
{
    // Flag to track whether painting mode is active
    public bool paintingWindows = false;

    // The texture of the building (must be assigned in the inspector)
    public Texture2D buildingTexture;

    // Grid to track which tiles are painted
    public bool[,] paintedTiles;

    // Grid dimensions (settable in the Inspector)
    public int gridSizeX = 10;  // Number of columns
    public int gridSizeY = 10;  // Number of rows

    // Size of each tile in world units
    public float tileSize = 1f;

    private void Awake()
    {
        // Initialize the grid storage based on grid size
        paintedTiles = new bool[gridSizeX, gridSizeY];
    }

    private void Update()
    {
        // Toggle painting mode when "P" key is pressed
        if (Input.GetKeyDown(KeyCode.P))
        {
            ToggleWindowPainting();
        }

        // Paint when left mouse button is clicked
        if (paintingWindows && Input.GetMouseButtonDown(0))
        {
            PaintAtMousePosition();
        }
    }

    // Toggle painting mode
    public void ToggleWindowPainting()
    {
        paintingWindows = !paintingWindows;
        Debug.Log("Window Painting Mode: " + (paintingWindows ? "ON" : "OFF"));
    }

    // Paint a window at the given grid coordinates
    public void PaintWindow(int x, int y)
    {
        // Ensure the coordinates are within bounds
        if (buildingTexture != null && x >= 0 && x < gridSizeX && y >= 0 && y < gridSizeY)
        {
            // Toggle the painted state of the tile
            paintedTiles[x, y] = !paintedTiles[x, y];

            // Force Scene View to update immediately
            UnityEditor.SceneView.RepaintAll();
        }
    }

    // Converts the mouse position into grid coordinates and paints
    private void PaintAtMousePosition()
    {
        // Convert mouse position to world position
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0; // Ensure it's in 2D space

        // Convert to local grid coordinates
        int x = Mathf.FloorToInt((mousePos.x - transform.position.x) / tileSize);
        int y = Mathf.FloorToInt((mousePos.y - transform.position.y) / tileSize);

        // Paint at the computed grid position
        PaintWindow(x, y);
    }
}
