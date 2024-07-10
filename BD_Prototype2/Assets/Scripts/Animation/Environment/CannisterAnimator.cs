using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BulletDance.Animation
{


    public class CannisterAnimator : RhythmAnimator
    {
        public  enum CanType { DEFAULT = 0, EXPLOSION = 1, FIRE = 2, GROOVY = 3};
        private CanType _colorName = CanType.DEFAULT;

        [Space]

        [SerializeField]
        private SpriteRenderer _cannisterRdr, _yokaiRdr;
        private Material _material;

        [SerializeField]
        private UnityEngine.Rendering.Universal.Light2D _light;

        [SerializeField]
        private List<Color> _energyColors;

        [Space]
        [SerializeField] private ParticleSystem _vfxSmoke;
        [SerializeField] private ParticleSystem _vfxExplosion;

        [SerializeField] private LineRenderer _line;
        [SerializeField] private ParticleSystem _lineParticles;



        public void SetUp(CanType type)
        {
            _material = _cannisterRdr.material;
            _yokaiRdr.flipX = Random.Range(0, 2) == 0 ? true : false;


            _colorName = type;
            print("set " + _colorName.ToString());
            SetCanisterColor();
        }


        public void Deactivate()
        {
            RhythmAnimationManager.Instance?.RemoveFromController(this);

            _vfxExplosion.Play();
            _vfxExplosion.transform.parent = null;
        }


        public override void PlayAnimation(int anticipation, float duration)
        {
            _beatDuration = duration;
            _duration = GetDuration(_beatDuration, _animNoteDuration);

            //Anticipation = beats in the future
            //This default method will play animations in the current beat (anticipation = 0)
            if(anticipation != 0) return;
            if(_animator == null) return;

            _animator.speed = 1f / _duration;
            _animator.SetTrigger("Start");
        }

        private void Update()
        {
            if(UnitManager.Instance?.GetBoss() == null) return;

            _line.SetPosition(1, UnitManager.Instance.GetBoss().transform.position - transform.position - _line.transform.localPosition - (Vector3.up * 2.5f));

            Vector3 dist = _line.GetPosition(1) - _line.GetPosition(0);

            var sh = _lineParticles.shape;
            sh.scale = new Vector3(dist.magnitude, 1f, 1f);

            var em = _lineParticles.emission;
            em.rateOverTime = sh.scale.x * 10;
        
            _lineParticles.transform.localPosition = _line.GetPosition(0) + dist.normalized * (dist.magnitude / 2);

            float v = Mathf.Asin(_line.GetPosition(1).y/ dist.magnitude) * Mathf.Rad2Deg;
            if (dist.x < 0) { v *= -1; }
            _lineParticles.transform.eulerAngles = new Vector3(0,0,v);
        }



        private void SetCanisterColor()
        {
            int colorIndex = (int)_colorName;
            Color color    = _energyColors[colorIndex];


            _material.SetColor("_NewColor", color);
            _light.color = color;
            _line.startColor = color * 0.3f;
            _line.endColor   = color * 0.15f;
            var main = _lineParticles.main;
            main.startColor = new ParticleSystem.MinMaxGradient(Color.white, color);

            _animator.SetFloat("YokaiType", colorIndex);
        }

        public void StartSmokeVFX()
        {
            _vfxSmoke.Play();        
        }


    }


}
