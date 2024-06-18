using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }


    public int Currency;


    private void Start()
    {
        ResetCurrency();

        EventManager.Instance.OnAddCurrency += AddCurrency;
    }

    private void OnDisable()
    {
        EventManager.Instance.OnAddCurrency -= AddCurrency;
    }

    void AddCurrency(int score)
    {
        Currency += score;
    }
    
    void ResetCurrency()
    {
        Currency = 0;
    }
}
