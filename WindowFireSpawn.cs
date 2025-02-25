using UnityEngine;

public class WindowFireSpawner : MonoBehaviour
{
    public GameObject firePrefab; // Fire prefab to be instantiated
    public float fireStartDelay = 5f; // Delay before the fire starts
    public bool canSpread = true; // Controls if fire can spread to other windows
    public int maxFireCount = 3; // Maximum times fire can duplicate before spreading

    private int fireCount = 0; // Tracks how many times the fire has duplicated
    private GameObject currentFire; // Stores the active fire instance

    // Define a proximity range to spread fire (adjust as needed)
    public float spreadRange = 5f;

    void Start()
    {
        // Start fire spawning process after a delay
        Invoke("StartFire", fireStartDelay);
    }

    void StartFire()
    {
        if (firePrefab == null)
        {
            Debug.LogError("Fire Prefab is missing! Assign it in the Inspector.");
            return;
        }

        // Check if FireManager allows a new fire to spawn
        if (FireManager.Instance.CanSpawnFire())
        {
            // Instantiate the fire at the window's position
            currentFire = Instantiate(firePrefab, transform.position, Quaternion.identity);
            fireCount++;

            FireManager.Instance.IncreaseFireCount(); // Increase fire count in FireManager

            Debug.Log("🔥 Fire started at: " + gameObject.name);

            // Start checking fire growth every 5 seconds
            InvokeRepeating("CheckFireGrowth", 5f, 5f);
        }
        else
        {
            Debug.Log("🔥 Fire limit reached. Can't start new fire.");
        }
    }

    void CheckFireGrowth()
    {
        // If fire hasn't reached the maximum count, duplicate it
        if (fireCount < maxFireCount)
        {
            // Grow the fire at the window by duplicating it
            GameObject newFire = Instantiate(firePrefab, transform.position, Quaternion.identity);
            fireCount++;

            FireManager.Instance.IncreaseFireCount(); // Increase fire count in FireManager

            Debug.Log("🔥 Fire grew on: " + gameObject.name);
        }
        else if (canSpread)
        {
            // Once fire has duplicated max times, start spreading
            SpreadFire();
        }
    }

    void SpreadFire()
    {
        // Find all windows in the scene
        WindowFireSpawner[] allWindows = FindObjectsByType<WindowFireSpawner>(FindObjectsSortMode.None);

        foreach (WindowFireSpawner window in allWindows)
        {
            // Only spread to windows that don't already have fire and are within range
            if (window.currentFire == null && Vector2.Distance(transform.position, window.transform.position) <= spreadRange)
            {
                window.StartFire(); // Start fire on the neighboring window
                Debug.Log("🔥 Fire spread to: " + window.gameObject.name);
                break; // Fire spreads only to one window at a time
            }
        }
    }
}
