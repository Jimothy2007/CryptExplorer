using UnityEngine;

public class SpikeScript : MonoBehaviour
{
    [SerializeField] private int damage = 1;

    public int GetDamage()
    {
        return damage;
    }
}
