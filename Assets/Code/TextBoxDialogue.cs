using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextBoxDialogue : MonoBehaviour
{
    public Queue<string> SentenceSequence;
    public GameObject textBox, characterController, audioSpeech;
    private TextMeshProUGUI textDisplay;
    private CharacterMangerScript characterManager;
    private AudioSource speechSource;

    private bool printingCharacters, printAll = false;
    private float printSpeed=0.1f;
    private string playerName = TextAssetToDialogueReader.characterName;

    

    // Start is called before the first frame update
    void Start()
    {
        printingCharacters = false;

        textDisplay = textBox.GetComponent<TextMeshProUGUI>();
        characterManager = characterController.GetComponent<CharacterMangerScript>();
        speechSource = audioSpeech.GetComponent<AudioSource>();

        //LoadTestSentenceSequenceFromFile();
        playerName = TextAssetToDialogueReader.characterName;
    }

    public void SetName(string input)
    {
        playerName = input;
    }

    private void LoadTestSentenceSequence()
    {
        SentenceSequence = new Queue<string>();
        SentenceSequence.Enqueue("<b>Chalmers: </b>Well, Seymour, I made it- despite your directions.");
        SentenceSequence.Enqueue("{Principal Skinner:} Ah. Superintendent Chalmers. Welcome. - I hope you're prepared for an unforgettable luncheon.");
        SentenceSequence.Enqueue("{<b>Chalmers:</b>} Yeah.");
        SentenceSequence.Enqueue("<b>{Principal Skinner:}</b> Oh, egads! My roast is ruined. But what if I were to purchase fast food and disguise it as my own cooking? Delightfully devilish, Seymour.");
        SentenceSequence.Enqueue("Singers: Ah - Skinner with his crazy explanations The superintendent's gonna need his medication When he hears Skinner's lame exaggerations There'll be trouble in town tonight!");
        SentenceSequence.Enqueue("Chalmers: Seymour!");
        SentenceSequence.Enqueue("Principal Skinner: Superintendent, I was just- uh, just stretching my calves on the windowsill. Isometric exercise. Care to join me?");
        SentenceSequence.Enqueue("Chalmers: Why is there smoke coming out of your oven, Seymour?");
        SentenceSequence.Enqueue("Principal Skinner: Uh- Oh. That isn't smoke. It's steam. Steam from the steamed clams we're having. Mmm. Steamed clams.");
        SentenceSequence.Enqueue("Principal Skinner: Whew. Superintendent, I hope you're ready for mouthwatering hamburgers.");
        SentenceSequence.Enqueue("Chalmers: I thought we were having steamed clams.");
        SentenceSequence.Enqueue("Principal Skinner: D'oh, no. I said steamed hams. That's what I call hamburgers.");
        SentenceSequence.Enqueue("Chalmers: You call hamburgers steamed hams?");
        SentenceSequence.Enqueue("Principal Skinner: Yes. It's a regional dialect.");
        SentenceSequence.Enqueue("Chalmers: Uh-huh. Uh, what region?");
        SentenceSequence.Enqueue("Principal Skinner: Uh, upstate New York.");
        SentenceSequence.Enqueue("Chalmers: Really. Well, I'm from Utica, and I've never heard anyone use the phrase 'steamed hams.'");
        SentenceSequence.Enqueue("Principal Skinner: Oh, not in Utica. No. It's an Albany expression.");
        SentenceSequence.Enqueue("Chalmers: I see. You know, these hamburgers are quite similar to the ones they have at Krusty Burger.");
        SentenceSequence.Enqueue("Principal Skinner: Oh, no. Patented Skinner burgers. Old family recipe.");
        SentenceSequence.Enqueue("Chalmers: For steamed hams.");
        SentenceSequence.Enqueue("Principal Skinner: Yes.");
        SentenceSequence.Enqueue("Chalmers: Yes. And you call them steamed hams despite the fact that they are obviously grilled.");
        SentenceSequence.Enqueue("Principal Skinner: Ye- You know, the- One thing I should- - Excuse me for one second.");
        SentenceSequence.Enqueue("Chalmers: Of course.");
        SentenceSequence.Enqueue("Principal Skinner: Oh well, that was wonderful. A good time was had by all. I'm pooped.");
        SentenceSequence.Enqueue("Chalmers: Yes. I should be- Good Lord! What is happening in there?");
        SentenceSequence.Enqueue("Principal Skinner: Aurora borealis.");
        SentenceSequence.Enqueue("Chalmers: Uh- Aurora borealis at this time of year at this time of day in this part of the country localized entirely within your kitchen?");
        SentenceSequence.Enqueue("Principal Skinner: Yes.");
        SentenceSequence.Enqueue("Chalmers: May I see it?");
        SentenceSequence.Enqueue("Principal Skinner: No.");
        SentenceSequence.Enqueue("Skinner's Mother: Seymour! The house is on fire!");
        SentenceSequence.Enqueue("Principal Skinner: No, Mother. It's just the northern lights.");
        SentenceSequence.Enqueue("Chalmers: Well, Seymour, you are an odd fellow but I must say you steam a good ham.");
        SentenceSequence.Enqueue("Skinner's Mother: Help! Help!");


    }

    public void LoadTestSentenceSequenceFromFile()
    {
        
        SentenceSequence = new Queue<string>();
        
        SentenceSequence = TextAssetToDialogueReader.ReadText("SteamedHams");
    }


    public void LoadSentenceSequence(Queue<string> input)
    {
        SentenceSequence = new Queue<string>();
        SentenceSequence = input;

    }

    public void LoadSentenceSequence(Queue<string> input, bool loadNext)
    {
        SentenceSequence = new Queue<string>();
        //Debug.Log(SentenceSequence);
        SentenceSequence = input;

        if (loadNext)
        {
            loadNextBox();
        }
        
    }


    private void loadNextBox()
    {
        string text;
        int len;

        len = SentenceSequence.Count;
        if (len > 0)
        {
            text = SentenceSequence.Dequeue();
            printingCharacters = true;
            StartCoroutine("DisplayText", text);
        }
        else
        {
            Debug.Log("Load empty error");
        }
    }

    public void ButtonClicked()
    {
        Debug.Log("Button Clicked");
        string text;
        int len;

        if (printingCharacters)
        {
            printAll = true;
        }
        else
        {
            len = SentenceSequence.Count;

            Debug.Log("Text Box Num: " + len);

            if (len > 0)
            {
                text = SentenceSequence.Dequeue();
                printingCharacters = true;
                StartCoroutine("DisplayText", text);
                

            }
            else
            {
                //End of conversation Sequence
                characterManager.ReadyServing();
            }
        }
        

        
        
    }

    IEnumerator DisplayText(string text)
    {
        bool tagged = false, quickTag = false;
        textDisplay.text="";

        foreach (char c in text)
        {
            if(printAll)
            {
                textDisplay.text = "";
                foreach (char character in text)
                {
                    if (character == '@')
                    {
                        textDisplay.text += playerName;
                    }
                    else if ( (character == '{' || character == '}') == false )
                    {
                        textDisplay.text += character;
                    }    
                }
                break;
            }

            if (c == '@')
            {
                textDisplay.text += playerName;
            }
            else if (c == '}')
            {
                quickTag = false;
            }
            else if (c == '{')
            {
                quickTag = true;
            }
            else if (quickTag)
            {
                textDisplay.text += c;
            }
            else if (c == '>')
            {
                tagged = false;
                //Debug.Log("CloseTag");
                textDisplay.text += c;
            }
            else if (tagged || c == '<')
            {
                tagged = true;
                textDisplay.text += c;
            }
            else
            {
                textDisplay.text += c;

                if (speechSource.isPlaying)
                {
                    //speechSource.Stop();
                }
                speechSource.Play();
                yield return new WaitForSeconds(printSpeed);
            }

            
        }
        printingCharacters = false;
        printAll = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
