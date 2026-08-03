using UnityEngine;
using UnityEngine.EventSystems;

public class ForkLiftSelection : MonoBehaviour
{
    public static ForkLiftSelection selectedForklift;


    public void Select()
    {
        selectedForklift = this;

        Debug.Log("Selected forklift: " + gameObject.name);
    }

}
