using UnityEngine;

public class WaterProjectile : MonoBehaviour
{
    public float speed = 10f;  // Speed of the water projectile
    public float lifetime = 2f;  // Time before the water disappears
    public Vector2 gravityMultiplier = new Vector2(1f, 0.5f);  // Gravity effect on water

    private Rigidbody2D rb;  // Rigidbody2D component
    private float timeAlive;  // Tracks how long the water has existed

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();  // Get Rigidbody2D component

        if (rb != null)
        {
            rb.gravityScale = gravityMultiplier.y;  // Set gravity scale
        }

        timeAlive = 0f;  // Reset timer
    }

    void Update()
    {
        timeAlive += Time.deltaTime;

        // Destroy projectile after lifetime expires
        if (timeAlive >= lifetime)
        {
            Destroy(gameObject);
        }
    }}       

   