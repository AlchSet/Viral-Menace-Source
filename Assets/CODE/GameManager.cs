using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public NPC[] npc;

    public int maxPopulation;
    public int currentPopulation;
    public int infected;

    public Text popText;
    public Text infText;

    Transform camParent;

    Vector3 oCamPos;


    bool inShake;

    public CanvasGroup GameOverG;
    public CanvasGroup VictoryG;

    public AudioSource BGM;


    public Text casualties;
    public Text messageTxt;
    public Image VictoryBG;


    public Gradient VictoryGradient;
    public Gradient VictoryTextLoop;

    public string[] victoryMSGs = {
        "You are the mighty Purger!",
        "Great Success!",
        "Great!",
        "Could do better.",
        "Meh..",
        "...",
        "You outright SUCK" };


    AudioSource sfx;

    public AudioClip type1;
    public AudioClip type3;
    public AudioClip VictorySound;

    public bool _VICTORY;

    //Camera

    // Start is called before the first frame update
    void Start()
    {
        sfx = GetComponent<AudioSource>();
        camParent = GameObject.Find("CamParent").transform;
        oCamPos = camParent.localPosition;


        foreach (NPC n in npc)
        {
            n.OnVirusStateChanged = CheckStatus;
        }

        maxPopulation = npc.Length;
        CheckStatus();

    }

    // Update is called once per frame
    void Update()
    {
        if (_VICTORY)
        {
            StartCoroutine(VictorySequence());
            this.enabled = false;
        }
    }


    void CheckStatus()
    {
        currentPopulation = 0;
        infected = 0;
        foreach (NPC n in npc)
        {
            if (n.enabled)
            {
                currentPopulation++;


                if (n.hasVirus)
                {
                    infected++;
                }

            }

        }

        popText.text = currentPopulation + "/" + maxPopulation;
        infText.text = infected + "/" + currentPopulation;




        if (currentPopulation == 0)
        {
            BGM.Stop();
            Time.timeScale = 0;
            StartCoroutine(GameOverSequence());
            //GameOverG.alpha = 1;
            return;
        }

        if (infected == 0)
        {
            BGM.Stop();
            Time.timeScale = 0;
            StartCoroutine(VictorySequence());

        }
    }

    public void PlayShake()
    {
        if (!inShake)
        {
            StartCoroutine(Shake());
        }
    }

    IEnumerator Shake()
    {
        inShake = true;
        float time = 0;
        while (true)
        {
            Vector3 npos = oCamPos + (Vector3.up * Mathf.Sin(Time.time * 60) * 1f) + (Vector3.right * Mathf.Cos(Time.time * 60) * 1f);

            camParent.localPosition = npos;

            time += Time.deltaTime;

            if (time > 0.5f)
            {
                camParent.localPosition = oCamPos;

                break;
            }
            yield return new WaitForEndOfFrame();
        }
        inShake = false;


    }

    IEnumerator GameOverSequence()
    {
        GameOverG.gameObject.SetActive(true);
        while (true)
        {
            GameOverG.alpha += Time.unscaledDeltaTime;
            yield return new WaitForSecondsRealtime(0.01f);
        }

    }
    IEnumerator VictorySequence()
    {

        VictoryG.gameObject.SetActive(true);
        float i = (float)currentPopulation / (float)maxPopulation;
        string m = "";
        if (currentPopulation == maxPopulation)
        {
            //EXCELLENT
            m = victoryMSGs[0];
        }
        else if (currentPopulation < maxPopulation && currentPopulation > 10)
        {//GREAT SUCCESS
            m = victoryMSGs[1];
        }
        else if (currentPopulation < 11 && currentPopulation > 7)
        {//GOOD JOB
            m = victoryMSGs[2];
        }
        else if (currentPopulation < 8 && currentPopulation > 5)
        {//COULD DO BETTER
            m = victoryMSGs[3];
        }
        else if (currentPopulation < 6 && currentPopulation > 3)
        {//MEH
            m = victoryMSGs[4];
        }
        else if (currentPopulation < 4 && currentPopulation > 1)
        {//...
            m = victoryMSGs[5];
        }
        else if (currentPopulation == 1)
        {//U SUCK
            m = victoryMSGs[6];
        }


        VictoryBG.color = VictoryGradient.Evaluate(i);
        while (true)
        {
            VictoryG.alpha += Time.unscaledDeltaTime;
            yield return new WaitForSecondsRealtime(0.01f);
            if (VictoryG.alpha >= 1)
            {
                break;
            }
        }

        yield return new WaitForSecondsRealtime(1f);
        sfx.PlayOneShot(type1);
        casualties.text = "" + (maxPopulation - currentPopulation);
        yield return new WaitForSecondsRealtime(1f);
        sfx.PlayOneShot(type3);
        messageTxt.text = m;
        yield return new WaitForSecondsRealtime(.45f);


        if(currentPopulation>6)
        {
            sfx.PlayOneShot(VictorySound);
        }

        if (currentPopulation >= 13)
        {
            while (true)
            {
                i = (i + Time.unscaledDeltaTime) % 1;

                messageTxt.color = VictoryTextLoop.Evaluate(i);
                yield return new WaitForSecondsRealtime(0.01f);
            }
        }

    }

    public void ReloadGame()
    {
        SceneManager.LoadScene(0);
    }

}
