using UnityEngine;
using UnityEngine.UI;

public class SlowMotionBar : MonoBehaviour
{
    
    public Image SlowMotionBarImage;
    public static SlowMotionBar Instance { get; private set; }


    public float CurrentTime;
    public float CoolDownTime;
    public bool CanSlowMo;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        EventManager.Instance.OnBeat += AddCooldown;
    }

    void Update()
    {
        if(CurrentTime==CoolDownTime)
        {
            CanSlowMo = true;
        }
        if(CurrentTime<CoolDownTime)
        {
            CanSlowMo=false;
        }
    }

    void AddCooldown(int beat)
    {
        if (beat % 2 != 0)
            return;

        CurrentTime += 1;

        if (CurrentTime > CoolDownTime)
            CurrentTime = CoolDownTime;

        UpdateStaminaBar();
    }

    void UpdateStaminaBar()
    {
        SlowMotionBarImage.fillAmount = Mathf.Clamp(CurrentTime / CoolDownTime, 0, 1f);
    }
}
