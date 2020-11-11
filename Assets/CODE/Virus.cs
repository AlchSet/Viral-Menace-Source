using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Virus : MonoBehaviour
{

    public float t;
    public float life = 1;

    public bool hasHost;

    public GameObject copy;

    Collider hostCollider;

    public NPC host;

    bool multiplied = true;

    ParticleSystem deathParticles;
    ParticleSystem currentP;
    Collider[] myColliders;
    bool isdead;


    AudioSource sfx;
    // Start is called before the first frame update
    void Start()
    {
        deathParticles = transform.Find("VirusDeath").GetComponent<ParticleSystem>();
        currentP = GetComponent<ParticleSystem>();
        deathParticles.Stop();

        sfx = GetComponent<AudioSource>();

        myColliders = GetComponents<Collider>();

        if (transform.parent != null)
        {
            hasHost = true;
            host = transform.root.GetComponent<NPC>();

        }
        else
        {
            hasHost = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (multiplied)
        {
            t += Time.deltaTime;

            if (t > 3)
            {
                multiplied = false;
                t = 0;
            }
        }
        else
        {
            if (hasHost)
            {
                t += Time.deltaTime;

                if (t > 5)
                {
                    Instantiate(copy, transform.position, Quaternion.identity);
                    t = 0;
                    multiplied = true;
                }
            }
            else
            {
                t += Time.deltaTime;

                if (t > 10)
                {
                    Destroy(gameObject);

                }
            }
        }




    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("NPC") && !multiplied)
        {
            NPC n = other.transform.root.GetComponent<NPC>();



            if (!n.hasVirus)
            {
                if (hasHost)
                {
                    if (other != host.GetSelf())
                    {
                        GameObject g = Instantiate(copy, other.transform, true);
                        g.transform.localPosition = Vector3.zero;
                        n.Infect(this);
                        Debug.Log("INFECT NPC");
                        multiplied = true;
                    }

                }
                else
                {
                    GameObject g = Instantiate(copy, other.transform, true);
                    g.transform.localPosition = Vector3.zero;
                    Debug.Log("INFECT NPC");
                    n.Infect(this);
                    multiplied = true;
                }
            }




        }
    }


    private void OnCollisionEnter(Collision collision)
    {

        //Debug.Log(collision.collider.name);

        //if (collision.gameObject.layer == LayerMask.NameToLayer("Virus"))
        //{
        //Debug.Log("INFECT");
        //}
    }

    public void Damage(float i)
    {
        if(!isdead)
        {
            if(!hasHost)
            {
                i *= 2;
            }

            life -= i;
            if (life <= 0)
            {
                if (hasHost)
                {
                    host.Cure();
                }
                StartCoroutine(Disinfected());
            }

        }
      
    }

    IEnumerator Disinfected()
    {
        isdead = true;
        this.enabled = false;
        currentP.Stop();
        deathParticles.Emit(20);
        sfx.PlayOneShot(sfx.clip);
        foreach (Collider d in myColliders)
        {
            d.enabled = false;
        }
        transform.parent = null;
        yield return new WaitForSeconds(3);
        Destroy(gameObject);
        //yield return null;
    }



}
