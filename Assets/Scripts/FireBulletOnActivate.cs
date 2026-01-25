using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class FireBulletOnActivate : MonoBehaviour
{
    public GameObject bullet;
    public Transform spawnPoint;

    public LCDModule.LCDModule LCDModule;

    [SerializeField]
    public float fireSpeed;
    public int ammo = 10;

    private int ammo_left;
    
    // Start is called before the first frame update
    void Start()
    {
        XRGrabInteractable grabbable = GetComponent<XRGrabInteractable>();
        grabbable.activated.AddListener(FireBullet);

        ammo_left = ammo;
        AmmoToChar();
    }

    public void FireBullet(ActivateEventArgs arg)
    {
        if (ammo_left <= 0) return;

        //bala gasta
        ammo_left--;

        AmmoToChar();

        GameObject spawnedBullet = Instantiate(bullet);
        spawnedBullet.transform.position = spawnPoint.position;
        spawnedBullet.GetComponent<Rigidbody>().velocity = spawnPoint.forward * fireSpeed;
        Destroy(spawnedBullet, 5);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Mag")
        {
            RealoadWeapon();

            Destroy(collision.gameObject);
        }
    }

    public void RealoadWeapon()
    {
        ammo_left = ammo;

        AmmoToChar();
    }

    public void AmmoToChar()
    {
        string ammo_in_char = new string('ÿ', ammo_left);

        LCDModule.setRow1("Ammo: " + ammo_in_char);
    }
}
