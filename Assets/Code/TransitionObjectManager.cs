using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionObjectManager : MonoBehaviour
{
    public GameObject charObj;
    public GameObject[] proceedScreens;
    public GameObject[] titleScreens;
    private CharacterMangerScript charScript;
    private int day;
    private float countdown;


    // Start is called before the first frame update
    void Start()
    {
        charScript = charObj.GetComponent<CharacterMangerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartTransition(int dayInput)
    {

        day = dayInput;

        if (day == 0)
        {
            proceedScreens[0].SetActive(true);
        }
        else
        {
            proceedScreens[1].SetActive(true);
        }


    }

    public void LoadTitleCard()
    {
        countdown = 1.9f;
        titleScreens[day].SetActive(true);
        StartCoroutine("Timer");
    }


    IEnumerator Timer()
    {
        while (countdown > 0 )
        {
            countdown -= 0.1f;
            yield return new WaitForSeconds(.1f);
        }

        //2 Seconds Elapsed
        if(countdown <= 0)
        {
            charScript.SetDayStart(true);
        }
        StopCoroutine("Timer");
    }
}
