using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wait : MonoBehaviour
{
    private short timer;

    public void Timer(short s)
    {
        timer = s;
        StartCoroutine(Test(timer));
    }

    IEnumerator Test(short Timer)
    {
        yield return new WaitForSeconds(Timer);
        Debug.Log("Espera Feita");
    }
}
