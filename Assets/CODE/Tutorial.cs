using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public static bool TUTDONE;
    public AudioSource BGM;
    public TopDown3DController player;
    // Start is called before the first frame update
    void Start()
    {
        if(!TUTDONE)
        {
            player.enabled = false;
            BGM.Stop();
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
            gameObject.SetActive(false);
        }
    }

    public void EndTutorial()
    {
        player.enabled = true;
        TUTDONE = true;
        gameObject.SetActive(false);
        Time.timeScale = 1;
        BGM.Play();
    }
}
