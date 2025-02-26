using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class BuildingManager : MonoBehaviour
{
    [Header("Building Setup")]
    [Tooltip("Main collider defining the building's area.")]
    public BoxCollider2D buildingArea;  // Collider to define the building's area (used for bounds)

    [Header("Windows & Doors")]
    [SerializeField, Tooltip("List of windows inside the building.")]
    private List<Window> windows = new List<Window>();  // List of windows in the building
    [Tooltip("The door that the player will break down for rescue.")]
    public Transform door;  // Door transform for reference

    [Header("Fire System")]
    [Tooltip("The maximum number of fires allowed in the scene at once.")]
    public int maxFires = 5;  // Maximum number of fires allowed in the building at once

    [Tooltip("The initial windows where fire will start.")]
    public List<int> startingFireWindows = new List<int>();  // List of windows where fire will start initially
    [Tooltip("Current number of active fires.")]
    [SerializeField] private int currentFires = 0;  // Number of currently active fires

    [Header("Fire Settings")]
    [Tooltip("The intensity of the fire growth rate.")]
    public float fireIntensity = 10f;  // Fire intensity (growth rate)
    [Tooltip("Cooldown time before fire can spread again.")]
    public float fireSpreadCooldown = 5f;  // Fire spread cooldown in seconds
    private float lastFireSpreadTime = 0f;  // Timer to track fire spread cooldown
    private float fireSpreadInterval = 5f;  // Interval between fire spreads
    [Tooltip("Chance of fire spreading to adjacent windows.")]
    public float spreadChance = 0.5f;  // Chance of fire spreading to adjacent windows

    [Header("Fire Events")]
    public UnityEvent onFireSpread;  // Event triggered when fire spreads

    [Header("Fire Effects")]
    public GameObject fireEffectPrefab;  // Placeholder for fire effect prefab (for particle effects)

    private List<Fire> activeFires = new List<Fire>();  // List of active fires in the building
    private List<Window> windowsOnFire = new List<Window>();  // List of windows that are currently on fire

    // Called at the start to initialize the building
    private void Start()
    {
        InitializeWindows();  // Initialize windows with numbering and fire setup
        StartInitialFires();  // Start the initial fires based on the startingFireWindows list
}

// Called once per frame to update the fire status
private void Update()
{
    // Update all active fires in the building
    UpdateFire();  // Update all active fires
    
    fireSpreadCooldown -= Time.deltaTime;

    // If the cooldown is over, check for fire spread conditions
    if (Time.time - lastFireSpreadTime >= fireSpreadInterval)
    {
        // Reset cooldown
        lastFireSpreadTime = Time.time; // Reset the last fire spread time

        // Iterate through each window to check if it should catch fire
        foreach (var window in windows)
        {
            // Check if the window is currently unburned and close enough to a burning window
            if (!window.IsBurning && IsNearby(window))
            {
                // If fire spreads, start the fire on this window
                StartFireInWindow(window);
            }
        }
    }
}

private bool IsNearby(Window window)
{
    // Logic to determine if the window is close enough to a burning window
    // This might involve distance checks, angle checks, etc.
    return window.DistanceToBurningWindow < 5f; // Example distance threshold, adjust based on your needs
}

    // Initializes the windows with proper numbering and sets their names
    private void InitializeWindows()
    {
        for (int i = 0; i < windows.Count; i++)
        {
            Window window = windows[i];
            window.windowNumber = i + 1;  // Set window number to a 1-based index
            window.isOnFire = false;  // Initially, no window is on fire

            if (window.windowTransform != null)
            {
                window.windowTransform.name = $"Window_{window.windowNumber}";  // Set window name for reference
            }
        }
        Debug.Log($"Building initialized with {windows.Count} windows. Door located at {door?.position}");
    }

    // Starts fire in the initial windows
    private void StartInitialFires()
    {
        foreach (int fireWindowIndex in startingFireWindows)
        {
            StartFireInWindow(windows[fireWindowIndex]);  // Start fire in the specified windows
        }
    }

    // Spread fire to adjacent windows
    public void SpreadFire(Fire fire)
    {
        if (Time.time - lastFireSpreadTime < fireSpreadCooldown) return;
        // If the maximum number of fires is reached, stop spreading
        if (activeFires.Count >= maxFires) return;  // Stop spreading if maximum fires are reached

        // Ensure enough time has passed to spread fire again (faster spread)

        lastFireSpreadTime = Time.time;  // Update the last fire spread time

        int windowIndex = fire.window.windowNumber;  // Get the current window's index
        List<int> potentialSpreads = new List<int>();  // List of potential windows to spread fire

        // Add adjacent windows (left and right) if they exist
        if (windowIndex > 1) potentialSpreads.Add(windowIndex - 1);  // Left window
        if (windowIndex < windows.Count) potentialSpreads.Add(windowIndex + 1);  // Right window
        if (windowIndex > 2) potentialSpreads.Add(windowIndex - 2);  // Two windows left
        if (windowIndex < windows.Count - 1) potentialSpreads.Add(windowIndex + 2);  // Two windows right

        // If there are any valid windows to spread to, pick one randomly
        bool shouldSpread = Random.Range(0f, 1f) < spreadChance;
        if (potentialSpreads.Count > 0 && shouldSpread)  // Increased chance of spread
        {
            int randomIndex = Random.Range(0, potentialSpreads.Count);  // Randomly pick one of the potential spreads
            StartFireInWindow(windows[potentialSpreads[randomIndex] - 1]);  // Start fire at the selected window
            currentFires = activeFires.Count;  // Update the current fires count
            UpdateFireCount();  // Update the fire count in UI or other parts
        }

        // Trigger the fire spread event (e.g., for sound effects or particle effects)
        onFireSpread?.Invoke();
    }

    // Start fire in a given window
    private void StartFireInWindow(Window targetWindow)
    {

        // If the window does not already have fire, create a new fire
        if (!targetWindow.isOnFire)
        {
            Fire newFire = new Fire(targetWindow, this);  // Create a new fire instance
            targetWindow.fire = newFire;  // Assign the fire to the window
            targetWindow.isOnFire = true;  // Mark window as on fire
            activeFires.Add(newFire);  // Add the fire to the active fires list
            windowsOnFire.Add(targetWindow);  // Add the window to the list of windows on fire

            // Spawn particle effect (fire effect) at the window's position
            SpawnFireEffect(targetWindow.windowTransform.position);
        }
        else
        {
            targetWindow.fire.IncreaseFireIntensity();  // If fire already exists, increase its intensity
        }

        currentFires = activeFires.Count;  // Update the current number of fires
        UpdateFireCount();  // Update the count of active fires
    }

    // Spawn fire effect (particle system or placeholder)
    private void SpawnFireEffect(Vector3 position)
    {
        if (fireEffectPrefab != null)
        {
            Instantiate(fireEffectPrefab, position, Quaternion.identity);  // Instantiate fire effect at the window's position
        }
    }

    // Update all active fires
    private void UpdateFire()
    {
        // Loop through all active fires in the building
        for (int i = 0; i < activeFires.Count; i++)
        {
            Fire fire = activeFires[i];  // Get the current fire from the list

            // Update the fire's health (intensity) over time based on the fire's growth rate
            fire.UpdateFire(fire.fireGrowthRate);

            // Check if the fire has reached its maximum health (intensity) and hasn't duplicated yet
            if (fire.fireHealth >= fire.maxFireHealth && !fire.isDuplicating)
            {
                // Spread the fire to adjacent windows
                SpreadFire(fire);

                // Cap the fire's health at the maximum value after spreading
                fire.fireHealth = fire.maxFireHealth;
            }
        }
    }

    // Update fire count (e.g., for display or limit checks)
    private void UpdateFireCount()
    {
        Debug.Log($"Current fires: {currentFires}/{maxFires}");
    }


// The Window class representing a window in the building
[System.Serializable]
public class Window
{
    public Transform windowTransform;  // Transform for positioning and reference
    public int windowNumber;  // The number assigned to the window (1-based)
    public bool isOnFire;  // Flag to check if the window is currently on fire
    public bool IsBurning => isOnFire;  // Property to check if the window is burning
    public Fire fire;  // The fire instance (if any) on the window
    public float DistanceToBurningWindow { get; set; }  // Distance to the nearest burning window
}

// The Fire class representing a fire in the building
public class Fire
{
    public Window window;  // The window this fire belongs to
    public bool isDuplicating;  // Flag to track if fire is duplicating
    public float fireHealth;  // Health or intensity of the fire
    public float maxFireHealth = 100f;  // Max fire intensity
    public float fireGrowthRate = 10f;  // Growth rate of fire intensity

    public float fireIntensity;  // Intensity of the fire
    public float fireSpreadThreshold = 50f;  // Threshold for fire to spread
    public float intensityIncreaseRate = 5f;  // Rate at which fire intensity increases

    public Fire(Window targetWindow, BuildingManager manager)
    {
        window = targetWindow;
        fireHealth = 0f;
        isDuplicating = false;
        fireIntensity = 0f;
    }

    // Increase fire intensity over time
    public void IncreaseFireIntensity()
    {
        fireIntensity += intensityIncreaseRate * Time.deltaTime;

        // Check if fire intensity reaches a certain threshold to spread
        if (fireIntensity >= fireSpreadThreshold)
        {
            SpreadFire();
        }
    }

    // Update fire with its growth rate
    public void UpdateFire(float growthRate)
    {
        fireHealth += growthRate * Time.deltaTime;  // Update fire health
    }

        // Spread fire to adjacent windows
        private void SpreadFire()
        {
            // Logic to spread fire to adjacent windows
        }
    }
}
