using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;  // Speed at which the player moves
    public LayerMask groundLayer;  // The layer used to check if the player is on the ground
    public Transform groundCheck;  // Empty GameObject at the player's feet for ground checking
    public float groundCheckRadius = 0.1f;  // Radius of the ground check

    private Rigidbody2D rb;  // Rigidbody2D component for player movement
    private bool isGrounded;  // Tracks whether the player is grounded
    private bool nearTruck = false;  // Tracks whether the player is near the fire truck

    public enum EquipmentState { None, Hose, Ladder, Axe }  // Equipment states for player (None, Hose, Ladder)
    public EquipmentState currentEquipment = EquipmentState.None;  // Default equipment is None

    private WaterGun waterGun;  // Reference to the WaterGun script for shooting water
    private GameObject currentLadder;  // Reference to the current placed ladder

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();  // Get the Rigidbody2D component attached to the player
        waterGun = GetComponent<WaterGun>();  // Get the WaterGun script attached to the player
    }

    void Update()
    {
        CheckGrounded();  // Check if the player is grounded
        MovePlayer();  // Handle player movement

        // Allow switching equipment only if the player is near the truck
        if (nearTruck && Input.GetKeyDown(KeyCode.E))
        {
            CycleEquipment();  // Switch equipment between Hose, Ladder, or None
        }

        // Handle water shooting with the Hose if the player has the Hose equipped
        if (currentEquipment == EquipmentState.Hose && Input.GetMouseButton(0))
        {
            waterGun.ShootWater();  // Call the ShootWater method from WaterGun
        }

        // Place the ladder if the player has the Ladder equipped and presses "F"
        if (currentEquipment == EquipmentState.Ladder && Input.GetKeyDown(KeyCode.F))
        {
            PlaceLadder();
        }
    }

    void CheckGrounded()
    {
        // Check if the player is grounded using a small overlap circle at the player's feet
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    void MovePlayer()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");  // Get horizontal input (left or right)

        // Update the Rigidbody2D's velocity for horizontal movement (no vertical change here)
        rb.linearVelocity = new Vector2(horizontal * moveSpeed, rb.linearVelocity.y);
    }

    void CycleEquipment()
    {
        // Cycle between equipment (Hose, Ladder, None)
        if (currentEquipment == EquipmentState.None)
        {
            currentEquipment = EquipmentState.Hose;  // Equip the Hose
        }
        else if (currentEquipment == EquipmentState.Hose)
        {
            currentEquipment = EquipmentState.Ladder;  // Equip the Ladder
        }
        else if (currentEquipment == EquipmentState.Ladder)
        {
            currentEquipment = EquipmentState.Axe;  // Equip the Axe
        }
        else
        {
            currentEquipment = EquipmentState.None;  // Unequip all equipment
        }

        Debug.Log("Current Equipment: " + currentEquipment);  // Log the current equipment state
    }

    void PlaceLadder()
    {
        if (currentLadder == null)  // Only place a ladder if one isn't already placed
        {
            // Load the ladder prefab from the Resources folder
            GameObject ladderPrefab = Resources.Load<GameObject>("LadderPrefab");

            // Check if the prefab was successfully loaded
            if (ladderPrefab == null)
            {
                Debug.LogError("LadderPrefab not found! Ensure it is in the Resources folder.");
                return;  // Exit if the prefab is not found
            }

            // Offset the ladder's position by adding to the y-axis (raise it slightly)
            Vector3 ladderPosition = transform.position + new Vector3(0, 1.5f, 0); // Adjust the 1.5f value as needed for the desired height

            // Instantiate the ladder at the adjusted position with no rotation
            currentLadder = Instantiate(ladderPrefab, ladderPosition, Quaternion.identity);
            
            // Set a name for the placed ladder for easier identification
            currentLadder.name = "Placed Ladder";  
            Debug.Log("Ladder placed at: " + ladderPosition);  
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // When the player enters a trigger (e.g., near the fire truck), check if it's the truck
        if (other.CompareTag("FireTruck"))
        {
            nearTruck = true;  // The player is near the truck
            Debug.Log("Near truck");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // When the player exits a trigger (e.g., moves away from the fire truck), check if it's the truck
        if (other.CompareTag("FireTruck"))
        {
            nearTruck = false;  // The player is no longer near the truck
            Debug.Log("Not near truck");
        }
    }
}
