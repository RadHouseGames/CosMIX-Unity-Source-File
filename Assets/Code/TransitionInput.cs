using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionInput : MonoBehaviour
{
    public GameObject transitionController, musicController;
    private TransitionObjectManager transitionControls;
    private MusicLoopManager musicControls;
    // Start is called before the first frame update
    void Start()
    {
        transitionControls = transitionController.GetComponent<TransitionObjectManager>();
        musicControls = musicController.GetComponent<MusicLoopManager>();
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            musicControls.SetLoop(5);
            transitionControls.LoadTitleCard();
            gameObject.SetActive(false);
        }
    }
}
