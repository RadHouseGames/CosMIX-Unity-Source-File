using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicLoopManager : MonoBehaviour
{
    public GameObject[] loops;
    private AudioSource[] loopSources;

    private int musicState;

    // Start is called before the first frame update
    void Start()
    {
        musicState = -1;
        loopSources = new AudioSource[loops.Length];
        for(int i=0; i < loops.Length; i++)
        {
            loopSources[i] = loops[i].GetComponent<AudioSource>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (musicState >= 0)
        {
            //Stop all music
            StopAll();

            loopSources[musicState].Play();

            musicState = -1;
        }

        

    }

    public void SetLoop(int input)
    {
        musicState = input;
    }


    private void StopAll()
    {
        foreach(AudioSource source in loopSources)
        {
            source.Stop();
        }
    }
   
}
