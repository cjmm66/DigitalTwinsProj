using System;
using UnityEngine;

[Serializable]
public class WarehouseClient
{
    [Header("Client Information")]
    public string clientName = "Client A";

    [Header("Client Expectations")]
    [Min(1f)]
    public float expectedMarketPrice = 1000f;

    [Header("Future Willingness Variables")]
    [Range(-20f, 20f)]
    public float reputationModifier = 0f;

    [Range(-20f, 20f)]
    public float containerPriorityModifier = 0f;

    [Range(-20f, 20f)]
    public float occupancyModifier = 0f;

    [Range(-20f, 20f)]
    public float relationshipModifier = 0f;
}