using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprayFire : MonoBehaviour
{





    // Start is called before the first frame update
    private void OnParticleCollision(GameObject other)
    {
        //Debug.Log(other.name);

        if(other.layer==LayerMask.NameToLayer("Virus"))
        {
            other.GetComponent<Virus>().Damage(2F * Time.deltaTime);
        }

    }


}
