using System;
using System.Collections;
using TMPro;
using UnityEngine;


public class PlayerUI : MonoBehaviour
{
    private void OnEnable()
    {
        if(EventManager.Instance == null) return;
        EventManager.Instance.OnPlayerSpawned    += Enable;
        EventManager.Instance.OnPlayerDeath      += Disable;
        EventManager.Instance.OnPlayerDamage     += TakeDamage;
        EventManager.Instance.OnPlayerHeal       += Heal;
        EventManager.Instance.OnCalibrationAlert += Cali;
    }

    private void OnDisable()
    {
        if(EventManager.Instance == null) return;
        EventManager.Instance.OnPlayerSpawned    -= Enable;
        EventManager.Instance.OnPlayerDeath      -= Disable;
        EventManager.Instance.OnPlayerDamage     -= TakeDamage;
        EventManager.Instance.OnPlayerHeal       -= Heal;
        EventManager.Instance.OnCalibrationAlert -= Cali;
    }


    // -- On, Off, and Setup -- //
    [SerializeField]
    GameObject _UIGO;

    void Enable()
    {
        _UIGO.SetActive(true);
        StartCoroutine(SetValue());
    }

    void Disable()
    {
        _UIGO.SetActive(false);
    }


    [SerializeField]
    UIBar _healthBar;
    Player player;
    IEnumerator SetValue()
    {
        while (UnitManager.Instance.GetPlayer() == null)
            yield return null;

        player = UnitManager.Instance.GetPlayer().GetComponent<Player>();

        _healthBar.SetUpUIBar(player.startingHealth);
    }


    // -- Health -- //
    public void TakeDamage(float damage)
    {
        if(player.isDead) return;
        _healthBar.DecreaseValue(damage);
    }

    public void Heal(float healAmount)
    {
        if(player.isDead) return;
        if(healAmount <= 0f) healAmount = player.defaultHealAmount;
        _healthBar.IncreaseValue(healAmount);
    }


    public GameObject caliAlert;
    public TextMeshProUGUI caliAlertText;
    public void Cali(string text)
    {
        StartCoroutine(CalibrationAlert(text));
    }

    IEnumerator CalibrationAlert(string text)
    {
        caliAlert.SetActive(true);
        caliAlertText.text = text;

        yield return new WaitForSeconds(10);

        caliAlert.SetActive(false);
    }

}