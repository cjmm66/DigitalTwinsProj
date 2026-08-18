using UnityEngine;

public class ShipmentZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        ForkliftMovement forklift =
            other.GetComponent<ForkliftMovement>();

        if (forklift == null)
            return;

        Debug.Log(
            forklift.gameObject.name +
            " entered the shipment zone."
        );

        if (forklift.IsEmpty())
        {
            Debug.Log(
                forklift.gameObject.name +
                " has no container to ship."
            );

            return;
        }

        forklift.CompleteContainerShipment();
    }
}