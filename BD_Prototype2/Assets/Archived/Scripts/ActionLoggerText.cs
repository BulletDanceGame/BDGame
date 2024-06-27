using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.LookDev;
using UnityEngine.UI;

public class ActionLoggerText : MonoBehaviour
{
    [SerializeField]
    TMPro.TextMeshProUGUI textGUI;

    [SerializeField]
    Slider slider;
    float maxTime = 0;
    float currentSliderTime = 0;

    int dashCount = 0, hitCount = 0, buntCount = 0, wallBounceCount = 0, rehitCount = 0, rehitWallBounceCount = 0, reHitWallBounceDamageCount = 0, DamageCount = 0;

    LinkedList<string> list = new LinkedList<string>();

    private void Start()
    {
        EventManager.Instance.OnDash += Dash;
        EventManager.Instance.OnHit += Hit;
        EventManager.Instance.OnWallBounce += WallBounce;
        EventManager.Instance.OnReHit += ReHit;
        EventManager.Instance.OnRehitWallBounce += ReHitWallBounce;
        EventManager.Instance.OnRehitWallBounceDamage += ReHitWallBounceDamage;
        EventManager.Instance.OnDamage += Damage;

        maxTime = ActionLogger.ScoreDelay;
        slider.maxValue = maxTime;
        slider.minValue = 0;
    }

    private void OnEnable()
    {
        if (currentSliderTime >= 0)
        {
            slider.value = currentSliderTime;
        }
        else
        {
            slider.value = 0;
        }
    }

    private void Update()
    {
        if (list.Count <= 0)
            return;

        textGUI.text = "";
        
        foreach (var t in list)
        {
            textGUI.text += t + "\n";
        }

        if(currentSliderTime >= 0)
        {
            currentSliderTime -= Time.deltaTime;
            slider.value = currentSliderTime;
        }
        
    }


    private Coroutine dashEnumer;

    void Dash()
    {
        if (dashEnumer != null)
        {
            StopCoroutine(dashEnumer);
            dashEnumer = null;
        }

        if(dashCount > 1)
            list.AddFirst("+ Dash x" + dashCount);
        else
            list.AddFirst("+ Dash");

        if (list.Count > 7)
            list.RemoveLast();

        dashEnumer = StartCoroutine(DashCounter());

        ResetCounter();
    }


    private Coroutine hitEnumer;

    void Hit()
    {
        if (hitEnumer != null)
        {
            StopCoroutine(hitEnumer);
            hitEnumer = null;
        }

        if (hitCount > 1)
            list.AddFirst("+ Hit x" + hitCount);
        else
            list.AddFirst("+ Hit");

        if (list.Count > 7)
            list.RemoveLast();

        hitEnumer = StartCoroutine(HitCounter());

        ResetCounter();
    }

    private Coroutine buntEnumer;

    void Bunt()
    {
        if(buntEnumer != null)
        {
            StopCoroutine(buntEnumer);
            buntEnumer = null;
        }

        if (buntCount >= 1)
            list.AddFirst("+ Bunt x" + buntCount);
        else
            list.AddFirst("+ Bunt");

        if (list.Count > 7)
            list.RemoveLast();

        buntEnumer = StartCoroutine(BuntCounter());

        ResetCounter();
    }

    private Coroutine wallBounceEnumer;

    void WallBounce()
    {
        if (wallBounceEnumer != null)
        {
            StopCoroutine(wallBounceEnumer);
            wallBounceEnumer = null;
        }

        if (wallBounceCount >= 1)
            list.AddFirst("+ Wall bounce x" + wallBounceCount);
        else
            list.AddFirst("+ Wall Bounce");

        if (list.Count > 7)
            list.RemoveLast();

        wallBounceEnumer = StartCoroutine(WallBounceCounter());

        ResetCounter();
    }

    private Coroutine reHitEnumer;

    void ReHit()
    {
        if (reHitEnumer != null)
        {
            StopCoroutine(reHitEnumer);
            reHitEnumer = null;
        }

        if (rehitCount >= 1)
            list.AddFirst("+ Re-hit x" + rehitCount);
        else
            list.AddFirst("+ Re-Hit");

        if (list.Count > 7)
            list.RemoveLast();

        reHitEnumer = StartCoroutine(ReHitCounter());

        ResetCounter();
    }
    private Coroutine reHitWallBounceEnumer;
    void ReHitWallBounce()
    {
        if (reHitWallBounceEnumer != null)
        {
            StopCoroutine(reHitWallBounceEnumer);
            reHitWallBounceEnumer = null;
        }

        if (rehitWallBounceCount >= 1)
            list.AddFirst("+ Re-Hit Wall bounce x" + rehitWallBounceCount);
        else
            list.AddFirst("+ Re-Hit Wall Bounce");

        if (list.Count > 7)
            list.RemoveLast();

        reHitWallBounceEnumer = StartCoroutine(ReHitWallBounceCounter());

        ResetCounter();
    }
    private Coroutine reHitWallBounceDamageEnumer;
    void ReHitWallBounceDamage()
    {

        if (reHitWallBounceDamageEnumer != null)
        {
            StopCoroutine(reHitWallBounceDamageEnumer);
            reHitWallBounceDamageEnumer = null;
        }

        if (reHitWallBounceDamageCount >= 1)
            list.AddFirst("+ Re-Hit Wall bounce Damage x" + reHitWallBounceDamageCount);
        else
            list.AddFirst("+ Re-Hit Wall Bounce Damage");

        if (list.Count > 7)
            list.RemoveLast();

        reHitWallBounceDamageEnumer = StartCoroutine(ReHitWallBounceDamageCounter());

        ResetCounter();
    }
    private Coroutine damageEnumer;
    void Damage()
    {
        if (damageEnumer != null)
        {
            StopCoroutine(damageEnumer);
            damageEnumer = null;
        }

        if (DamageCount >= 1)
            list.AddFirst("+ Damage enemy x" + DamageCount);
        else
            list.AddFirst("+ Damaged enemy");

        if (list.Count > 7)
            list.RemoveLast();

        damageEnumer = StartCoroutine(DamageCounter());

        ResetCounter();
    }

    IEnumerator DashCounter()
    {
        dashCount++;
        yield return new WaitForSeconds(0.1f);
        dashCount = 0;
    }

    IEnumerator HitCounter()
    {
        hitCount++;
        yield return new WaitForSeconds(0.1f);
        hitCount = 0;
    }

    IEnumerator BuntCounter()
    {
        buntCount++;
        yield return new WaitForSeconds(0.1f);
        buntCount = 0;
    }

    IEnumerator ReHitCounter()
    {
        rehitCount++;
        yield return new WaitForSeconds(0.1f);
        rehitCount = 0;
    }

    IEnumerator WallBounceCounter()
    {
        wallBounceCount++;
        yield return new WaitForSeconds(0.1f);
        wallBounceCount = 0;
    }

    IEnumerator ReHitWallBounceCounter()
    {
        rehitWallBounceCount++;
        yield return new WaitForSeconds(0.1f);
        rehitWallBounceCount = 0;
    }

    IEnumerator ReHitWallBounceDamageCounter()
    {
        reHitWallBounceDamageCount++;
        yield return new WaitForSeconds(0.1f);
        reHitWallBounceDamageCount = 0;
    }

    IEnumerator DamageCounter()
    {
        DamageCount++;
        yield return new WaitForSeconds(0.1f);
        DamageCount = 0;
    }

    private void ResetCounter()
    {
        currentSliderTime = maxTime;
        CancelInvoke();
        Invoke("Counter", maxTime);

    }

    void Counter()
    {
        dashCount = 0;
        hitCount = 0;
        buntCount = 0;
        wallBounceCount = 0;
        rehitCount = 0;
        rehitWallBounceCount = 0;
        reHitWallBounceDamageCount = 0;
        DamageCount = 0;

        list.Clear();

        slider.value = 0;

        textGUI.text = "";
    }
}
