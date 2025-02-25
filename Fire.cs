using UnityEngine;

public class Fire : MonoBehaviour
{
    [Header("Fire Growth Settings")]
    public float initialSize = 0.5f; // Initial size of the fire
    public float maxSize = 1.5f; // Maximum size the fire can grow
    public float growthRate = 0.05f; // How much the fire grows each second
    public float spreadCooldown = 3f; // Cooldown time between fire spreads

    [Header("Fire Duplication Settings")]
    public int maxDuplications = 3; // How many times this fire can duplicate
    public GameObject firePrefab; // The fire prefab to spawn

    [Header("Fire Extinguishing Settings")]
    public float fireHealth = 100f; // Total health of the fire
    public float waterEffectiveness = 10f; // How much water reduces fire health

    private int duplicationCount = 0; // Tracks the number of times this fire has spread
    private bool isExtinguished = false; // Whether the fire is extinguished or not
    private float nextSpreadTime = 0f; // Timer for spread cooldown

    // Called when the script starts
    void Start()
    {
        transform.localScale = Vector3.one * initialSize; // Set the initial size of the fire
    }

    // Called once per frame to update fire behavior
    void Update()
    {
        // Stop updating if the fire is extinguished
        if (isExtinguished) return;

        // Gradually grow the fire until it reaches max size
        if (transform.localScale.x < maxSize)
        {
            transform.localScale += Vector3.one * (growthRate * Time.deltaTime);
        }

        // Allow the fire to spread only if it has reached max size and the cooldown is over
        if (transform.localScale.x >= maxSize && duplicationCount < maxDuplications)
        {
            if (Time.time >= nextSpreadTime)
            {
                DuplicateFire(); // Attempt to duplicate the fire
                nextSpreadTime = Time.time + spreadCooldown; // Set the next spread time
            }
        }
    }

    // Method to duplicate the fire and spread it to nearby areas
    void DuplicateFire()
    {
        // If no fire prefab is assigned, log an error and return
        if (firePrefab == null)
        {
            Debug.LogError("Fire Prefab is missing!");
            return;
        }

        // Check if the current fire has already reached the duplication limit
        if (FireManager.Instance.GetFireCount() >= FireManager.Instance.maxFires)
        {
            Debug.Log("🔥 Fire limit reached. Cannot spread further.");
            return; // Prevent further spreading if the max fire count is reached
        }

        duplicationCount++; // Increment the duplication count

        // Check nearby objects for potential windows where the fire can spread
        Collider2D[] nearbyObjects = Physics2D.OverlapCircleAll(transform.position, 2f);
        foreach (Collider2D obj in nearbyObjects)
        {
            if (obj.CompareTag("Window") && obj.transform.childCount == 0) // Ensure the window is empty
            {
                // Instantiate the fire at the window position and attach it to the window
                GameObject newFireForWindow = Instantiate(firePrefab, obj.transform.position, Quaternion.identity);
                newFireForWindow.transform.parent = obj.transform;

                // Increment the fire count in the FireManager
                FireManager.Instance.IncrementFireCount();
                return; // Fire spreads only to one window at a time
            }
        }

        // If no window is available, spread the fire to a random nearby position
        Vector2 newPosition = (Vector2)transform.position + Random.insideUnitCircle * 0.5f;
        GameObject newRandomFire = Instantiate(firePrefab, newPosition, Quaternion.identity);

        // Increment the fire count in the FireManager
        FireManager.Instance.IncrementFireCount();
    }

    // Method to handle the effect of water on the fire
    public void OnWaterTouch(float waterPower)
    {
        fireHealth -= waterPower / waterEffectiveness; // Decrease fire health based on water power

        if (fireHealth <= 0) // If fire health reaches 0 or below, extinguish the fire
        {
            isExtinguished = true;
            FireManager.Instance.DecrementFireCount(); // Decrement the fire count in FireManager
            Destroy(gameObject); // Destroy the fire object
        }
    }
}
