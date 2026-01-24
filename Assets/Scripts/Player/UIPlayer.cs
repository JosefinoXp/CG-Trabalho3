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
        ui.setRow1("Health: " + HP.ToString());
    }

    //TODO AMMO
    public void SetAmmo(int ammo)
    {
        ui.setRow2("Ammo: " + ammo.ToString());
    }
}
