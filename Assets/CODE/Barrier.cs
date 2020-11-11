using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour
{

    float t;

    public delegate void BarrierEvent();

    public BarrierEvent OnEnd;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    //void Update()
    //{
    //    //t += Time.deltaTime;


    //    //if(t>60)
    //    //{
    //    //    TopDown3DController.bcount = Mathf.Clamp(TopDown3DController.bcount + 1, 0, 6);

    //    //    if(OnEnd!=null)
    //    //    {
    //    //        OnEnd();
    //    //    }

    //    //    Destroy(gameObject);
    //    //}
    //}


    public void RemoveBarrier()
    {
        TopDown3DController.bcount = Mathf.Clamp(TopDown3DController.bcount + 1, 0, 6);

        if (OnEnd != null)
        {
            OnEnd();
        }

        Destroy(gameObject);
    }
}
