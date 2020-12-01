using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleCardHide : MonoBehaviour
{
    private float countdown=2f;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("Timer");
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    IEnumerator Timer()
    {
        while (countdown > 0)
        {
            countdown -= 0.1f;
            yield return new WaitForSeconds(.1f);
        }

        //2 Seconds Elapsed
        gameObject.SetActive(false);

    }
}
