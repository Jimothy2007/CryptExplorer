using Unity.XR.OpenVR;
using UnityEngine;

public class HealthScript : MonoBehaviour
{
    [SerializeField] private int currentHealth = 3;
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private float iFramesDuration = 1f;

    private float iFramesTimer = 0f;
    private bool hasIFrames => iFramesTimer > 0f;

    void Start()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (hasIFrames)
        {
            iFramesTimer -= Time.deltaTime;
        }
    }

    public void TakeDamage(int damage)
    {
        if (hasIFrames) return;

        currentHealth -= damage;
        TriggerIFrames();

        Debug.Log(gameObject.name + " took " + damage + " damage. Remaining health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Debug.Log(gameObject.name + " has been killed!");
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        Debug.Log(gameObject.name + " healed " + amount + " health. Current health: " + currentHealth);
    }

    public void TriggerIFrames()
    {
        iFramesTimer = iFramesDuration;
    }

    private void OnCollisionEnter(UnityEngine.Collision collision)
    {
        if (collision.gameObject.CompareTag("Spike"))
        {
            GameObject currentSpike = collision.gameObject;
            SpikeScript currentSpikeScript = currentSpike.GetComponent<SpikeScript>();

            TakeDamage(currentSpikeScript.GetDamage());

            Vector3 knockbackDirection = transform.position - currentSpike.transform.position;
            knockbackDirection.y = 0.25f;
            gameObject.GetComponent<PlayerMovement>().ApplyKnockback(knockbackDirection, currentSpikeScript.GetKnockbackForce());
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("HealthPickup"))
        {
            Heal(collision.GetComponent<HealthPickupScript>().GetHealAmount());
            Debug.Log(gameObject.name + " picked up health from " + collision.gameObject.name);
            Destroy(collision.gameObject);
        }
    }
}
