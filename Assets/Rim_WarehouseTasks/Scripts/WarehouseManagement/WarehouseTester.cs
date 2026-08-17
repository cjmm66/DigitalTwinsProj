using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WarehouseTester : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private WarehouseManager warehouseManager;

    [SerializeField]
    private ContainerData containerPrefab;

    [SerializeField]
    private Transform containerSpawnPoint;

    private readonly List<ContainerData> containers =
        new List<ContainerData>();

    private int containerNumber = 1;
    private int clientIndex = 0;

    private readonly string[] clients =
    {
        "Client A",
        "Client B",
        "Client C"
    };

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
        {
            return;
        }

        if (keyboard.aKey.wasPressedThisFrame)
        {
            CreateAndStoreContainer();
        }

        if (keyboard.rKey.wasPressedThisFrame)
        {
            RemoveLastContainer();
        }
    }

    public void CreateAndStoreContainer()
    {
        if (warehouseManager == null)
        {
            Debug.LogError(
                "Warehouse Manager is not assigned in WarehouseTester."
            );

            return;
        }

        if (containerPrefab == null)
        {
            Debug.LogError(
                "Container Prefab is not assigned in WarehouseTester."
            );

            return;
        }

        Vector3 spawnPosition =
            containerSpawnPoint != null
                ? containerSpawnPoint.position
                : transform.position;

        ContainerData newContainer = Instantiate(
            containerPrefab,
            spawnPosition,
            Quaternion.identity
        );

        string selectedClient = clients[clientIndex];

        string containerId =
            $"C-{containerNumber:000}";

        string clientId =
            $"CLIENT-{clientIndex + 1}";

        newContainer.Initialize(
            containerId,
            clientId,
            selectedClient
        );

        bool stored =
            warehouseManager.TryStoreContainer(
                newContainer
            );

        if (stored)
        {
            containers.Add(newContainer);

            Debug.Log(
                $"{containerId} belonging to " +
                $"{selectedClient} was created and stored."
            );

            containerNumber++;
            clientIndex++;

            if (clientIndex >= clients.Length)
            {
                clientIndex = 0;
            }
        }
        else
        {
            Destroy(newContainer.gameObject);
        }
    }

    public void RemoveLastContainer()
    {
        for (
            int index = containers.Count - 1;
            index >= 0;
            index--)
        {
            ContainerData container =
                containers[index];

            if (container == null)
            {
                containers.RemoveAt(index);
                continue;
            }

            bool removed =
                warehouseManager.RemoveContainer(
                    container
                );

            if (removed)
            {
                containers.RemoveAt(index);

                Debug.Log(
                    $"{container.ContainerId} was removed."
                );

                Destroy(container.gameObject);
            }

            return;
        }

        Debug.Log(
            "No stored container is available."
        );
    }
}