using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopDown3DController : MonoBehaviour
{
    public Vector3 input;
    Rigidbody body;
    public float speed=5;

    public Transform model;

    Vector3 pointing;

    public Spray spray;

    public GameObject Visor;

    bool visor;

    bool restock;

    float stockTime = 0;

    float st = 0;

    public Image visorgauge;
    Vector2 oGauge;
    float visorENERGY = 1;

    Color ocolor;
    Color tcolor;

    float _visorPriceT;
    bool _visorPrice;


    public int mode = 1;


    public GameObject HBarrier;
    public GameObject Barrier;

    public static int bcount=6;


    public Text barrierText;

    Collider[] rBox=new Collider[1];

    public Animator anim;

    AudioSource sfx;

    public AudioClip ammoClip;
    public AudioClip tapeClip;
    public AudioClip tape2Clip;
    public AudioClip restClip;
    public AudioClip visorClip;
    public AudioClip switchClip;

    ParticleSystem restP;
    ParticleSystem.EmissionModule restEm;

    public Image slot1;
    public Image slot2;

    public Color SelectedSlot;
    public Color UnSelectedSlot;

    // Start is called before the first frame update
    void Start()
    {
        bcount = 6;
        oGauge = visorgauge.rectTransform.sizeDelta;
        body = GetComponent<Rigidbody>();
        sfx= GetComponent<AudioSource>();
        ocolor = visorgauge.color;
        tcolor = ocolor;
        tcolor.a = 0.5f;

        restP = transform.Find("ReloadFX").GetComponent<ParticleSystem>();
        restEm = restP.emission;

        restEm.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        //Vector2 mpos = Camera.main.ScreenToWorldPoint(Input.mousePosition);


        if (input.sqrMagnitude > 0)
        {
            anim.SetBool("Walk", true);
        }
        else
        {
            anim.SetBool("Walk",false);
        }


        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        Debug.DrawRay(mouseRay.origin, mouseRay.direction * 100);


        if(Physics.Raycast(mouseRay,out RaycastHit hit, 100, 1 << LayerMask.NameToLayer("Ground")))
        {
            pointing = hit.point;
        }

        Vector3 dir = pointing - transform.position;
        dir.y = 0;
        float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        model.rotation = Quaternion.Euler(0, angle, 0);



        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            HBarrier.SetActive(false);
            mode = 1;
            slot1.color = SelectedSlot;
            slot2.color = UnSelectedSlot;
            sfx.PlayOneShot(switchClip);

        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            mode = 2;
            HBarrier.SetActive(true);

            slot2.color = SelectedSlot;
            slot1.color = UnSelectedSlot;
            sfx.PlayOneShot(switchClip);
        }




        switch(mode)
        {
            case 1:
                if (Input.GetMouseButtonDown(0))
                {
                    spray.ActivateSpray();
                }
                if (Input.GetMouseButtonUp(0))
                {
                    spray.DeactivateSpray();
                }
                break;


            case 2:
                spray.DeactivateSpray();
                HBarrier.transform.position = pointing + Vector3.up * 0.7f;
                Vector3 d = transform.position - HBarrier.transform.position;
                d.y = 0;
                HBarrier.transform.rotation = Quaternion.LookRotation(d);


                
               

                if (Input.GetMouseButtonDown(0)&&bcount>0)
                {
                    sfx.PlayOneShot(tapeClip);
                    bcount--;
                    barrierText.text = "" + bcount;
                    GameObject g = Instantiate(Barrier, HBarrier.transform.position, HBarrier.transform.rotation);
                    g.GetComponent<Barrier>().OnEnd = UpdateUi;
                }

                if (Input.GetMouseButtonDown(1))
                {
                    
                    rBox = Physics.OverlapBox(pointing, new Vector3(.15f, .73f, 6f), HBarrier.transform.rotation, 1 << LayerMask.NameToLayer("Barrier"));

                    if (rBox.Length > 0)
                    {
                        sfx.PlayOneShot(tape2Clip);
                        rBox[0].transform.root.GetComponent<Barrier>().RemoveBarrier();
                        //Debug.Log("HIT SOMETHING " + rBox.Length + " , " + rBox[0].name);
                    }

                }
                break;

        }
       

       

        bool v = Input.GetButton("Jump");
        if (v)
        {
            if(visorENERGY > .25f)
            {
                if(!visor)
                {
                    sfx.PlayOneShot(visorClip);
                }
                visor = true;
                Visor.SetActive(visor);
                _visorPrice = true;
            }
            

        }
        else
        {
            if(_visorPrice&&visorENERGY>0)
            {
                
                _visorPriceT += Time.deltaTime;

                if(_visorPriceT>0.25f)
                {
                    visor = false;
                    Visor.SetActive(visor);
                    _visorPrice = false;
                    _visorPriceT = 0;
                }
            }
            else
            {
                visor = false;
                Visor.SetActive(visor);
                _visorPrice = false;
                _visorPriceT = 0;
            }
           
        }

        if(visor)
        {
            visorENERGY = Mathf.Clamp01(visorENERGY - Time.deltaTime*.25f);

            if(visorENERGY<=0)
            {
                visor = false;
                
                Visor.SetActive(visor);
            }
        }
        else
        {

            if(visorENERGY<0.25f)
            {
                visorgauge.color = tcolor;
            }
            else
            {
                visorgauge.color = ocolor;
            }

            visorENERGY = Mathf.Clamp01(visorENERGY + Time.deltaTime*.05f);
        }

        Vector2 ns = oGauge;
        ns.x = oGauge.x * visorENERGY;
        visorgauge.rectTransform.sizeDelta = ns;

        if(restock)
        {
            restEm.enabled = true;
            stockTime += Time.deltaTime;
            if(stockTime>1)
            {
                st = st + 1;
                sfx.PlayOneShot(ammoClip);
                //spray.ammo = Mathf.Clamp(spray.ammo+(st/10)*100, 0, 100);

                spray.ReplenishAmmo((st / 10) * 100);
                stockTime = 0;

                if(spray.ammo>=100)
                {
                    restEm.enabled = false;
                    restock = false;
                    stockTime = 0;
                    st = 0;
                }
                    
            }
        }

    }


    private void FixedUpdate()
    {


        //body.AddForce(Vector3.right * 5, ForceMode.Impulse);

        Vector3 targetVel = input.normalized * speed;
        Vector3 vel = body.velocity;
        targetVel = targetVel - vel;
        targetVel.y = 0;
        body.AddForce(targetVel, ForceMode.Impulse);

        //Vector3 vel = body.velocity;
        //Vector3 desvel = input.normalized * speed;
        //vel.x = desvel.x;
        //vel.z = desvel.z;
        //body.velocity = vel;
    }

    void UpdateUi()
    {
        barrierText.text = "" + bcount;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Reload")&&spray.ammo<100)
        {
            Debug.Log("Reload WPN");
            restock = true;
            sfx.PlayOneShot(restClip);
        }
    }


    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Reload"))
        {
            visorENERGY = Mathf.Clamp01(visorENERGY + Time.deltaTime * .2f);
        }
    }


    

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Reload"))
        {
            restEm.enabled = false;
            //Debug.Log("Reload WPN");
            stockTime = 0;
            st = 0;
            restock = false;
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(pointing, 0.5f);
    }

}
