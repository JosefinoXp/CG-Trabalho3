using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightTurn : MonoBehaviour
{
    public Light light;

    public void TurnLight()
    {
        light.enabled = !light.enabled;
    }
}
