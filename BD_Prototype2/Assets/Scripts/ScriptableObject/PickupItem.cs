using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItem : MonoBehaviour
{
    
    public enum AbilityType { DASH, BAT }

    public AbilityType Type;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.parent.gameObject.GetComponent<Player>())
        {
            switch (Type)
            {
                case AbilityType.DASH:
                    collision.transform.parent.GetComponent<PlayerMovement>().DashActivated = true;
                    Debug.Log("Dash activated");
                    break;
                case AbilityType.BAT:
                    collision.transform.parent.GetComponentInChildren<PlayerSwing>().SwingActivated = true;
                    Debug.Log("Swing activated");
                    break;
            }

            Destroy(gameObject);
        }
    }

}
