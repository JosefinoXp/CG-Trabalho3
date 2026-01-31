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

    [SerializeField]
    public AudioSource shotAudio;
    public AudioSource reloadAudio;

    [SerializeField]
    public Light shotLight;

    [SerializeField]
    public GameObject shotSprite;
    
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

        ShotAudio();
        StartCoroutine(ShotLight());

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

        ReloadAudio();

        AmmoToChar();
    }

    public void AmmoToChar()
    {
        string ammo_in_char = new string('ÿ', ammo_left);

        LCDModule.setRow1("Ammo: " + ammo_in_char);
    }

    public void ShotAudio()
    {
        shotAudio.Play();
    }

    public void ReloadAudio()
    {
        reloadAudio.Play();
    }

    public IEnumerator ShotLight()
    {
        shotLight.enabled = true;

        yield return ShotSprite();

        shotLight.enabled = false;
    }

    public IEnumerator ShotSprite()
    {
        shotSprite.SetActive(true);

        yield return new WaitForSeconds(0.3f);

        shotSprite.SetActive(false);
    }
}
