using UnityEngine;

public class WarehouseSlot : MonoBehaviour
{
    [Header("Grid Position")]
    [SerializeField]
    private int row;

    [SerializeField]
    private int column;

    [Header("References")]
    [SerializeField]
    private Transform containerPosition;

    [SerializeField]
    private SpriteRenderer slotRenderer;

    [Header("Slot Colors")]
    [SerializeField]
    private Color emptyColor = Color.green;

    [SerializeField]
    private Color occupiedColor = Color.red;

    private ContainerData storedContainer;

    public int Row => row;
    public int Column => column;
    public bool IsOccupied => storedContainer != null;
    public ContainerData StoredContainer => storedContainer;

    public void Initialize(int newRow, int newColumn)
    {
        row = newRow;
        column = newColumn;

        gameObject.name =
            $"WarehouseSlot_{row}_{column}";

        UpdateVisual();
    }

    public bool StoreContainer(ContainerData container)
    {
        if (container == null)
        {
            Debug.LogWarning(
                "Cannot store a null container."
            );

            return false;
        }

        if (IsOccupied)
        {
            Debug.LogWarning(
                $"Slot {row}, {column} is already occupied."
            );

            return false;
        }

        storedContainer = container;

        Transform target =
            containerPosition != null
                ? containerPosition
                : transform;

        container.transform.SetParent(target);
        container.transform.localPosition = Vector3.zero;
        container.transform.localRotation = Quaternion.identity;

        container.SetWarehouseSlot(this);

        UpdateVisual();

        return true;
    }

    public ContainerData RemoveContainer()
    {
        if (!IsOccupied)
        {
            Debug.LogWarning(
                $"Slot {row}, {column} is already empty."
            );

            return null;
        }

        ContainerData removedContainer =
            storedContainer;

        storedContainer = null;

        removedContainer.transform.SetParent(null);
        removedContainer.ClearWarehouseSlot();

        UpdateVisual();

        return removedContainer;
    }

    private void UpdateVisual()
    {
        if (slotRenderer == null)
        {
            return;
        }

        slotRenderer.color =
            IsOccupied
                ? occupiedColor
                : emptyColor;
    }
}