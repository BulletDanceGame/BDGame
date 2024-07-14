using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class BossUI : MonoBehaviour
{
    private void Start()
    {
        Disable();
        StartCoroutine(SetValue());
    }

    private void OnDisable()
    {
        if(EventManager.Instance == null) return;
        EventManager.Instance.OnBossDamage -= TakeDamage;
        EventManager.Instance.OnBossHeal -= HealDamage;
    }


    // -- On, Off, and Setup -- //
    public void Enable()
    {
        GetComponent<HideUI>().enabled = true;
        GetComponent<CanvasGroup>().alpha = 1f;
    }

    public void Disable()
    {
        GetComponent<HideUI>().enabled = false;
        GetComponent<CanvasGroup>().alpha = 0f;
    }


    BossHealthController _bossHealth;
    IEnumerator SetValue()
    {
        while (UnitManager.Instance.GetBoss() == null)
            yield return null;

        _bossHealth = UnitManager.Instance.GetBoss().GetComponent<BossHealthController>();
        for(int i = 0; i < _bossHealth.phaseInfo.Count; i++)
        {
            //Have to Instantiate the last phase bar first
            //   Else they stack on each other and show the last phase bar first (unity UI stack logic)
            SetBossHealthBar(_bossHealth.phaseInfo.Count - 1 - i);
        }

        SetBossName(_bossHealth.BossName);
        EventManager.Instance.OnBossDamage += TakeDamage;
        EventManager.Instance.OnBossHeal += HealDamage;
    }


    public void TakeDamage(float damage)
    {
        _healthBar[_bossHealth.currentPhase].DecreaseValue(damage);
    }

    public void HealDamage(float damageToBeaHealed)
    {
        _healthBar[_bossHealth.currentPhase].DecreaseValue(-damageToBeaHealed);
    }

    public void ShowBarByPhase(int phase)
    {
        _healthBar[phase - 1].gameObject.SetActive(true);
    }


    [SerializeField]
    GameObject _UIBarPrefab, _barParent;
    List<UIBar> _healthBar = new List<UIBar>();
    void SetBossHealthBar(int phaseIndex)
    {
        var phaseInfo = _bossHealth.phaseInfo[phaseIndex];
        UIBar bar     = Instantiate(_UIBarPrefab, _barParent.transform).GetComponent<UIBar>();
        bar.SetUpUIBar(phaseInfo.phaseHealth);
        bar.GetComponentsInChildren<Image>()[1].color = phaseInfo.UIColor;
        bar.gameObject.SetActive(!phaseInfo.hideUI);
        _healthBar.Insert(0, bar);
    }

    void SetBossName(string name = "???")
    {
        GetComponentInChildren<TMPro.TextMeshProUGUI>().text = name;
    }
}