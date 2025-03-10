using UnityEngine;
using UnityEngine.InputSystem;

public class UIinputManager : MonoBehaviour
{
    public bool isBacken;

    public bool isNexten;
    
    public bool isPreviousen;


    public void OnBack(InputValue context)
    {
        isBacken = context.isPressed;
    }

    public void OnNext(InputValue context)
    {
        isNexten = context.isPressed;
    }

    public void OnPrevious(InputValue context)
    {
        isPreviousen = context.isPressed;
    }
}
