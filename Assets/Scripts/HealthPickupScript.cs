using UnityEngine;

public class HealthPickupScript : MonoBehaviour
{
    [SerializeField] private int healAmount = 1;

    public int GetHealAmount()
    {
        return healAmount;
    }
}
