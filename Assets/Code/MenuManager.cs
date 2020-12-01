using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("ERefs")]
    public TMP_InputField nameInput;
    public GameObject startScreen;
    public GameObject menuScreen;

    private bool nameStage = false;
    private void Start()
    {
        menuScreen.SetActive(true);
        startScreen.SetActive(false);
    }

    private bool keydownDone;
    public void Update()
    {
        if (Input.anyKeyDown && !keydownDone)
        {
            keydownDone = true;
            menuScreen.GetComponent<Animator>().Play("Main");
            startScreen.GetComponent<Animator>().Play("Sec");
            Invoke("StartGame", 1);
        }
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            startScreen.GetComponent<Animator>().Play("Main");
            menuScreen.GetComponent<Animator>().Play("Sec");
            Invoke("Play", 1);
        }
    }

    public void StartGame()
    {
        menuScreen.SetActive(false);
        startScreen.SetActive(true);
        nameStage = true;
    }
    public void LoseGame()
    {
        menuScreen.SetActive(true);
        startScreen.SetActive(false);
    }
    public void Play()
    {
        TextAssetToDialogueReader.characterName = nameInput.text;
        menuScreen.SetActive(false);
        startScreen.SetActive(false);
        SceneManager.LoadScene(1);
    }


}
