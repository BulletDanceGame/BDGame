using UnityEngine;
using UnityEngine.EventSystems;


public class ButtonHovered : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private CalibrationMenu cal;

    //Detect if the Cursor starts to pass over the GameObject
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        cal.canHit = false;
    }

    //Detect when Cursor leaves the GameObject
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        cal.canHit = true;
    }
}
