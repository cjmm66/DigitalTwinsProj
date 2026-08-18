using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{

    private Camera mainCamera;


    private void Awake()
    {
        mainCamera = Camera.main;
    }



    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            SelectForklift();
        }


        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            MoveForklift();
        }
    }



    private void SelectForklift()
    {
        //Vector2 mousePosition =
        //    mainCamera.ScreenToWorldPoint(
        //        Mouse.current.position.ReadValue()
        //    );


        //Collider2D hit =
        //    Physics2D.OverlapPoint(mousePosition);



        //if (hit != null)
        //{
        //    ForkLiftSelection forklift =
        //        hit.GetComponent<ForkLiftSelection>();


        //    if (forklift != null)
        //    {
        //        forklift.Select();
        //    }
        //}

        Vector2 mousePosition =
        mainCamera.ScreenToWorldPoint(
            Mouse.current.position.ReadValue()
        );

        int forkliftLayer =
            LayerMask.GetMask("Default");

        Collider2D hit =
            Physics2D.OverlapPoint(
                mousePosition,
                forkliftLayer
            );

        if (hit != null)
        {
            ForkLiftSelection forklift =
                hit.GetComponent<ForkLiftSelection>();

            if (forklift != null)
            {
                forklift.Select();
            }
        }
    }




    private void MoveForklift()
    {
        if (ForkLiftSelection.selectedForklift == null)
        {
            Debug.Log("No forklift selected");
            return;
        }



        Vector2 destination =
            mainCamera.ScreenToWorldPoint(
                Mouse.current.position.ReadValue()
            );



        ForkliftMovement movement =
            ForkLiftSelection.selectedForklift
            .GetComponent<ForkliftMovement>();


        if (movement != null)
        {
            movement.MoveTo(destination);
        }
    }
}
