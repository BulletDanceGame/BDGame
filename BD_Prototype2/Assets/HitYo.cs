using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitYo : MonoBehaviour
{
    public GameObject _gameObect, _gameObject2, _gameObject3;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _gameObect.GetComponent<PlayerSwing>().SwingActivated = true;
        _gameObject2.GetComponent<Turret>().Activate();
        _gameObject3.transform.gameObject.SetActive(true);

        Destroy(gameObject);
    }
}
