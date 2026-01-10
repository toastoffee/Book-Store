using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager instance;
 
    public int money = 0;

    public TMP_Text moneyText;

    void Start()
    {
        instance = this;
        moneyText.text = $"金额:{money}";
    }

    void Update()
    {
        moneyText.text = $"金额:{money}";
    }
}
