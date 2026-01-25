using LCDModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPlayer : MonoBehaviour
{
    [Header("Player LCD (UI)")]
    public LCDModule.LCDModule ui;

    [Header("Player LCD Str")]
    public string textHP;
    private string valueHP;

    //public string textAmmo;
    //private string valueAmmo;

    public void SetHealth(float HP)
    {
        HPToChar(HP);
    }

    public void HPToChar( float playerHealth )
    {
        int hp_left = ((int)playerHealth) / 10;

        string life_in_char = new string('ÿ', hp_left);

        ui.setRow1("Health: " + life_in_char);
    }
}
