using UnityEngine;

public class BulletExplode : MonoBehaviour
{
    public GameObject Bullet;

    private Color _currentcolor;
    void Start()
    {
        _currentcolor = Bullet.GetComponentInChildren<SpriteRenderer>().color;
    }

    // Update is called once per frame
    void OnAwake()
    {
        ParticleSystem.MainModule settings = GetComponent<ParticleSystem>().main;
        settings.startColor = new ParticleSystem.MinMaxGradient(_currentcolor);
    }
}
