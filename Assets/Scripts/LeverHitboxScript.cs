using UnityEngine;

public class LeverHitboxScript : MonoBehaviour
{
    [SerializeField] private LeverScript leverScript;

    public LeverScript GetLeverScript()
    {
        return leverScript;
    }
}
