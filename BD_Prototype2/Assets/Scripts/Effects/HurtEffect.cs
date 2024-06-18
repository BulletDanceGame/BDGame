using System.Collections;
using UnityEngine;

public class HurtEffect : MonoBehaviour
{
    [SerializeField] private Material _flashMaterial;
    [SerializeField] private float _duration;
    public ParticleSystem _particleSystem;

    [SerializeField] bool isBoss = true;

    private SpriteRenderer _spriteRenderer;
    private Material _originalMaterial;
    
    private Coroutine _flashRoutine;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        if (_spriteRenderer == null) return;
            _originalMaterial = _spriteRenderer.material;
        

        if(isBoss)
            EventManager.Instance.OnBossDamage += Hurt;
    }

    void OnDestroy()
    {
        if(isBoss)
            EventManager.Instance.OnBossDamage -= Hurt;
    }


    public void Hurt(float none)
    {
        _particleSystem.Play();
    }


    public void HurtForPLayer() //for the sake of the playtest on 21/04/2023 this will be here so the game wont break
    {
        _particleSystem.Play();
        //Instantiate(_particleSystem, transform.position, Quaternion.identity);
        
        if (gameObject.activeSelf)
        {
            if (_flashRoutine != null)
            {
                StopCoroutine(_flashRoutine);
            }
            _flashRoutine = StartCoroutine(FlashRoutine());
        }
        
    }


    private IEnumerator FlashRoutine()
    {
        _spriteRenderer.material = _flashMaterial;
        yield return new WaitForSeconds(_duration);
        _spriteRenderer.material = _originalMaterial;
        _flashRoutine = null;
    }
}