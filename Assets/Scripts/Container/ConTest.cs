using UnityEngine;

public class ContainerTest : MonoBehaviour
{
    [SerializeField] private ContainerStorage container;

    private void Start()
    {
        container.StartStorageTimer();
    }
}