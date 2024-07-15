using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBarrel : MonoBehaviour
{
    public GameObject ExplosivePar;
    public float Damage;
    public float DeflectSpeed;
    public float ExplosionRadius;
    public float ExplosionForce;
    Collider2D[] Effected=new Collider2D[100];
    private bool _playerFound = false;
    private bool _deflected = false;
    private SpriteRenderer spriteR;
    private Rigidbody2D _rb;
    private Vector2 _hitdirection;

    PlayerSwing _playerSwing;


    private bool _explode=false;
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void Update()
    {
        Effected = Physics2D.OverlapCircleAll(transform.position, ExplosionRadius);
        if(_explode)
        {
            Explode();
        }
    }

    void Explode()
    {
        foreach (Collider2D GO in Effected)
        {
            Rigidbody GO_rigidbody=GO.GetComponent<Rigidbody>();
                Vector2 distance = GO.transform.position - transform.position;

                if (distance.magnitude > 0)
                {

                    //sometime it get the detection collider instead
                    //dunno how to fiz
                    GO.GetComponent<EnemyHealthController>()?.MinionTakeDamage(Damage);
                    GO.GetComponent<ExplosiveBarrel>()?.SetExplode();
                    GO.GetComponent<Barrel>()?.GetExploded();

                    if (GO.gameObject.tag == "Player")
                    {
                        //mokey solusion bc player has 2+ colliders
                        if (!_playerFound)
                        {
                            _playerFound = true;
                            EventManager.Instance.PlayerDamage(Damage);
                        }
                    }
                }
                
            if(Effected==null)
            {
                Destroy(this.gameObject);
            }
        }
    }
    public void SetExplode()
    {
        Invoke("ChainExplode", 0.2f);
    }

    void ChainExplode()
    {
        _explode = true;
        Instantiate(ExplosivePar, this.transform.position, Quaternion.identity);
        gameObject.GetComponent<SpriteRenderer>().enabled = false;

        Destroy(this.gameObject,0.01f);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag=="DeflectedBullet")
        {
            Bullet bullet = collision.GetComponent<Bullet>();

            Instantiate(ExplosivePar, this.transform.position, Quaternion.identity);
            _explode = true;
            bullet.Deactivate();

        }
        if (collision.gameObject.tag=="PlayerSwingBox")
        {
            _playerSwing=collision.GetComponentInParent<PlayerSwing>(); 
            BeatTiming hitTiming = _playerSwing.hitTiming;
            if(hitTiming!=BeatTiming.BAD)
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

                //Vector2 hitdirection = transform.position - UnitManager.Instance.GetPlayer().transform.position;
                _rb.velocity = _hitdirection * DeflectSpeed;
                _deflected = true;
            }
            else
            {
                Bullet bullet = collision.GetComponent<Bullet>();
                _explode = true;

                Instantiate(ExplosivePar, this.transform.position, Quaternion.identity);
                Destroy(this.gameObject,0.1f);

                bullet.Deactivate();

            }

        }
        if (_deflected)
        {           
            if (collision.gameObject.tag == "Enemy")
            {
                _explode = true;
                Instantiate(ExplosivePar, this.transform.position, Quaternion.identity);
                gameObject.GetComponent<SpriteRenderer>().enabled = false;
                Destroy(this.gameObject,2f);

            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(_deflected)
        {
            if (_deflected)
            {
                string[] targettags = new string[3] { "Wall", "ExplosiveBarrel", "Barrel" };
                for (int i = 0; i < targettags.Length; i++)
                {
                    if (collision.gameObject.tag == targettags[i])
                    {
                        Instantiate(ExplosivePar, this.transform.position, Quaternion.identity);
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
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, ExplosionRadius);
    }
}
