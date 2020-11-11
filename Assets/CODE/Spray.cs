using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spray : MonoBehaviour
{

    public ParticleSystem spray;
    ParticleSystem.EmissionModule sprayEM;

    [Range(0,100)]
    public float ammo = 100;

    bool fire;

    public Image gauge;
    Vector2 oGauge;

    AudioSource sfx;

    // Start is called before the first frame update
    void Start()
    {
        sprayEM = spray.emission;
        sprayEM.enabled = false;

        oGauge = gauge.rectTransform.sizeDelta;

        sfx = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if(fire)
        {
            ammo = Mathf.Clamp(ammo - Time.deltaTime * 6, 0, 100);

            Vector2 ns = oGauge;
            ns.x = oGauge.x * (ammo / 100);
            gauge.rectTransform.sizeDelta = ns;
        }

        if(ammo<=0)
        {
            DeactivateSpray();
        }
    }

    public void ActivateSpray()
    {
        if(ammo>0)
        {
            sprayEM.enabled = true;
            fire = true;
            sfx.Play();
        }
        
    }

    public void ReplenishAmmo(float a)
    {
        ammo = Mathf.Clamp(ammo +a, 0, 100);
        Vector2 ns = oGauge;
        ns.x = oGauge.x * (ammo / 100);
        gauge.rectTransform.sizeDelta = ns;
        

    }

    public void DeactivateSpray()
    {
        fire = false;
        sprayEM.enabled = false;
        sfx.Stop();
    }
}
