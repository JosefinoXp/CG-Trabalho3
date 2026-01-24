using LCDModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LCDTest : MonoBehaviour
{
    public LCDModule.LCDModule LCDModule;
    
    // Start is called before the first frame update
    void Start()
    {
        LCDModule.setRow1("Hello World!");
        LCDModule.setRow2("Ola Mundo!");
    }  
}
