using System.Collections;
using UnityEngine;


public class PlayerUI : MonoBehaviour
{
    private void Start()
    {
        Enable();
        if(EventManager.Instance == null) return;
        EventManager.Instance.OnPlayerDeath   += Disable;
    }

    private void OnDisable()
    {
        if(EventManager.Instance == null) return;
        EventManager.Instance.OnPlayerDeath   -= Disable;
        EventManager.Instance.OnPlayerDamage -= TakeDamage;
        EventManager.Instance.OnPlayerHeal   -= Heal;
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

        //All of these are dependent on if there is a player
        if(EventManager.Instance == null) yield break;
        EventManager.Instance.OnPlayerDamage += TakeDamage;
        EventManager.Instance.OnPlayerHeal   += Heal;
    }


    // -- Health -- //
    public void TakeDamage(float damage)
    {
        if (Player.currentHealth <= 0) return;
        _healthBar.DecreaseValue(damage);
    }

    public void Heal(float healAmount)
    {
        if (Player.currentHealth <= 0) return;
        if(healAmount <= 0f) healAmount = player.defaultHealAmount;
        _healthBar.IncreaseValue(healAmount);
    }
}