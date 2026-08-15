using UnityEngine;

public class ContainerData : MonoBehaviour
{
    [Header("Container Information")]
    [SerializeField]
    private string containerId;

    [SerializeField]
    private string clientId;

    [SerializeField]
    private string clientName;

    private WarehouseSlot currentSlot;

    public string ContainerId => containerId;
    public string ClientId => clientId;
    public string ClientName => clientName;

    public WarehouseSlot CurrentSlot => currentSlot;
    public bool IsStored => currentSlot != null;

    public void Initialize(
        string newContainerId,
        string newClientId,
        string newClientName)
    {
        containerId = newContainerId;
        clientId = newClientId;
        clientName = newClientName;

        gameObject.name = $"Container_{containerId}";
    }

    public void SetWarehouseSlot(WarehouseSlot slot)
    {
        currentSlot = slot;
    }

    public void ClearWarehouseSlot()
    {
        currentSlot = null;
    }
}