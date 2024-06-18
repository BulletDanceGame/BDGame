using System.Collections.Generic;
using UnityEngine;
using BulletDance.Animation;


public class BossGraphicsTest : MonoBehaviour
{
    LayeredSpriteAnimator anim;

    [SerializeField]
    private SpriteRenderer _torsoRenderer;
    private Material _mat;

    [SerializeField]
    private UnityEngine.Animator _torsoAnimator;


    [SerializeField]
    private List<BulletDance.Graphics.ColorName> _energyColors;
    int _colorIndex = 0;

    [SerializeField]
    private Gradient _colorOverTime;
    [SerializeField]
    private float _colorChangeSpeed;
    private float _colorTime = 0f;

    float _curAnimState = 0;
    float _prevAnimState = 0;


    int _phaseNum = 1;
    int maxphaseNum = 3;

    // Start is called before the first frame update
    void Start()
    {
        // -- Getting references -- //
        anim = GetComponentInChildren<LayeredSpriteAnimator>();        

        _mat = _torsoRenderer.material;

        _prevAnimState = _curAnimState;

        //EventManager.Instance.OnBossPhaseChangeFinished += OnPhaseChangeFinished;
    }


    void OnPhaseChange(int phaseNum)
    {
        anim.SetDirection(Direction.Front);
        anim.Anim(5);
    }

    void OnPhaseChangeFinished(int phaseNum)
    {
        _phaseNum=phaseNum;
        if(_phaseNum > maxphaseNum)
            _phaseNum = 1;

        anim.SetLibraryByPhase(phaseNum);
    }


    // Update is called once per frame
    void Update()
    {
        //Death
        if(Input.GetKeyDown(KeyCode.U) && _phaseNum == 3)
        {
            anim.SetDirection(Direction.Front);
            anim.Anim(6);
        }

        //Phases
        if(Input.GetKeyDown(KeyCode.P))
        {
            anim.SetDirection(Direction.Front);
            anim.Anim(5);
            _mat.SetColor("_NewColor", Color.white);
        }

        if(_phaseNum == 3)
        {
            _colorTime += Time.deltaTime * _colorChangeSpeed;
            if(_colorTime >= 1f)
                _colorTime = 0f;
            
            _mat.SetColor("_OldColor", _colorOverTime.Evaluate(_colorTime));
        }

        if(Input.GetKeyDown(KeyCode.S))
        {
            anim.Anim(4);

            _mat.SetColor("_OldColor", _energyColors[_colorIndex].color);

            _colorIndex++;
            if(_colorIndex > _energyColors.Count - 1)
                _colorIndex = 0;

            _mat.SetColor("_NewColor", _energyColors[_colorIndex].color);
        }

        _curAnimState = (float)_torsoAnimator.GetInteger("State");

        if(_curAnimState != 4 && _prevAnimState == 4)
        {
            _mat.SetColor("_OldColor", _energyColors[_colorIndex].color);
        }

        _prevAnimState = _curAnimState;


        //Directions
        if(Input.GetKeyDown(KeyCode.UpArrow))
            anim.SetDirection(Direction.Back);

        if(Input.GetKeyDown(KeyCode.DownArrow))
            anim.SetDirection(Direction.Front);

        if(Input.GetKeyDown(KeyCode.RightArrow))
            anim.SetDirection(Direction.Right);

        if(Input.GetKeyDown(KeyCode.LeftArrow))
            anim.SetDirection(Direction.Left);


        //Anims
        if(Input.GetKeyDown(KeyCode.Escape))
            anim.Anim(AnimAction.Idle);

        if(Input.GetKeyDown(KeyCode.A))
            anim.Anim(AnimAction.Attack);

        if(Input.GetKeyDown(KeyCode.D))
            anim.Anim(AnimAction.Dash);

        if(Input.GetKeyDown(KeyCode.W))
            anim.Anim(AnimAction.Walk);

        if(Input.GetKeyDown(KeyCode.E))
            anim.AnimHurt();

        if(Input.GetKeyDown(KeyCode.Q))
            anim.Anim(5);


        if(Input.GetKeyDown(KeyCode.Keypad1))
            anim.AnimWalk("Legs");

        if(Input.GetKeyDown(KeyCode.Keypad2))
            anim.AnimDash("Legs");

        if(Input.GetKeyDown(KeyCode.Keypad3))
            anim.AnimIdle("Legs");

        if(Input.GetKeyDown(KeyCode.Keypad7))
            anim.AnimAttack("Torso");

        if(Input.GetKeyDown(KeyCode.Keypad9))
            anim.AnimIdle("Torso");
    }
}
