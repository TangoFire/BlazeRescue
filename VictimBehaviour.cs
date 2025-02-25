using UnityEngine;

public class VictimBehaviour : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f; // Starting health
    private float currentHealth; // Victim's current health
    public float baseHealthDecayRate = 1f; // Base health decay per second

    [Header("Fire Settings")]
    public string fireTag = "Fire"; // Tag for fire objects in the scene
    public float extraDamagePerFire = 0.5f; // Additional damage per fire in the scene

    [Header("Death Settings")]
    public GameObject deathEffect; // Optional particle effect on death
    public bool destroyOnDeath = true; // Should the victim be destroyed on death?

    [Header("Current Health")]
    [SerializeField] private float currentHealthSerialized; // Health displayed in inspector
    
    [Header("Current Fires")]
    public int currentFireCount;

    void Start()
    {
        currentHealth = maxHealth; // Set health to full at start
        currentHealthSerialized = currentHealth; // Sync serialized health with actual health
        currentFireCount = CountFires(); // Initialize the current fire count
    }

    void Update()
    {
        // Update current fire count based on the number of fires
        currentFireCount = CountFires();

        // Only trigger burn effect if there are fires in the scene
        if (currentFireCount > 0)
        {
            Burn();  // Apply damage over time when there are fires
        }
    }

    /// Counts the number of fires in the scene.
    private int CountFires()
    {
        // Get the number of fire objects using the FireManager singleton
        return FireManager.Instance.FireCount;
    }

    /// Handles victim death.
    void Die()
    {
        Debug.Log(gameObject.name + " has died!");

        // Play death effect if assigned
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        // Destroy victim if enabled
        if (destroyOnDeath)
        {
            Destroy(gameObject);
        }
    }

    /// Applies health decay over time based on the number of fires.
    public void Burn()
    {
        // Adjust decay rate based on fire count
        float totalDecayRate = baseHealthDecayRate + (currentFireCount * extraDamagePerFire);

        // Reduce health over time
        currentHealth -= totalDecayRate * Time.deltaTime;
        currentHealth = Mathf.Max(currentHealth, 0); // Prevent negative health

        // Sync the serialized value with the current health
        currentHealthSerialized = currentHealth;

        // Check for death condition
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Expose current health through a public property to display in the Inspector.
    public float CurrentHealth
    {
        get { return currentHealthSerialized; }
    }
}