using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCritter : MonoBehaviour
{
    private BulletDance.Animation.UnitAnimationHandler _animHandler = null;


    private Vector3 _startPos;

    [SerializeField] private Transform _shootingPattern;
    private static List<BulletBag.BulletTypes> _bullets = new List<BulletBag.BulletTypes>();

    [SerializeField] private GameObject _critterPrefab;
    private GameObject _critter;

    void Start()
    {
        if (Random.Range(0f,10f) <= 5)
        {
            Invoke("AnimJump", Random.Range(0f, 5f));
        }

        if(_bullets.Count == 0)
        {
            _bullets.Add(BulletBag.BulletTypes.unhittable); //m
            _bullets.Add(BulletBag.BulletTypes.normal);
            _bullets.Add(BulletBag.BulletTypes.normal);

            _bullets.Add(BulletBag.BulletTypes.normal); //m
            _bullets.Add(BulletBag.BulletTypes.unhittable);
            _bullets.Add(BulletBag.BulletTypes.unhittable);

        }
    }
       

    void AnimJump()
    {
        _animHandler = GetComponentInChildren<BulletDance.Animation.UnitAnimationHandler>();
        _animHandler.WalkStart();
    }



    void OnTriggerEnter2D(Collider2D collision)
    {
        //Escape if tag is not Deflected
        if (!collision.gameObject.GetComponent<Bullet>())
            return;

        Bullet bullet = collision.GetComponent<Bullet>();

        if (bullet.type == BulletOwner.BOSSBULLET)
            return;


        bullet.Deactivate();

        Shooting.ShootInDirection(_shootingPattern, _bullets);


        _critter = Instantiate(_critterPrefab,null);
        _critter.transform.position = transform.position;
        _critter.SetActive(true);

        StartCoroutine(Return());


        _startPos = transform.position;
        transform.position += new Vector3(0, 300, 0);

    }




    IEnumerator Return()
    {
        while (transform.position != _startPos)
        {
            transform.position = Vector2.MoveTowards(transform.position, _startPos, 50 * Time.deltaTime);
            if (Vector2.Distance(transform.position, _startPos) < 5f)
            {
                transform.position = _startPos;
            }
            yield return null;
        }
    }
}
