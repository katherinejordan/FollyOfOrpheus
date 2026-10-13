using UnityEngine;

public class FalsePlayer : MonoBehaviour, IBoostable
{
    public void Boost(float boostAmount, AccuracyLevel level)
    {
        Debug.Log($"Boosting by: {boostAmount}");
    }
}