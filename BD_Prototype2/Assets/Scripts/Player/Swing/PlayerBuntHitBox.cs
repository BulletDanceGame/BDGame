using UnityEngine;

public class PlayerBuntHitBox : MonoBehaviour
{
    Vector2 bulletAngle;

    public int bulletBackSpeed = 5;

    [SerializeField] private AK.Wwise.Event _buntSFX;

    private void Update()
    {
        bulletAngle = GetComponentInParent<PlayerSwing>().bulletAngle;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //GetComponent() slows things down, replaced with tag.
        //if (!collision.GetComponent<Bullet>())
        if(!collision.GetComponent<Bullet>()) //Bunt can hit DeflectedBullet so use Contains
            return;

        //GetComponent() slows things down, replaced with Bullet variable.
        Bullet bullet = collision.GetComponent<Bullet>();

        //Escape if unhittable
        if(!bullet.hittable) return;

        //Modify Bullet
        bullet.SetMoveDir(GetComponentInParent<PlayerSwing>().bulletAngle);
        bullet.SetSpeed(bulletBackSpeed);

        Debug.Log(bulletAngle);

        _buntSFX.Post(gameObject);
    }
}
