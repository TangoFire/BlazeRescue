using UnityEngine;
using System.Collections.Generic;

public class Window : MonoBehaviour
{
    public GameObject firePrefab; // Fire prefab to be instantiated
    public float fireStartDelay = 5f; // Delay before the fire starts
    public bool canSpread = true; // Controls if fire can spread to other windows
    public int maxFireCount = 3; // Maximum times fire can duplicate before spreading

    private int fireCount = 0; // Tracks how many times the fire has duplicated
    private GameObject currentFire; // Stores the active fire instance
    public bool isOnFire = false; // Tracks if the window is currently on fire

    // The two windows this fire can spread to
    public Window exit1;
    public Window exit2;

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

        // Check if a new fire can be spawned based on the fire manager's limit
        if (FireManager.Instance.CanSpawnFire())
        {
            // Instantiate fire at this window's position
            currentFire = Instantiate(firePrefab, transform.position, Quaternion.identity);
            fireCount++;
            FireManager.Instance.IncreaseFireCount();
            isOnFire = true; // Mark window as burning

            Debug.Log("🔥 Fire started at: " + gameObject.name);

            // Schedule fire growth checks every 5 seconds
            InvokeRepeating("CheckFireGrowth", 5f, 5f);
        }
        else
        {
            Debug.Log("🔥 Fire limit reached. Can't start new fire.");
        }
    }

    void CheckFireGrowth()
    {
        // If fire has not reached its max growth, create another fire instance
        if (fireCount < maxFireCount)
        {
            GameObject newFire = Instantiate(firePrefab, transform.position, Quaternion.identity);
            fireCount++;
            FireManager.Instance.IncreaseFireCount();
            Debug.Log("🔥 Fire grew on: " + gameObject.name);
        }
        else if (canSpread)
        {
            // Once max fire count is reached, attempt to spread
            SpreadFire();
        }
    }

    void SpreadFire()
    {
        // Spread fire to one of the designated exit windows if they are not already burning
        if (exit1 != null && !exit1.isOnFire)
        {
            exit1.StartFire();
            Debug.Log("🔥 Fire spread to: " + exit1.gameObject.name);
        }
        else if (exit2 != null && !exit2.isOnFire)
        {
            exit2.StartFire();
            Debug.Log("🔥 Fire spread to: " + exit2.gameObject.name);
        }
    }

    // Extinguish the fire manually, stopping its spread and removing the fire instance
    public void ExtinguishFire()
    {
        if (currentFire != null)
        {
            Destroy(currentFire); // Remove fire effect
            fireCount = 0; // Reset fire growth count
            isOnFire = false; // Mark window as safe
            CancelInvoke("CheckFireGrowth"); // Stop further fire growth checks
            Debug.Log("💧 Fire extinguished at: " + gameObject.name);
        }
    }
}
