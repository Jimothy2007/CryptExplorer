using UnityEngine;

public class SpikeScript : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float knockbackForce = 5f;

    public int GetDamage()
    {
        return damage;
    }

    public float GetKnockbackForce()
    {
        return knockbackForce;
    }
}
