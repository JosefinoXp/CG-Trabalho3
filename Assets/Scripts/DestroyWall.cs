using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyWall : MonoBehaviour
{
    [Header("SetWall")]
    [SerializeField]
    public GameObject wall;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Throwable")
        {
            Destroy(wall);
            Destroy(this.gameObject);
        }
    }
}
