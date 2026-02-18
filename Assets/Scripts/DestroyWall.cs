using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyWall : MonoBehaviour
{
    [Header("SetWall")]
    [SerializeField]
    public GameObject wall;

    public AudioSource clip;
    public BoxCollider collider;

    private void Awake()
    {
        collider = GetComponent<BoxCollider>();
        clip = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Throwable")
        {
            clip.Play();

            Destroy(wall);

            collider.enabled = false;

            other.gameObject.SetActive(false);
        }
    }
}
