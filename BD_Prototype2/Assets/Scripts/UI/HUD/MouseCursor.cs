using UnityEngine;

public class MouseCursor : MonoBehaviour
{
    public static MouseCursor instance;

    Vector2 lastMousePosition;
    bool mouseMoving = true;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {

        EventManager.Instance.OnGamePadUsed += DisableMouse;
        EventManager.Instance.OnKeyBoardAndMouseUsed += EnableMosue;

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = GetMousePosition();


        if (Input.mousePosition != (Vector3)lastMousePosition)
        {
            lastMousePosition = Input.mousePosition;
            mouseMoving = true;
        }
        else
            mouseMoving = false;
    }

    public bool IsMouseMoving()
    {
        return mouseMoving;
    }

    public static Vector3 GetMousePosition()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        return mousePos;
    }

    public void DisableMouse()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void EnableMosue()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
