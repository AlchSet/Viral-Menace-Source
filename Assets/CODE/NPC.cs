using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPC : MonoBehaviour
{


    public Transform[] routes;

    ParticleSystem deathFX;

    public SpriteRenderer biohazard;

    Transform model;

    public Virus virus;

    AudioSource sfx;


    GameManager manager;

    public Animator anim;


    struct RouteSelection
    {
        int lastChoice;
        int max;

        int[] selections;


        public void Initialize(int routeLength)
        {
            max = routeLength;
            selections = new int[routeLength - 1];
            lastChoice = Random.Range(0, routeLength);
            //Debug.Log("L=" + lastChoice);

        }


        public int PickRandom()
        {
            int v = 0;
            for (int i = 0; i < selections.Length; i++)
            {
                if (v != lastChoice)
                {
                    selections[i] = v;
                }
                v++;

            }

            v = selections[Random.Range(0, selections.Length)];
            lastChoice = v;

            return v;
        }

    }


    RouteSelection routeSelection;


    public Transform pointA;
    public Transform pointB;

    bool b;
    float t;

    NavMeshAgent agent;

    Collider self;

    public bool hasVirus;

    public float life = 1;


    public delegate void VirusEvent();

    public VirusEvent OnVirusStateChanged;


    public AudioClip explosionSound;
    public AudioClip CoughSound;

    bool isDead;


    float _timer;

    // Start is called before the first frame update
    void Awake()
    {

        manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        self = transform.Find("Model").GetComponent<Collider>();
        agent = transform.GetComponent<NavMeshAgent>();
        //agent.destination = pointA.position;

        routeSelection.Initialize(routes.Length);

        agent.destination = routes[routeSelection.PickRandom()].position;


        deathFX = transform.Find("Explosion").GetComponent<ParticleSystem>();

        //Debug.Log(routeSelection.PickRandom());
        model = transform.Find("Model");
        sfx = GetComponent<AudioSource>();

        StartCoroutine(CoughRoutine());
    }

    // Update is called once per frame
    void Update()
    {

        if(!isDead)
        {

            if (agent.velocity.sqrMagnitude > 0)
            {
                anim.SetBool("Walk", true);
            }
            else
            {
                anim.SetBool("Walk", false);
            }

            if (hasVirus)
            {
                life -= Time.deltaTime * 0.01f;

                if (life <= 0)
                {
                    Explode();

                    OnVirusStateChanged();
                }
            }

            _timer += Time.deltaTime;



            if(agent.remainingDistance>10&&_timer>20)
            {
                //Debug.Log("TESTTTTTTSA");
                _timer = 0;
                agent.destination = routes[routeSelection.PickRandom()].position;
            }


            if (agent.enabled&&agent.remainingDistance <= 0.65f)
            {
                //Debug.Log(gameObject.name + " is idleing");
                b = !b;

                t += Time.deltaTime;

                if (t > 5)
                {
                    //if(b)
                    //{
                    //agent.destination = pointB.position;

                    //Use it like this
                    agent.destination = routes[routeSelection.PickRandom()].position;
                    t = 0;
                    _timer = 0;
                    //}
                    //else

                    //{
                    //    t = 0;
                    //    //agent.destination = pointA.position;
                    //}
                }


            }





        }
        

    }



    public void Infect(Virus v)
    {
        if (!hasVirus)
        {

            virus = v;
            v.t = 0;
            v.hasHost = true;
            hasVirus = true;
            OnVirusStateChanged();


        }
    }


    public void Explode()
    {

        //gameObject.SetActive(false);
        StartCoroutine(FadeInOut());
        manager.PlayShake();
        this.enabled = false;
        OnVirusStateChanged();
        self.enabled = false;
        agent.isStopped = true;
        //agent.enabled = false;
        model.gameObject.SetActive(false);
        virus.hasHost = false;

        //virus.transform.parent = null;

        deathFX.Play();
        isDead = true;
        sfx.clip = explosionSound;
        sfx.Play();

    }

    public void Cure()
    {
        hasVirus = false;
        OnVirusStateChanged();
    }
    public Collider GetSelf()
    {
        return self;
    }

    IEnumerator FadeInOut()
    {

        Color a = Color.clear;
        Color b = Color.black;
        float i = 0;

        while (true)
        {
            i = Mathf.Clamp01(i + Time.deltaTime);
            biohazard.color = Color.Lerp(a, b, i);


            if (i >= 1)
            {
                break;
            }
            yield return new WaitForEndOfFrame();
        }

        while (true)
        {
            i = Mathf.Clamp01(i - Time.deltaTime);
            biohazard.color = Color.Lerp(a, b, i);


            if (i <= 0)
            {
                break;
            }
            yield return new WaitForEndOfFrame();
        }
    }



    IEnumerator CoughRoutine()
    {
        bool inCough = false;
        float coughChance = 0;
        float c = 0;
        float coughTimer = 0;
        while (true)
        {

            if (isDead)
            {
                break;
            }


            if (hasVirus)
            {
                if(!inCough)
                {
                    coughChance = life;

                    c = Random.Range(0f, 1f);

                    if(c>coughChance)
                    {
                        sfx.PlayOneShot(CoughSound);
                        anim.SetTrigger("Cough");
                        inCough = true;
                    }
                    else
                    {
                        inCough = true;
                    }

                }
                else
                {
                    coughTimer += Time.deltaTime;

                    if(coughTimer>=Mathf.Lerp(3,15,life))
                    {
                        inCough = false;
                        coughTimer = 0;
                    }
                }
                



            }
            else
            {
                inCough = false;
                coughTimer = 0;
            }



            yield return new WaitForEndOfFrame();
        }

        yield return null;
    }

    void PickRandomPoint()
    {

    }
}
