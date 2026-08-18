//using UnityEngine;

//public class ForkliftPickupZone : MonoBehaviour
//{
//    private ForkliftMovement forklift;

//    private void Awake()
//    {
//        forklift =
//            GetComponentInParent<ForkliftMovement>();
//    }

//    private void OnTriggerEnter2D(Collider2D other)
//    {
//        ContainerStorage container =
//            other.GetComponent<ContainerStorage>();

//        if (container == null)
//            return;

//        // Don't even attempt pickup if the forklift
//        // is already carrying something.
//        if (!forklift.IsEmpty())
//            return;

//        // Try to pick up the container.
//        forklift.TryPickupContainer(container);
//    }
//}

using UnityEngine;

public class ForkliftPickupZone : MonoBehaviour
{
    private ForkliftMovement forklift;

    private void Awake()
    {
        forklift =
            GetComponentInParent<ForkliftMovement>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(
            "PickupZone detected: " +
            other.gameObject.name
        );

        ContainerStorage container =
            other.GetComponent<ContainerStorage>();

        if (container == null)
        {
            Debug.Log(
                "Object detected, but it is NOT a container."
            );

            return;
        }

        Debug.Log(
            "Container detected: " +
            container.gameObject.name
        );

        if (!forklift.IsEmpty())
        {
            Debug.Log(
                "Forklift is already carrying a container."
            );

            return;
        }

        forklift.TryPickupContainer(container);
    }
}