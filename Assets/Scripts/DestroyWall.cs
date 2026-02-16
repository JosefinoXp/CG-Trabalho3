using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyWall : MonoBehaviour
{
    [Header("SetWall")]
    [SerializeField]
    public GameObject wall;

    public AudioSource clip;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Throwable")
        {
            clip.Play();

            Destroy(wall);
        }
    }
}
