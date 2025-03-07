using UnityEngine;
using UnityEngine.InputSystem;

public class UIinputManager : MonoBehaviour
{
    public bool isBacken;


    public void OnBack(InputValue context)
    {
        isBacken = context.isPressed;
    }
}
