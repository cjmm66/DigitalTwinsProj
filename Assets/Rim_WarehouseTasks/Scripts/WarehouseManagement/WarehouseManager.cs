using System;
using System.Collections.Generic;
using UnityEngine;

public class WarehouseManager : MonoBehaviour
{
    [Header("Grid Size")]
    [Min(1)]
    [SerializeField]
    private int rows = 4;

    [Min(1)]
    [SerializeField]
    private int columns = 5;

    [Header("Grid Spacing")]
    [SerializeField]
    private float horizontalSpacing = 1.8f;

    [SerializeField]
    private float verticalSpacing = 1.8f;

    [Header("Grid References")]
    [SerializeField]
    private WarehouseSlot slotPrefab;

    [SerializeField]
    private Transform slotsParent;

    private readonly List<WarehouseSlot> slots =
        new List<WarehouseSlot>();

    public event Action OnWarehouseChanged;

    public int TotalCapacity => slots.Count;

    public int OccupiedSpaces
    {
        get
        {
            int count = 0;

            foreach (WarehouseSlot slot in slots)
            {
                if (slot.IsOccupied)
                {
                    count++;
                }
            }

            return count;
        }
    }

    public int AvailableSpaces =>
        TotalCapacity - OccupiedSpaces;

    public bool IsFull =>
        TotalCapacity > 0 &&
        AvailableSpaces == 0;

    private void Start()
    {
        GenerateWarehouseGrid();
    }

    public void GenerateWarehouseGrid()
    {
        if (slotPrefab == null)
        {
            Debug.LogError(
                "Warehouse Slot Prefab is not assigned."
            );

            return;
        }

        ClearExistingGrid();

        for (int row = 0; row < rows; row++)
        {
            for (
                int column = 0;
                column < columns;
                column++)
            {
                Vector3 position =
                    transform.position +
                    new Vector3(
                        column * horizontalSpacing,
                        row * verticalSpacing,
                        0f
                    );

                WarehouseSlot newSlot =
                    Instantiate(
                        slotPrefab,
                        position,
                        Quaternion.identity,
                        slotsParent
                    );

                newSlot.Initialize(row, column);
                slots.Add(newSlot);
            }
        }

        NotifyWarehouseChanged();
    }

    private void ClearExistingGrid()
    {
        slots.Clear();

        if (slotsParent == null)
        {
            return;
        }

        for (
            int index = slotsParent.childCount - 1;
            index >= 0;
            index--)
        {
            Destroy(
                slotsParent.GetChild(index).gameObject
            );
        }
    }

    public WarehouseSlot FindAvailableSlot()
    {
        foreach (WarehouseSlot slot in slots)
        {
            if (!slot.IsOccupied)
            {
                return slot;
            }
        }

        return null;
    }

    public bool TryStoreContainer(
        ContainerData container)
    {
        if (container == null)
        {
            Debug.LogWarning(
                "Cannot store a null container."
            );

            return false;
        }

        if (container.IsStored)
        {
            Debug.LogWarning(
                $"{container.ContainerId} is already stored."
            );

            return false;
        }

        if (IsFull)
        {
            Debug.LogWarning(
                "The warehouse is full."
            );

            return false;
        }

        WarehouseSlot availableSlot =
            FindAvailableSlot();

        if (availableSlot == null)
        {
            Debug.LogWarning(
                "No available warehouse space was found."
            );

            return false;
        }

        bool stored =
            availableSlot.StoreContainer(container);

        if (stored)
        {
            NotifyWarehouseChanged();

            Debug.Log(
                $"{container.ContainerId} stored in " +
                $"slot {availableSlot.Row}, " +
                $"{availableSlot.Column}."
            );
        }

        return stored;
    }

    public bool RemoveContainer(
        ContainerData container)
    {
        if (container == null)
        {
            return false;
        }

        WarehouseSlot slot =
            container.CurrentSlot;

        if (slot == null)
        {
            Debug.LogWarning(
                $"{container.ContainerId} is not stored."
            );

            return false;
        }

        ContainerData removedContainer =
            slot.RemoveContainer();

        if (removedContainer == null)
        {
            return false;
        }

        NotifyWarehouseChanged();

        Debug.Log(
            $"{container.ContainerId} removed."
        );

        return true;
    }

    public Dictionary<string, int>
        GetContainerCountByClient()
    {
        Dictionary<string, int> clientCounts =
            new Dictionary<string, int>();

        foreach (WarehouseSlot slot in slots)
        {
            if (!slot.IsOccupied)
            {
                continue;
            }

            ContainerData container =
                slot.StoredContainer;

            string clientName =
                string.IsNullOrWhiteSpace(
                    container.ClientName)
                    ? "Unknown Client"
                    : container.ClientName;

            if (clientCounts.ContainsKey(clientName))
            {
                clientCounts[clientName]++;
            }
            else
            {
                clientCounts.Add(clientName, 1);
            }
        }

        return clientCounts;
    }

    private void NotifyWarehouseChanged()
    {
        OnWarehouseChanged?.Invoke();
    }
}