using UnityEngine;

public class FireManager : MonoBehaviour
{
    // The singleton instance of FireManager, which will be accessed globally
    public static FireManager Instance;

    [Header("Fire Limit Settings")]
    public int maxFires = 250; // Maximum number of fires allowed in the scene

    [Header("Current Fire Count")]
    [SerializeField] private int fireCount = 0; // Tracks the current number of fires in the scene
    public int FireCount => fireCount; // Public getter to access fireCount in other scripts (read-only)

    // Called when the script is initialized to ensure only one instance of FireManager exists
    private void Awake()
    {
        // If the instance is not set yet, set it to this instance
        if (Instance == null)
        {
            Instance = this;
        }
        // If another instance already exists, destroy this object to enforce singleton pattern
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Method to retrieve the current number of fires
    public int GetFireCount()
    {
        return fireCount; // Simply returns the current number of fires
    }

    // Method to increment the fire count when a new fire is spawned
    public void IncrementFireCount()
    {
        fireCount++; // Increments the fireCount by 1 whenever a new fire is created
    }

    // Method to decrement the fire count when a fire is extinguished
    public void DecrementFireCount()
    {
        fireCount--; // Decrements the fireCount by 1 whenever a fire is extinguished
    }

    // Check if new fire can spawn, based on the current fire count and max fires limit
    public bool CanSpawnFire()
    {
        return fireCount < maxFires; // Returns true if fireCount is less than the maxFires limit
    }

    // Increment the fire count directly (can be used in other scripts if needed)
    public void IncreaseFireCount()
    {
        fireCount++; // Increments the fireCount directly by 1
    }

    // Optional: Update fireCount based on the number of "Fire" tagged objects in the scene
    private void Update()
    {
        // Updates the fireCount every frame by counting all GameObjects with the "Fire" tag
        fireCount = GameObject.FindGameObjectsWithTag("Fire").Length;
    }
}
