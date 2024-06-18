using UnityEngine;

public class BurnBarFX : MonoBehaviour
{
    [SerializeField] private GameObject _burnImageGO;

    void Start()
    {
        _burnImageGO.SetActive(false);
        if(EventManager.Instance == null) return;
        EventManager.Instance.OnPlayerBurn += BurnFX;
    }

    void OnDestroy()
    {
        if(EventManager.Instance == null) return;
        EventManager.Instance.OnPlayerBurn -= BurnFX;
    }

    void BurnFX(bool isBurning)
    {
        if(isBurning)
            _burnImageGO.SetActive(true);

        else
            _burnImageGO.SetActive(false);
    }
}
