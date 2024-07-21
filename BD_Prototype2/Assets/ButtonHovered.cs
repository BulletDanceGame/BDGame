using UnityEngine;
using UnityEngine.EventSystems;


public class ButtonHovered : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private CalibrationMenu cal;

    bool on;

    //Detect if the Cursor starts to pass over the GameObject
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        on = true;
        cal.canHit = false;
    }

    //Detect when Cursor leaves the GameObject
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        cal.canHit = true;
    }

    private void OnDisable()
    {
        if (on)
        {
            cal.canHit = true;
        }
    }
}
