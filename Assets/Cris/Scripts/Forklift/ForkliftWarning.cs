using UnityEngine;

public class ForkliftWarning : MonoBehaviour
{
    private ForkliftMovement myMovement;

    private void Awake()
    {
        myMovement = GetComponentInParent<ForkliftMovement>();

        if (myMovement == null)
        {
            Debug.LogError(
                gameObject.name +
                " could not find ForkliftMovement in its parent!"
            );
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        ForkliftMovement otherMovement =
            other.GetComponent<ForkliftMovement>();

        if (otherMovement == null)
        {
            return;
        }

        // Don't react to our own forklift
        if (otherMovement == myMovement)
        {
            return;
        }

        Debug.Log("You were about to crash, be carefull!");

        DeterminePriority(otherMovement);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        ForkliftMovement otherMovement =
            other.GetComponent<ForkliftMovement>();

        if (otherMovement == null)
        {
            return;
        }

        if (otherMovement == myMovement)
        {
            return;
        }

        ResumeIfNeeded(otherMovement);
    }

    private void DeterminePriority(ForkliftMovement otherMovement)
    {
        // Safety checks
        if (myMovement == null)
        {
            Debug.LogError(
                gameObject.name +
                " does not have access to its ForkliftMovement."
            );

            return;
        }

        if (otherMovement == null)
        {
            Debug.LogError(
                "The other forklift does not have a ForkliftMovement."
            );

            return;
        }

        bool myForkliftMoving = myMovement.IsMoving();
        bool otherForkliftMoving = otherMovement.IsMoving();

        // Both forklifts are stopped.
        if (!myForkliftMoving && !otherForkliftMoving)
        {
            return;
        }

        // My forklift is moving and the other one isn't.
        if (myForkliftMoving && !otherForkliftMoving)
        {
            otherMovement.Stop();

            Debug.Log(
                myMovement.gameObject.name +
                " has priority. Other forklift stopped."
            );

            return;
        }

        // Other forklift is moving and mine isn't.
        if (!myForkliftMoving && otherForkliftMoving)
        {
            myMovement.Stop();

            Debug.Log(
                otherMovement.gameObject.name +
                " has priority. This forklift stopped."
            );

            return;
        }

        // Both forklifts are moving.
        if (myForkliftMoving && otherForkliftMoving)
        {
            if (myMovement.GetMovementStartTime() <
                otherMovement.GetMovementStartTime())
            {
                otherMovement.Stop();

                Debug.Log(
                    myMovement.gameObject.name +
                    " has priority because it started moving first."
                );
            }
            else
            {
                myMovement.Stop();

                Debug.Log(
                    otherMovement.gameObject.name +
                    " has priority because it started moving first."
                );
            }
        }
    }

    private void ResumeIfNeeded(ForkliftMovement otherMovement)
    {
        if (myMovement == null || otherMovement == null)
        {
            return;
        }

        if (myMovement.IsStoppedByAvoidance())
        {
            myMovement.Resume();
        }

        if (otherMovement.IsStoppedByAvoidance())
        {
            otherMovement.Resume();
        }
    }
}