using UnityEngine;

public class WaterGun : MonoBehaviour
{
    [Header("Water Projectile Settings")]
    public GameObject waterPrefab; // Water projectile prefab (assign in Inspector)
    public Transform hoseTip; // The point where water is fired from
    public float waterForce = 10f; // Base force of water shot
    public float arcForce = 1f; // Upward force to create a gentle arc
    public float waterLifetime = 2f; // How long the water exists before disappearing

    [Header("Recoil Settings")]
    public float recoilForce = 0.2f; // Lowered recoil for a gentle pushback
    public Rigidbody2D playerRb; // Player's Rigidbody2D (assign in Inspector)

    /// <summary>
    /// Shoots a water projectile with an arcing effect.
    /// </summary>
    public void ShootWater()
    {
        if (waterPrefab != null && hoseTip != null)
        {
            // Instantiate water projectile
            GameObject water = Instantiate(waterPrefab, hoseTip.position, Quaternion.identity);
            Rigidbody2D waterRb = water.GetComponent<Rigidbody2D>();

            if (waterRb != null)
            {
                // Get mouse position in world space
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                // Calculate shoot direction (normalized)
                Vector2 shootDirection = (mousePos - (Vector2)hoseTip.position).normalized;

                // Apply force to create an arc: forward + slight upward force
                Vector2 force = shootDirection * waterForce;
                force.y += arcForce; // Adds a small upward arc

                // Apply velocity to the water projectile
                waterRb.linearVelocity = force;

                // Apply gentle recoil to the player
                ApplyRecoil(shootDirection);

                // Destroy the water projectile after a set time
                Destroy(water, waterLifetime);
            }
        }
    }

    /// <summary>
    /// Applies a subtle recoil force to push the player in the opposite direction of the shot.
    /// </summary>
    void ApplyRecoil(Vector2 shootDirection)
    {
        if (playerRb != null)
        {
            // Apply a very gentle pushback using impulse
            playerRb.AddForce(-shootDirection * recoilForce, ForceMode2D.Impulse);
        }
    }
}
