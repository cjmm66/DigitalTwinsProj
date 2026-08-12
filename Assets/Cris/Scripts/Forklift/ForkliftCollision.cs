using UnityEngine;
using System.Collections;

public class ForkliftCollision : MonoBehaviour
{
    [SerializeField] private float recoveryTime = 30f;

    private ForkliftMovement movement;
    private bool isRecovering = false;

    private void Awake()
    {
        movement = GetComponent<ForkliftMovement>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ForkliftCollision otherForklift =
            collision.gameObject.GetComponent<ForkliftCollision>();

        if (otherForklift == null)
            return;

        // Prevent the same crash from being processed twice.
        if (isRecovering || otherForklift.isRecovering)
            return;

        Debug.Log("FORKLIFT CRASH!");

        StartCoroutine(RecoverFromCrash());
        otherForklift.StartCoroutine(otherForklift.RecoverFromCrash());
    }

    private IEnumerator RecoverFromCrash()
    {
        isRecovering = true;

        // Stop the forklift.
        movement.Stop();

        Debug.Log(gameObject.name + " is recovering for " + recoveryTime + " seconds.");

        yield return new WaitForSeconds(recoveryTime);

        movement.Resume();

        isRecovering = false;

        Debug.Log(gameObject.name + " has recovered and can move again.");
    }
}