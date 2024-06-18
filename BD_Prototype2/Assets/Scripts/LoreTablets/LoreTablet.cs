using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoreTablet : MonoBehaviour
{

    public TabletText tabletText;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.GetComponentInParent<Player>())
        {
            return;
        }

        collision.transform.parent.GetComponent<Player>().AbleToInteract = true;
        collision.GetComponentInParent<Player>().textToDisplay = tabletText;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.GetComponentInParent<Player>())
        {
            return;
        }

        collision.transform.parent.GetComponent<Player>().AbleToInteract = false;
        collision.GetComponentInParent<Player>().textToDisplay = null;
    }
}
