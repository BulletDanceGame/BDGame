using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimAssistGameObject
{
    public GameObject target;
    public float minAngle, maxAngle;
    public bool AcceptableTarget = true;

    public AimAssistGameObject(GameObject enemyTarget, float enemyMinAngle = 0, float enemyMaxAngle = 0)
    {
        target = enemyTarget;
        minAngle = enemyMinAngle;
        maxAngle = enemyMaxAngle;
    } 
}

public class PlayerSwing : MonoBehaviour
{
    public PlayerInputActions _playerInput;
    public ControllerType CurrentController;

    public bool SwingActivated = false;
    public enum SwingState { NOTSWINGING, SWINGING, TIRED};
    public SwingState PlayerSwingState;
    private bool canRotate = true;

    Vector2 batDirection;
    public Vector2 bulletAngle { get; private set; }

    [Header("Aim Assist")]
    public float AimAssistRange = 45f;
    [SerializeField] bool AllowAimAssist = false;
    [SerializeField] private List<AimAssistGameObject> aimAssistCandidates;
    [SerializeField] private AimAssistGameObject aimAssistTarget;

    [Header ("Swing Hitbox")]
    public GameObject SwingHitbox;
    [SerializeField] float SwingHitboxUptime = 0.1f;
    [SerializeField] float SwingHitBoxAngle;

    [Header("Damage Modifiers")]
    [SerializeField] float PerfectDamageAddon = 5;
    [SerializeField] float GoodDamageAddon = 2;
    [SerializeField] float BadDamageAddon = 0;

    [Header("Swing Angles")]
    public bool BulletShouldGetDestroyedOnBad = false;
    public float BadHitAngle = 90;
    

    private Material _swingMat;

    [Header("Misc")]
    [SerializeField]  private SpriteRenderer _rend;

    public BeatTiming hitTiming { get; private set; }

    public GameObject rhythmUI;


    IEnumerator Start()
    {
        PlayerSwingState  = SwingState.NOTSWINGING;
        CurrentController = InputManager.Instance.CurrentController;
        _playerInput      = InputManager.Instance.PlayerInput;

        _swingMat = _rend.material;

        SwingActivated = SaveSystem.Instance.GetData().hasBat;

        aimAssistCandidates = new List<AimAssistGameObject>();

        yield return null;

        EventManager.Instance.OnKeyBoardAndMouseUsed += ChangeToKBM;
        EventManager.Instance.OnGamePadUsed += ChangeToGAMEPAD;

        EventManager.Instance.OnBossSpawned += EnableAimAssist;
        EventManager.Instance.OnBossDeath -= EnableAimAssist;
        EventManager.Instance.OnBossDeath += DisableAimAssist;

        EventManager.Instance.OnPlayerLastHit += LastHitSwing;
    }

    private void Update()
    {
        RotateBox();

        if (aimAssistTarget != null)
            aimAssistTarget = null;

        AimAssist();

        if (SwingActivated)
            rhythmUI.gameObject.SetActive(true);
        else
            rhythmUI.gameObject.SetActive(false);


    }

    void OnSwing()
    {
        if (!SwingActivated || GetComponentInParent<Player>().pauseActions)
            return;

        if(!canRotate) return;

        //SwingState
        if (PlayerSwingState == SwingState.TIRED) return;

        if (PlayerSwingState != SwingState.NOTSWINGING || GetComponentInParent<Player>().playerFailState == Player.PlayerFailState.FAILED)
        {
            print("TIRED, TIRED, TIRED");
            return;
        }
            

        PlayerSwingState = SwingState.SWINGING;

        //Hitbox
        SwingHitbox.SetActive(true);
        StartCoroutine(DisableSwingHitbox());


        //Timing
        hitTiming = PlayerRhythm.Instance.GetBeatTiming(ButtonInput.swing);

        switch (hitTiming)
        {
            case BeatTiming.PERFECT:
                AimAssist();
                bulletAngle = batDirection;
                _swingMat.SetColor("_SwingColor", new Color(0.3f, 0.7f, 0.8f, 0.9f));
                break;

            case BeatTiming.GOOD:
                AimAssist();
                bulletAngle = batDirection;
                _swingMat.SetColor("_SwingColor", new Color(0, 0.3f, 0.5f, 0.7f));
                break;

            case BeatTiming.BAD:
                bulletAngle = GetRandomAngle(-BadHitAngle, BadHitAngle);
                _swingMat.SetColor("_SwingColor", new Color(0.3f, 0, 0 , 0.75f));

                GetComponentInParent<Player>().Fail();
                EventManager.Instance.PlayerMiss();

                break;

        }
        
        EventManager.Instance.PlayerAttack(hitTiming, batDirection);
    }

    IEnumerator DisableSwingHitbox()
    {
        yield return new WaitForSeconds(SwingHitboxUptime);
        PlayerSwingState = SwingState.NOTSWINGING;
        SwingHitbox.SetActive(false);
        hitTiming = BeatTiming.NONE;
    }

    void ChangeToKBM()
    {
        CurrentController = ControllerType.KEYBOARDANDMOUSE;
    }

    void ChangeToGAMEPAD()
    {
        CurrentController = ControllerType.GAMEPAD;
    }

    void RotateBox()
    {
        if(!canRotate) return;

        if (CurrentController == ControllerType.KEYBOARDANDMOUSE)
        {
            // Get the mouse position in world space
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Camera.main.transform.position.z - transform.position.z;
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);

            // Calculate the direction to rotate towards
            batDirection = mousePos - transform.position;
        }
        else
        {
            Vector2 dir = _playerInput.Player.Aim.ReadValue<Vector2>();
            if (dir == new Vector2())
                return;
            dir = new Vector2(dir.x + transform.position.x, dir.y + transform.position.y);
            batDirection = (Vector3)dir - transform.position;
        }

        SwingHitBoxAngle = Mathf.Atan2(batDirection.y, batDirection.x) * Mathf.Rad2Deg - 90f;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, SwingHitBoxAngle),1);

        Quaternion rotation = transform.rotation;
        batDirection = rotation * Vector2.up;

    }

    void AimAssist()
    {
        if (!AllowAimAssist)
            return;


        SwingHitBoxAngle = (SwingHitBoxAngle + 360) % 360;

        CheckAimAssist();

        if (aimAssistTarget == null)
            return;

        if (aimAssistTarget.target != null)
        {
            Vector2 playerPos = transform.position;
            Vector2 targetPos = aimAssistTarget.target.transform.position;
            Vector2 direction = targetPos - playerPos;

            batDirection = direction;
        }
    }

    void CheckAimAssist()
    {
        aimAssistCandidates.Clear();

        foreach (GameObject enemy in RoomController._currentActiveEnemies)
        {
            aimAssistCandidates.Add(new AimAssistGameObject(enemy, 0,0));
        }

        if(UnitManager.Instance.GetBoss() != null)
            aimAssistCandidates.Add(new AimAssistGameObject(UnitManager.Instance.GetBoss(), 0, 0));

        foreach (AimAssistGameObject enemy in aimAssistCandidates)
        {
            if (enemy == null)
            {
                enemy.AcceptableTarget = false;
                continue;
            }

            if (enemy.target == null)
            {
                enemy.AcceptableTarget = false;
                continue;
            }

            

            RaycastHit2D hit = Physics2D.Raycast(transform.position, enemy.target.transform.position - transform.position,1000, LayerMask.GetMask("Enemy"));

            if (!hit)
            {
                enemy.AcceptableTarget = false;
                continue;
            }

            if (hit.collider.tag != "Enemy")
            {
                enemy.AcceptableTarget = false;
                continue;
            }

            enemy.minAngle = AngleToTarget(enemy.target.transform) - AimAssistRange;
            enemy.maxAngle = AngleToTarget(enemy.target.transform) + AimAssistRange;

            enemy.minAngle = (enemy.minAngle + 360) % 360;
            enemy.maxAngle = (enemy.maxAngle + 360) % 360;

            if (enemy.minAngle < enemy.maxAngle)
            {
                // If the minimum angle is less than the maximum angle,
                // the range spans from minAngle to maxAngle
                if (SwingHitBoxAngle >= enemy.minAngle && SwingHitBoxAngle <= enemy.maxAngle)
                {
                    enemy.AcceptableTarget = true;
                }
                else
                {
                    enemy.AcceptableTarget = false;
                }
            }
            else
            {
                // If the minimum angle is greater than the maximum angle,
                // the range spans from minAngle to 360 and 0 to maxAngle
                if (SwingHitBoxAngle >= enemy.minAngle || SwingHitBoxAngle <= enemy.maxAngle)
                {
                    enemy.AcceptableTarget = true;
                }
                else
                {
                    enemy.AcceptableTarget = false;
                }
            }

        }

        foreach(AimAssistGameObject enemy in aimAssistCandidates)
        {
            if (enemy.AcceptableTarget)
            {
                aimAssistTarget = enemy;
                break;
            }
        }

        if (aimAssistTarget == null)
        {
            return;
        }

        float angleToEnemy = 0;
        float angleToAimassistTarget = 0;

        foreach (AimAssistGameObject enemy in aimAssistCandidates)
        {
            if (!enemy.AcceptableTarget || enemy.target == aimAssistTarget.target)
                continue;

            angleToEnemy = ((AngleToTarget(enemy.target.transform) + 360) % 360) - (SwingHitBoxAngle + 360 % 360);
            angleToAimassistTarget = ((AngleToTarget(aimAssistTarget.target.transform) + 360) % 360) - (SwingHitBoxAngle + 360 % 360);

            if (angleToEnemy < 0)
                angleToEnemy *= -1;

            if(angleToAimassistTarget < 0)
                angleToAimassistTarget *= -1;

            if (angleToEnemy < angleToAimassistTarget)
                aimAssistTarget = enemy;

        }        
    }

    float AngleToTarget(Transform target)
    {
        Vector2 playerPos = transform.position;
        Vector2 targetPos = target.position;
        Vector2 direction = targetPos - playerPos;

        float angle = Mathf.Atan2(direction.y, direction.x);
        float degrees = angle * Mathf.Rad2Deg - 90f;

        return degrees;
    }

    Vector2 GetRandomAngle(float angle1, float angle2)
    {
        Vector2 targetPos = MouseCursor.GetMousePosition();
        return Quaternion.Euler(0f, 0f, Random.Range(angle1, angle2) + Vector2.SignedAngle(Vector2.right, targetPos - (Vector2)transform.position)) * Vector2.right;
    }

    void EnableAimAssist()
    {
        AllowAimAssist = true;
    }
    void DisableAimAssist()
    {
        AllowAimAssist = false;
    }


    public void EnableSwing()
    {
        SwingActivated = true;
    }


    // -- Last hit shit -- //
    void LastHitSwing(BeatTiming beatTiming)
    {
        if(!BossController.Instance.bossHealth.isLastPhase) return;
        if(beatTiming == BeatTiming.BAD) return;
        canRotate = false;
        Invoke("EnableRotate", 5.5f);
    }
    void EnableRotate()
    {
        canRotate = true;
    }
}
