using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class WarehouseUI : MonoBehaviour
{
    [Header("Warehouse")]
    [SerializeField]
    private WarehouseManager warehouseManager;

    [Header("Text References")]
    [SerializeField]
    private TMP_Text capacityText;

    [SerializeField]
    private TMP_Text occupiedText;

    [SerializeField]
    private TMP_Text availableText;

    [SerializeField]
    private TMP_Text fullStatusText;

    [SerializeField]
    private TMP_Text clientListText;

    private void OnEnable()
    {
        if (warehouseManager != null)
        {
            warehouseManager.OnWarehouseChanged +=
                RefreshUI;
        }
    }

    private void Start()
    {
        RefreshUI();
    }

    private void OnDisable()
    {
        if (warehouseManager != null)
        {
            warehouseManager.OnWarehouseChanged -=
                RefreshUI;
        }
    }

    public void RefreshUI()
    {
        if (warehouseManager == null)
        {
            Debug.LogWarning(
                "Warehouse Manager is not assigned."
            );

            return;
        }

        if (capacityText != null)
        {
            capacityText.text =
                $"Capacity: " +
                $"{warehouseManager.TotalCapacity}";
        }

        if (occupiedText != null)
        {
            occupiedText.text =
                $"Occupied: " +
                $"{warehouseManager.OccupiedSpaces}";
        }

        if (availableText != null)
        {
            availableText.text =
                $"Available: " +
                $"{warehouseManager.AvailableSpaces}";
        }

        if (fullStatusText != null)
        {
            fullStatusText.text =
                warehouseManager.IsFull
                    ? "Status: FULL"
                    : "Status: AVAILABLE";
        }

        RefreshClientList();
    }

    private void RefreshClientList()
    {
        if (clientListText == null)
        {
            return;
        }

        Dictionary<string, int> clientCounts =
            warehouseManager
                .GetContainerCountByClient();

        StringBuilder builder =
            new StringBuilder();

        builder.AppendLine("Clients:");

        if (clientCounts.Count == 0)
        {
            builder.AppendLine(
                "No containers stored."
            );
        }
        else
        {
            foreach (
                KeyValuePair<string, int> client
                in clientCounts)
            {
                builder.AppendLine(
                    $"{client.Key}: {client.Value}"
                );
            }
        }

        clientListText.text = builder.ToString();
    }
}