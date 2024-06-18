using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : MonoBehaviour
{
    public GameObject DustPar;
    public float Damage;
    public float DeflectSpeed;
    private bool _deflected = false;
    private Rigidbody2D _rb;
    private Vector2 _hitdirection;
    PlayerSwing _playerSwing;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GetExploded()
    {
        Instantiate(DustPar, this.transform.position, Quaternion.identity);

        Destroy(this.gameObject);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "DeflectedBullet")
        {
            Bullet bullet = collision.GetComponent<Bullet>();
            bullet.Deactivate();
            Destroy(this.gameObject);
            Instantiate(DustPar, this.transform.position, Quaternion.identity);

        }
        if (collision.gameObject.tag == "PlayerSwingBox")
        {
            _playerSwing = collision.GetComponentInParent<PlayerSwing>();
            BeatTiming hitTiming = _playerSwing.hitTiming;
            if (hitTiming != BeatTiming.BAD)
            {
                if (_playerSwing.CurrentController == ControllerType.KEYBOARDANDMOUSE)
                {
                    // Get the mouse position in world space
                    Vector3 mousePos = Input.mousePosition;
                    mousePos.z = Camera.main.transform.position.z - transform.position.z;
                    mousePos = Camera.main.ScreenToWorldPoint(mousePos);

                    // Calculate the direction to rotate towards
                    _hitdirection = mousePos - transform.position;
                }
                else
                {
                    Vector2 dir = _playerSwing._playerInput.Player.Aim.ReadValue<Vector2>();
                    if (dir == new Vector2())
                        return;
                    dir = new Vector2(dir.x + transform.position.x, dir.y + transform.position.y);
                    _hitdirection = (Vector3)dir - transform.position;
                }

                _rb.velocity = _hitdirection * DeflectSpeed;
                _deflected = true;
            }
            else
            {
                Bullet bullet = collision.GetComponent<Bullet>();

                Instantiate(DustPar, this.transform.position, Quaternion.identity);
                Destroy(this.gameObject);

                bullet.Deactivate();

            }

        }
        if (_deflected)
        {
            if (collision.gameObject.tag == "Enemy")
            {
                collision.GetComponent<EnemyHealthController>()?.MinionTakeDamage(Damage);
                Instantiate(DustPar, this.transform.position, Quaternion.identity);
                Destroy(this.gameObject);

            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_deflected)
        {
            string[] targettags = new string[3] { "Wall", "ExplosiveBarrel", "Barrel" };
                for (int i = 0; i < targettags.Length; i++)
            {
                if (collision.gameObject.tag == targettags[i])
                {
                    Instantiate(DustPar, this.transform.position, Quaternion.identity);
                    collision.gameObject.GetComponent<ExplosiveBarrel>()?.SetExplode();
                    collision.gameObject.GetComponent<Barrel>()?.GetExploded();

                    gameObject.GetComponent<SpriteRenderer>().enabled = false;
                    Destroy(this.gameObject);
                    break;
                }
                   
            }
        }
    }

}
