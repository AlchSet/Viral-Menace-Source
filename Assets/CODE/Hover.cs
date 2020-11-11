using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hover : MonoBehaviour
{
    Vector3 oPos;
    public bool useLocal;

    public float freq = 2.5f;
    public float mag = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        if(useLocal)
        {
            oPos = transform.localPosition;
        }
        else
        {
            oPos = transform.position;
        }
    }
        

    // Update is called once per frame
    void Update()
    {
        if (useLocal)
        {
            transform.localPosition = oPos + Vector3.up * Mathf.Sin(Time.unscaledTime * freq) * mag;
        }
        else
        {
            transform.position = oPos + Vector3.up * Mathf.Sin(Time.unscaledTime * freq) * mag;
        }
        


    }
}
