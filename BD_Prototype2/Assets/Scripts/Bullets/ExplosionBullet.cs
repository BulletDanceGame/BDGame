using UnityEngine;

public class ExplosionBullet : MonoBehaviour
{
    [SerializeField]
    GameObject _bulletPF;

    Bullet _bullet;

    int _beats = 0;

    int numObjects = 36;

    [SerializeField] private bool _fireBullet;
    [SerializeField] private bool _groovyBullet;
    [SerializeField] private bool _fireGroovyBullet;
    [SerializeField] private bool _normalBullet;
    [SerializeField] private bool _unhittableBullet;

    [SerializeField] private float _beatExplode;

    private void OnEnable()
    {
        EventManager.Instance.OnBeat += Onbeat;
        _beats = 0;
    }

    private void OnDisable()
    {
        EventManager.Instance.OnBeat -= Onbeat;
    }

    private void Start()
    {
        _bullet = GetComponent<Bullet>();
    }

    private void Update()
    {
        if (_beats >= _beatExplode)
        {
            if(_normalBullet)
            {
                Explode();
            }
            if (_unhittableBullet)                  //king ape solution lol
            {
                UnhittableExplode();
            }
            if (_fireBullet)
            {
                FireExplode();
            }
            if (_groovyBullet)
            {
                GroovyExplode();
            }
            if (_fireGroovyBullet)
            {
                FireGroovyExplode();
            }
        }

    }

    void Onbeat(int _arbitrary)
    {
        Debug.Log("Beat" + _beats);
        _beats++;
    }

    void Explode()
    {
        Debug.Log("Explody");

        for (int i = 0; i < numObjects; i++)
            {
                float angle = i * (360f / numObjects);
                Vector3 position = Quaternion.Euler(0, 0, angle) * Vector2.up; //* Vector3.right * distance + transform.position;

                GameObject bullet = BulletBag.instance.FindBullet(BulletBag.BulletTypes.normal);
                if (bullet == null)
                {
                    continue;
                }

                bullet.transform.position = transform.position;
                bullet.transform.rotation = Quaternion.identity;

                bullet.GetComponent<Bullet>().SetMoveDir(position);

                bullet.SetActive(true);
                //Instantiate(_bulletPF, position, Quaternion.identity, transform);
            }

        _bullet.Deactivate();
    }

    void UnhittableExplode()
    {
        Debug.Log("Explody");

        for (int i = 0; i < numObjects; i++)
        {
            float angle = i * (360f / numObjects);
            Vector3 position = Quaternion.Euler(0, 0, angle) * Vector2.up; //* Vector3.right * distance + transform.position;

            GameObject bullet = BulletBag.instance.FindBullet(BulletBag.BulletTypes.unhittable);
            if (bullet == null)
            {
                continue;
            }

            bullet.transform.position = transform.position;
            bullet.transform.rotation = Quaternion.identity;

            bullet.GetComponent<Bullet>().SetMoveDir(position);

            bullet.SetActive(true);
            //Instantiate(_bulletPF, position, Quaternion.identity, transform);
        }

        _bullet.Deactivate();
    }

    void FireExplode()
    {
        Debug.Log("Explody");

        for (int i = 0; i < numObjects; i++)
        {
            float angle = i * (360f / numObjects);
            Vector3 position = Quaternion.Euler(0, 0, angle) * Vector2.up; //* Vector3.right * distance + transform.position;

            GameObject bullet = BulletBag.instance.FindBullet(BulletBag.BulletTypes.fire);
            if (bullet == null)
            {
                continue;
            }

            bullet.transform.position = transform.position;
            bullet.transform.rotation = Quaternion.identity;

            bullet.GetComponent<Bullet>().SetMoveDir(position);

            bullet.SetActive(true);
            //Instantiate(_bulletPF, position, Quaternion.identity, transform);
        }

        _bullet.Deactivate();
    }

    void GroovyExplode()
    {
        Debug.Log("Explody");

        for (int i = 0; i < numObjects; i++)
        {
            float angle = i * (360f / numObjects);
            Vector3 position = Quaternion.Euler(0, 0, angle) * Vector2.up; //* Vector3.right * distance + transform.position;

            GameObject bullet = BulletBag.instance.FindBullet(BulletBag.BulletTypes.groovy);
            if (bullet == null)
            {
                continue;
            }

            bullet.transform.position = transform.position;
            bullet.transform.rotation = Quaternion.identity;

            bullet.GetComponent<Bullet>().SetMoveDir(position);

            bullet.SetActive(true);
            //Instantiate(_bulletPF, position, Quaternion.identity, transform);
        }

        _bullet.Deactivate();
    }

    void FireGroovyExplode()
    {
        Debug.Log("Explody");

        for (int i = 0; i < numObjects; i++)
        {
            float angle = i * (360f / numObjects);
            Vector3 position = Quaternion.Euler(0, 0, angle) * Vector2.up; //* Vector3.right * distance + transform.position;

            GameObject bullet = BulletBag.instance.FindBullet(BulletBag.BulletTypes.firegroovy);
            if (bullet == null)
            {
                continue;
            }

            bullet.transform.position = transform.position;
            bullet.transform.rotation = Quaternion.identity;

            bullet.GetComponent<Bullet>().SetMoveDir(position);

            bullet.SetActive(true);
            //Instantiate(_bulletPF, position, Quaternion.identity, transform);
        }

        _bullet.Deactivate();
    }
}
