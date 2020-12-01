using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMangerScript : MonoBehaviour
{
    public GameObject[] characterAnchors, characters;
    public GameObject textBoxController, drinkManagerObject, transitionController, musicController;
    private DrinkManager drinkManagerScript;
    private TextBoxDialogue textBoxControls;
    private TransitionObjectManager transitionControls;
    private MusicLoopManager musicControlls;

    private bool served, readyToServe, nodeContainsOrder, dayChange, dayStart, dayStarted;
    private int day, count, textTagIndex, nodeListIndex, customerState, timingState;

    private Recipy.DrinkName servedDrink = Recipy.DrinkName.NULL;
    private Recipy.DrinkQuality servedQuality;

    private List<DialogueNode> currentDayNodes = new List<DialogueNode>();
    private List<DialogueNode> day0Nodes = new List<DialogueNode>();
    private List<DialogueNode> day1Nodes = new List<DialogueNode>();
    private List<DialogueNode> day2Nodes = new List<DialogueNode>();
    private List<DialogueNode> day3Nodes = new List<DialogueNode>();
    private List<DialogueNode> day4Nodes = new List<DialogueNode>();

    // Start is called before the first frame update
    void Start()
    {
        textBoxControls = textBoxController.GetComponent<TextBoxDialogue>();
        drinkManagerScript = drinkManagerObject.GetComponent<DrinkManager>();
        transitionControls = transitionController.GetComponent<TransitionObjectManager>();
        musicControlls = musicController.GetComponent<MusicLoopManager>();

        dayStarted = false;
        dayStart = false;
        dayChange = true;

        served = false;
        readyToServe = false;
        timingState = 0;

        day = 0;

        
    }

    // Update is called once per frame
    void Update()
    {
        if (dayChange)
        {
            dayChange = false;
            //Bring Up Next Transition Screen
            dayTransition();
            transitionControls.StartTransition(day);
        }
        else if(dayStart)
        {
            dayStart = false;
            musicControlls.SetLoop(day);
            SetDay(day);
            dayStarted = true;
        }
        else if (dayStarted)
        {
            TimingControlStateMachine();
        }

        //In transition Screen, exited when SetDayStart called
        
    }



    #region functionalityMethods definition  

    public void SetDayStart(bool input)
    {
        dayStart = input;
    }

    public void SetDay(int input)
    {
        day = input;

        switch (day)
        {
            case 0:
                Day0();
                break;
            case 1:
                Day1();
                break;
            case 2:
                Day2();
                break;
            case 3:
                Day3();
                break;
            case 4:
                Day4();
                break;
            default:
                Debug.Log("No day specified");
                break;
        }
    }

    private void dayTransition()
    {
        served = false;
        readyToServe = false;
        timingState = 0;
        nodeListIndex = 0;
    }

    private void TimingControlStateMachine()
    {
        switch (timingState)
        {
            case 0:
                Debug.Log("Node " + nodeListIndex + "Timing control started");
                timingState = 1;
                break;
            case 1:
                if (readyToServe)
                {
                    Debug.Log("Node " + nodeListIndex + " Ready to serve");
                    nodeContainsOrder = currentDayNodes[nodeListIndex].hasDrink;
                    timingState = 2;
                }
                break;
            case 2:
                if (nodeContainsOrder)
                {
                    served = false;
                    Debug.Log("Node " + nodeListIndex + "contains order");

                    if (servedDrink != Recipy.DrinkName.NULL)
                    {
                        served = true;
                    }

                }
                else
                {
                    served = true;
                    customerState = 0;
                    Debug.Log("Node " + nodeListIndex + "does not contain order");
                }

                timingState = 3;
                break;
            case 3:
                if (served)
                {
                    Debug.Log("Node " + nodeListIndex + " completed");
                    timingState = 4;
                }
                break;

            case 4:
                NextNode(currentDayNodes[nodeListIndex]);
                if (nodeListIndex < currentDayNodes.Count )
                {
                    
                    Debug.Log("Loading Next file at node " + nodeListIndex);


                    served = false;
                    readyToServe = false;
                    customerState = 0;
                    timingState = 0;

                }
                else
                {
                    Debug.Log("End of Day State Transition");
                    timingState = 5;
                    day +=1;
                    dayChange = true;
                }
                break;
        }
    }


    private void SetCharacterPosition(int charIndex, int posIndex)
    {
        characters[charIndex].transform.position = characterAnchors[posIndex].transform.position;
    }

    private int LoadTextSequence(string filename)
    {
        int len;
        Queue<string> file = new Queue<string>();
        file = TextAssetToDialogueReader.ReadText(filename);
        len = file.Count;
        textBoxControls.LoadSentenceSequence(file);
        return len;
    }

    private int LoadTextSequence(string filename, bool firstText)
    {
        int len;
        Queue<string> file = new Queue<string>();
        file = TextAssetToDialogueReader.ReadText(filename);
        len = file.Count;
        textBoxControls.LoadSentenceSequence(file, firstText);
        return len;
    }

    public void ReadyServing()
    {
        readyToServe = true;

    }

    public void ServeOrder(Recipy.DrinkName drinkName, Recipy.DrinkQuality drinkQuality)
    {
        servedDrink = drinkName;
        servedQuality = drinkQuality;
        served = true;
    }


    private int finishOrder(Recipy.DrinkName[] orderOptions)
    {
        int state;
        bool inArray=false;

        foreach (Recipy.DrinkName order in orderOptions)
        {
            if (servedDrink==order)
            {
                inArray = true;
            }
            
        }
        if (servedDrink==orderOptions[0] && servedQuality == Recipy.DrinkQuality.Perfect)
        {
            //Perfect Drink, Perfect Quality
            state = 0;
        }
        else if(servedDrink == orderOptions[0] && servedQuality == Recipy.DrinkQuality.Good)
        {
            //Perfect Drink, Good Quality
            state = 1;
        }
        else if (servedDrink == orderOptions[0])
        {
            //Perfect Drink, Bad or Sludge Quality
            state = 2;
        }
        else if (inArray && servedQuality == Recipy.DrinkQuality.Perfect)
        {
            //Good Drink, Perfect Quality
            state = 3;
        }
        else if (inArray && servedQuality == Recipy.DrinkQuality.Good)
        {
            //Good Drink, Good Quality
            state = 4;
        }
        else if (inArray)
        {
            //Good Drink, Bad or Sludge Quality
            state = 5;
        }
        else if (servedDrink == Recipy.DrinkName.Sludge)
        {
            //Sludge Served, Sludge not desired drink
            state = 6;
        }
        else if (servedDrink ==Recipy.DrinkName.NULL)
        {
            state = 7;
            Debug.Log("Error Null Drink Served");
        }
        else
        {
            //Disliked non sludge drink
            state = 7;
        }


        //Calculate paid price and send to drink Manager
        CalcPrice(servedDrink);

        servedDrink = Recipy.DrinkName.NULL;
        servedQuality = Recipy.DrinkQuality.NULL;

        return state;
    }

    private void NextNode(DialogueNode curNode)
    {

        if (curNode.movesChar)
        {
            for( int i=0; i < curNode.character.Length; i++  )
            {
                SetCharacterPosition(curNode.character[i], curNode.targetAnchor[i]);
            }
        }

        if (curNode.hasDrink)
        {
            customerState = finishOrder(curNode.likedDrinks);
        }
        else
        {
            customerState = 0;
        }

        nodeListIndex += curNode.indexShift[customerState];
        Debug.Log(nodeListIndex);

        if (nodeListIndex < currentDayNodes.Count)
        {
            count = LoadTextSequence(curNode.nextLinks[customerState], curNode.hasDrink);
        }

        
    }

    private void CalcPrice(Recipy.DrinkName drinkName)
    {
        float toPay, priceMult;

        switch (drinkName)
        {
            case Recipy.DrinkName.Moonshine:
                toPay = 13;
                break;
            case Recipy.DrinkName.Bloodmoon:
                toPay = 10;
                break;
            case Recipy.DrinkName.Fruitmoon:
                toPay = 12;
                break;
            case Recipy.DrinkName.Midnightmoon:
                toPay = 27;
                break;
            case Recipy.DrinkName.SexOnTheMoon:
                toPay = 18;
                break;
            case Recipy.DrinkName.ShootingStar:
                toPay = 22;
                break;
            case Recipy.DrinkName.VoidsRevenge:
                toPay = 45;
                break;
            case Recipy.DrinkName.VoidOrangeJuice:
                toPay = 3;
                break;
            case Recipy.DrinkName.MoonberryJuice:
                toPay = 3;
                break;
            case Recipy.DrinkName.ShotOfCosmicLiquer:
                toPay = 5;
                break;
            case Recipy.DrinkName.GlassFromTheUnderWorld:
                toPay = 30;
                break;
            case Recipy.DrinkName.WaterCyclone:
                toPay = 2;
                break;
            default:
                toPay = 1;
                break;
        }
        


        switch (customerState)
        {
            case 0:
                priceMult = 2;
                break;
            case 1:
                priceMult = 1.5f;
                break;
            case 2:
                priceMult = 1;
                break;
            case 3:
                priceMult = 1.2f;
                break;
            case 4:
                priceMult = 1;
                break;
            case 5:
                priceMult = 0.8f;
                break;
            case 6:
                priceMult = 0;
                break;
            case 7:
                priceMult = 0.5f;
                break;
            default:
                priceMult = 0;
                break;
        }

        toPay = priceMult*toPay;

        drinkManagerScript.Pay((int)toPay);



    }


    #endregion

    #region Day0 definition  

    private void Day0ListInit()
    {
        //Tut1 already loaded

        //Drink1
        day0Nodes.Add(new DialogueNode("Tut2Drink1", 1));
        day0Nodes.Add(new DialogueNode(new string[] { "Tut2Drink1Feedback1", "Tut2Drink1Feedback2", "Tut2Drink1Feedback3", "Tut2Drink1Feedback4-6", "Tut2Drink1Feedback4-6", "Tut2Drink1Feedback4-6", "Tut2Drink1Feedback7-8", "Tut2Drink1Feedback7-8" }, new Recipy.DrinkName[] { Recipy.DrinkName.WaterCyclone, Recipy.DrinkName.Bloodmoon, Recipy.DrinkName.Fruitmoon, Recipy.DrinkName.GlassFromTheUnderWorld, Recipy.DrinkName.Midnightmoon, Recipy.DrinkName.MoonberryJuice, Recipy.DrinkName.Moonshine, Recipy.DrinkName.SexOnTheMoon, Recipy.DrinkName.ShootingStar, Recipy.DrinkName.ShotOfCosmicLiquer, Recipy.DrinkName.VoidOrangeJuice, Recipy.DrinkName.VoidsRevenge }, new int[] { 3, 3, 1, 1, 1, 1, 1, 1}));
        day0Nodes.Add(new DialogueNode("Tut2Drink1Remake", 1));
        //Drink1 Remake
        day0Nodes.Add(new DialogueNode(new string[] { "Tut2Drink1Feedback1", "Tut2Drink1Feedback2", "Tut2Drink1Feedback3", "Tut2Drink1Feedback4-6", "Tut2Drink1Feedback4-6", "Tut2Drink1Feedback4-6", "Tut2Drink1Feedback7-8", "Tut2Drink1Feedback7-8" }, new Recipy.DrinkName[] { Recipy.DrinkName.WaterCyclone, Recipy.DrinkName.Bloodmoon, Recipy.DrinkName.Fruitmoon, Recipy.DrinkName.GlassFromTheUnderWorld, Recipy.DrinkName.Midnightmoon, Recipy.DrinkName.MoonberryJuice, Recipy.DrinkName.Moonshine, Recipy.DrinkName.SexOnTheMoon, Recipy.DrinkName.ShootingStar, Recipy.DrinkName.ShotOfCosmicLiquer, Recipy.DrinkName.VoidOrangeJuice, Recipy.DrinkName.VoidsRevenge }, new int[] { 1, 1, -1, -1, -1, -1, -1, -1 }));
        //Drink2
        day0Nodes.Add(new DialogueNode("Tut2Drink2", 1));
        day0Nodes.Add(new DialogueNode(new string[] { "Tut2Drink2Feedback1", "Tut2Drink2Feedback2", "Tut2Drink2Feedback3", "Tut2Drink2Feedback4-6", "Tut2Drink2Feedback4-6", "Tut2Drink2Feedback4-6", "Tut2Drink2Feedback7-8", "Tut2Drink2Feedback7-8" }, new Recipy.DrinkName[] { Recipy.DrinkName.VoidOrangeJuice, Recipy.DrinkName.Bloodmoon, Recipy.DrinkName.Fruitmoon, Recipy.DrinkName.GlassFromTheUnderWorld, Recipy.DrinkName.Midnightmoon, Recipy.DrinkName.MoonberryJuice, Recipy.DrinkName.Moonshine, Recipy.DrinkName.SexOnTheMoon, Recipy.DrinkName.ShootingStar, Recipy.DrinkName.ShotOfCosmicLiquer, Recipy.DrinkName.VoidsRevenge, Recipy.DrinkName.WaterCyclone }, new int[] { 3, 3, 1, 1, 1, 1, 1, 1 }));
        day0Nodes.Add(new DialogueNode("Tut2Drink2Remake", 1));
        //Drink 2 Remake
        day0Nodes.Add(new DialogueNode(new string[] { "Tut2Drink2Feedback1", "Tut2Drink2Feedback2", "Tut2Drink2Feedback3", "Tut2Drink2Feedback4-6", "Tut2Drink2Feedback4-6", "Tut2Drink2Feedback4-6", "Tut2Drink2Feedback7-8", "Tut2Drink2Feedback7-8" }, new Recipy.DrinkName[] { Recipy.DrinkName.VoidOrangeJuice, Recipy.DrinkName.Bloodmoon, Recipy.DrinkName.Fruitmoon, Recipy.DrinkName.GlassFromTheUnderWorld, Recipy.DrinkName.Midnightmoon, Recipy.DrinkName.MoonberryJuice, Recipy.DrinkName.Moonshine, Recipy.DrinkName.SexOnTheMoon, Recipy.DrinkName.ShootingStar, Recipy.DrinkName.ShotOfCosmicLiquer, Recipy.DrinkName.VoidsRevenge, Recipy.DrinkName.WaterCyclone }, new int[] { 1, 1, -1, -1, -1, -1, -1, -1 }));
        //Drink3
        day0Nodes.Add(new DialogueNode("Tut3Drink3", 1));
        day0Nodes.Add(new DialogueNode(new string[] { "Tut3Drink3Feedback1", "Tut3Drink3Feedback2", "Tut3Drink3Feedback3", "Tut3Drink3Feedback4-6", "Tut3Drink3Feedback4-6", "Tut3Drink3Feedback4-6", "Tut3Drink3Feedback7-8", "Tut3Drink3Feedback7-8" }, new Recipy.DrinkName[] { Recipy.DrinkName.Moonshine, Recipy.DrinkName.Bloodmoon, Recipy.DrinkName.Fruitmoon, Recipy.DrinkName.GlassFromTheUnderWorld, Recipy.DrinkName.Midnightmoon, Recipy.DrinkName.MoonberryJuice, Recipy.DrinkName.SexOnTheMoon, Recipy.DrinkName.ShootingStar, Recipy.DrinkName.ShotOfCosmicLiquer, Recipy.DrinkName.VoidsRevenge, Recipy.DrinkName.VoidOrangeJuice, Recipy.DrinkName.WaterCyclone }, new int[] { 3, 3, 1, 1, 1, 1, 1, 1 }));
        day0Nodes.Add(new DialogueNode("Tut3Drink3Remake", 1));
        //Drink3 Remake
        day0Nodes.Add(new DialogueNode(new string[] { "Tut3Drink3Feedback1", "Tut3Drink3Feedback2", "Tut3Drink3Feedback3", "Tut3Drink3Feedback4-6", "Tut3Drink3Feedback4-6", "Tut3Drink3Feedback4-6", "Tut3Drink3Feedback7-8", "Tut3Drink3Feedback7-8" }, new Recipy.DrinkName[] { Recipy.DrinkName.Moonshine, Recipy.DrinkName.Bloodmoon, Recipy.DrinkName.Fruitmoon, Recipy.DrinkName.GlassFromTheUnderWorld, Recipy.DrinkName.Midnightmoon, Recipy.DrinkName.MoonberryJuice, Recipy.DrinkName.SexOnTheMoon, Recipy.DrinkName.ShootingStar, Recipy.DrinkName.ShotOfCosmicLiquer, Recipy.DrinkName.VoidsRevenge, Recipy.DrinkName.VoidOrangeJuice, Recipy.DrinkName.WaterCyclone }, new int[] { 1, 1, -1, -1, -1, -1, -1, -1 }));
        //Tut4
        day0Nodes.Add(new DialogueNode("Tut4", 1));
        //End reached
        day0Nodes.Add(new DialogueNode("endofday", new int[] { 0 }, new int[] { 0 }, 1));

    }


    private void Day0()
    {
        Day0ListInit();
        SetCharacterPosition(0, 1);
        count = LoadTextSequence("Tut1", true);
        currentDayNodes = day0Nodes;
    }

    #endregion

    #region Day1 definition  

    private void Day1ListInit()
    {
        //Kaline
        //Drink1
        day1Nodes.Add(new DialogueNode(new string[] {"Day1-1Drink1Feedback1", "Day1-1Drink1Feedback2", "Day1-1Drink1Feedback2", "Day1-1Drink1Feedback2", "Day1-1Drink1Feedback3", "Day1-1Drink1Feedback3", "Day1-1Drink1Feedback3", "Day1-1Drink1Feedback4", "Day1-1Drink1Feedback3" }, new Recipy.DrinkName[] { Recipy.DrinkName.Moonshine, Recipy.DrinkName.Fruitmoon, Recipy.DrinkName.Bloodmoon }, new int[] { 1, 1, 1, 1, 1, 1, 1, 1 }));

        //Drink2
        day1Nodes.Add(new DialogueNode("Day1-12", 1));
        day1Nodes.Add(new DialogueNode(new string[] { "Day1-12Drink2Feedback1", "Day1-12Drink2Feedback2", "Day1-12Drink2Feedback2", "Day1-12Drink2Feedback2", "Day1-12Drink2Feedback3", "Day1-12Drink2Feedback3", "Day1-12Drink2Feedback3", "Day1-12Drink2Feedback4", "Day1-12Drink2Feedback3" }, new Recipy.DrinkName[] { Recipy.DrinkName.WaterCyclone, Recipy.DrinkName.VoidOrangeJuice, Recipy.DrinkName.MoonberryJuice }, new int[] { 1, 1, 1, 1, 1, 1, 1, 1 }));
        day1Nodes.Add(new DialogueNode("Day1-13", 1));
        //Kaline Leaves, Marcus Enters
        //Drink3
        day1Nodes.Add(new DialogueNode("Day1-14",new int[] { 0, 1 }, new int[] { 0, 1}, 1));
        day1Nodes.Add(new DialogueNode(new string[] { "Day1-14Drink3Feedback1", "Day1-14Drink3Feedback2", "Day1-14Drink3Feedback2", "Day1-14Drink3Feedback2", "Day1-14Drink3Feedback3", "Day1-14Drink3Feedback3", "Day1-14Drink3Feedback3", "Day1-14Drink3Feedback4", "Day1-14Drink3Feedback3" }, new Recipy.DrinkName[] { Recipy.DrinkName.Moonshine, Recipy.DrinkName.Midnightmoon, Recipy.DrinkName.ShootingStar }, new int[] { 1, 1, 1, 1, 1, 1, 1, 1 }));
        //Drink4
        day1Nodes.Add(new DialogueNode("Day1-15", 1));
        day1Nodes.Add(new DialogueNode(new string[] { "Day1-15FeedbackShot", "Day1-15FeedbackShot", "Day1-15FeedbackShot", "Day1-15Drink4Feedback1", "Day1-15Drink4Feedback2", "Day1-15Drink4Feedback3", "Day1-15Drink4Feedback3", "Day1-15Drink4Feedback4", "Day1-15Drink4Feedback3" }, new Recipy.DrinkName[] { Recipy.DrinkName.ShotOfCosmicLiquer, Recipy.DrinkName.Midnightmoon, Recipy.DrinkName.ShootingStar, Recipy.DrinkName.Moonshine }, new int[] { 1, 1, 1, 1, 1, 1, 1, 1 }));
        day1Nodes.Add(new DialogueNode("Day1-16", 1));
        //Marcus Leaves, Sean and Callum enter
        //Drink5
        day1Nodes.Add(new DialogueNode("Day1-17", new int[] { 1, 2 }, new int[] { 0, 1 }, 1));
        day1Nodes.Add(new DialogueNode(new string[] { "Day1-17Drink5Feedback1", "Day1-17Drink5Feedback2", "Day1-17Drink5Feedback2", "Day1-17Drink5Feedback2", "Day1-17Drink5Feedback3", "Day1-17Drink5Feedback3", "Day1-17Drink5Feedback3", "Day1-17Drink5Feedback4", "Day1-17Drink5Feedback3" }, new Recipy.DrinkName[] { Recipy.DrinkName.Moonshine, Recipy.DrinkName.Fruitmoon }, new int[] { 1, 1, 1, 1, 1, 1, 1, 1 }));
        //Drink6
        day1Nodes.Add(new DialogueNode("Day1-18", 1));
        day1Nodes.Add(new DialogueNode(new string[] { "Day1-18Drink6Feedback1", "Day1-18Drink6Feedback2", "Day1-18Drink6Feedback2", "Day1-18Drink6Feedback2", "Day1-18Drink6Feedback3", "Day1-18Drink6Feedback3", "Day1-18Drink6Feedback3", "Day1-18Drink6Feedback4", "Day1-18Drink6Feedback3" }, new Recipy.DrinkName[] { Recipy.DrinkName.Fruitmoon, Recipy.DrinkName.Moonshine }, new int[] { 1, 1, 1, 1, 1, 1, 1, 1 }));
        //Drink7
        day1Nodes.Add(new DialogueNode("Day1-19", 1));
        day1Nodes.Add(new DialogueNode(new string[] { "Day1-19Drink7Feedback1", "Day1-19Drink7Feedback2", "Day1-19Drink7Feedback2", "Day1-19Drink7Feedback2", "Day1-19Drink7Feedback3", "Day1-19Drink7Feedback3", "Day1-19Drink7Feedback3", "Day1-19Drink7Feedback4", "Day1-19Drink7Feedback3" }, new Recipy.DrinkName[] { Recipy.DrinkName.Bloodmoon, Recipy.DrinkName.Fruitmoon, Recipy.DrinkName.MoonberryJuice, Recipy.DrinkName.GlassFromTheUnderWorld, Recipy.DrinkName.VoidsRevenge }, new int[] { 1, 1, 1, 1, 1, 1, 1, 1 }));

        day1Nodes.Add(new DialogueNode("Day1-20", 1));
        //Callum and Shaun Leave
        day1Nodes.Add(new DialogueNode("Day1-21", new int[] { 2 }, new int[] { 0 }, 1));

        //End reached
        day1Nodes.Add(new DialogueNode("endofday",  1));
    }

    private void Day1()
    {
        Day1ListInit();
        SetCharacterPosition(0, 1);

        count = LoadTextSequence("Day1-1", true);
        currentDayNodes = day1Nodes;
    }

    #endregion

    #region Day2 definition  

    private void Day2ListInit()
    {
        //Drink1 (Kaline)
        day2Nodes.Add(new DialogueNode(new string[] { "Day2-1Drink1Feedback1", "Day2-1Drink1Feedback2", "Day2-1Drink1Feedback2", "Day2-1Drink1Feedback2", "Day2-1Drink1Feedback3", "Day2-1Drink1Feedback3", "Day2-1Drink1Feedback3", "Day2-1Drink1Feedback4", "Day2-1Drink1Feedback3" }, new Recipy.DrinkName[] { Recipy.DrinkName.ShootingStar, Recipy.DrinkName.Fruitmoon, Recipy.DrinkName.Moonshine }, new int[] { 1, 1, 1, 1, 1, 1, 1, 1 }));
        day2Nodes.Add(new DialogueNode("Day2-2", 1));
        //Kaline Exits, April Enters Drink2
        day2Nodes.Add(new DialogueNode("Day2-3", new int[] { 0, 3 }, new int[] { 0, 1 }, 1));
        day2Nodes.Add(new DialogueNode(new string[] { "Day2-3Drink2Feedback1", "Day2-3Drink2Feedback2", "Day2-3Drink1Feedback2", "Day2-3Drink2Feedback2", "Day2-3Drink2Feedback3", "Day2-3Drink2Feedback3", "Day2-3Drink2Feedback3", "Day2-3Drink2Feedback4", "Day2-3Drink2Feedback3" }, new Recipy.DrinkName[] { Recipy.DrinkName.Midnightmoon, Recipy.DrinkName.ShootingStar, Recipy.DrinkName.Bloodmoon }, new int[] { 1, 1, 1, 1, 1, 1, 1, 1 }));
        //Drink3
        day2Nodes.Add(new DialogueNode("Day2-4", 1));
        day2Nodes.Add(new DialogueNode(new string[] { "Day2-4Drink3Feedback1", "Day2-4Drink3Feedback2", "Day2-4Drink3Feedback2", "Day2-4Drink3Feedback2", "Day2-4Drink3Feedback3", "Day2-4Drink3Feedback3", "Day2-4Drink3Feedback3", "Day2-4Drink3Feedback4", "Day2-4Drink3Feedback3" }, new Recipy.DrinkName[] { Recipy.DrinkName.ShotOfCosmicLiquer, Recipy.DrinkName.GlassFromTheUnderWorld }, new int[] { 1, 1, 1, 1, 1, 1, 1, 1 }));
        //Drink4
        day2Nodes.Add(new DialogueNode("Day2-5", 1));
        day2Nodes.Add(new DialogueNode(new string[] { "Day2-5Drink4Feedback1", "Day2-5Drink4Feedback2", "Day2-5Drink4Feedback2", "Day2-5Drink4Feedback2", "Day2-5Drink4Feedback3", "Day2-5Drink4Feedback3", "Day2-5Drink4Feedback3", "Day2-5Drink4Feedback4", "Day2-5Drink4Feedback3" }, new Recipy.DrinkName[] { Recipy.DrinkName.WaterCyclone, Recipy.DrinkName.VoidOrangeJuice, Recipy.DrinkName.MoonberryJuice }, new int[] { 1, 1, 1, 1, 1, 1, 1, 1 }));
        day2Nodes.Add(new DialogueNode("Day2-6", 1));
        //April Exits, Darren enters Drink 5
        day2Nodes.Add(new DialogueNode("Day2-7", new int[] { 3, 4 }, new int[] { 0, 1 }, 1));
        day2Nodes.Add(new DialogueNode(new string[] { "Day2-7Drink5Feedback1", "Day2-7Drink5Feedback2", "Day2-7Drink5Feedback2", "Day2-7Drink5Feedback2", "Day2-7Drink5Feedback3", "Day2-7Drink5Feedback3", "Day2-7Drink5Feedback3", "Day2-7Drink5Feedback4", "Day2-7Drink5Feedback3" }, new Recipy.DrinkName[] { Recipy.DrinkName.SexOnTheMoon, Recipy.DrinkName.Fruitmoon, Recipy.DrinkName.ShootingStar }, new int[] { 1, 1, 1, 1, 1, 1, 1, 1 }));
        //Drink6
        day2Nodes.Add(new DialogueNode("Day2-8", 1));
        day2Nodes.Add(new DialogueNode(new string[] { "Day2-8Drink6Correct", "Day2-8Drink6Correct", "Day2-8Drink6Correct", "Day2-8Drink6Correct", "Day2-8Drink6Correct", "Day2-8Drink6Correct", "Day2-8Drink6Correct", "Day2-8Drink6DCorrect", "Day2-8Drink6DCorrect" }, new Recipy.DrinkName[] { Recipy.DrinkName.SexOnTheMoon, Recipy.DrinkName.Fruitmoon, Recipy.DrinkName.ShootingStar }, new int[] { 1, 1, 1, 1, 1, 1, 1, 1 }));
        day2Nodes.Add(new DialogueNode("Day2-9", 1));
        //Darren Exits
        day2Nodes.Add(new DialogueNode("Day2-10", new int[] { 4 }, new int[] { 0 }, 1));

        //end of day
        day2Nodes.Add(new DialogueNode("endofday", 1));
    }

    private void Day2()
    {
        Day2ListInit();
        SetCharacterPosition(0, 1);
        count = LoadTextSequence("Day2-1", true);
        currentDayNodes = day2Nodes;
    }
    #endregion

    #region Day3 definition  

    private void Day3ListInit()
    {

        //Lawrence enters, Drink 1
        day3Nodes.Add(new DialogueNode("Day3-2", new int[] { 5 }, new int[] { 1 } , 1));
        day3Nodes.Add(new DialogueNode(new string[] { "Day3-2Drink1Feedback1", "Day3-2Drink1Feedback2", "Day3-2Drink1Feedback2", "Day3-2Drink1Feedback2", "Day3-2Drink1Feedback3", "Day3-2Drink1Feedback3", "Day3-2Drink1Feedback4", "Day3-2Drink1Feedback3" }, new Recipy.DrinkName[] { Recipy.DrinkName.ShotOfCosmicLiquer, Recipy.DrinkName.VoidOrangeJuice, Recipy.DrinkName.MoonberryJuice, Recipy.DrinkName.WaterCyclone, Recipy.DrinkName.GlassFromTheUnderWorld }, new int[] { 1, 1, 1, 1, 1, 1, 1, 1 }));
        //Drink2
        day3Nodes.Add(new DialogueNode("Day3-3Drink2", 1));
        day3Nodes.Add(new DialogueNode(new string[] { "Day3-3Drink2Correct", "Day3-3Drink2Correct", "Day3-3Drink2Correct", "Day3-3Drink2Correct", "Day3-3Drink2Correct", "Day3-3Drink2Correct", "Day3-3Drink2DCorrect", "Day3-3Drink2DCorrect" }, new Recipy.DrinkName[] { Recipy.DrinkName.VoidsRevenge, Recipy.DrinkName.GlassFromTheUnderWorld, Recipy.DrinkName.ShotOfCosmicLiquer, Recipy.DrinkName.SexOnTheMoon, Recipy.DrinkName.Moonshine }, new int[] { 1, 1, 1, 1, 1, 1, 1, 1 }));
        day3Nodes.Add(new DialogueNode("Day3-4", 1));

        //Lawrence Leaves, Meryl Enters Drink3
        day3Nodes.Add(new DialogueNode("Day3-5", new int[] { 5, 6 }, new int[] { 0, 1 }, 1));
        day3Nodes.Add(new DialogueNode(new string[] { "Day3-5Drink3Feedback1", "Day3-5Drink1Feedback2", "Day3-5Drink3Feedback2", "Day3-5Drink3Feedback2", "Day3-5Drink3Feedback3", "Day3-5Drink3Feedback3", "Day3-5Drink3Feedback4", "Day3-5Drink3Feedback3" }, new Recipy.DrinkName[] { Recipy.DrinkName.MoonberryJuice, Recipy.DrinkName.VoidOrangeJuice }, new int[] { 1, 1, 1, 1, 1, 1, 1, 1 }));

        //Drink4
        day3Nodes.Add(new DialogueNode("Day3-6", 1));
        day3Nodes.Add(new DialogueNode(new string[] { "Day3-6Drink4Correct", "Day3-6Drink4Correct", "Day3-6Drink4Correct", "Day3-6Drink4Correct", "Day3-6Drink4Correct", "Day3-6Drink4Correct", "Day3-6Drink4DCorrect", "Day3-6Drink4DCorrect" }, new Recipy.DrinkName[] { Recipy.DrinkName.Fruitmoon, Recipy.DrinkName.Bloodmoon, Recipy.DrinkName.VoidOrangeJuice, Recipy.DrinkName.MoonberryJuice }, new int[] { 1, 1, 1, 1, 1, 1, 1, 1 }));
        day3Nodes.Add(new DialogueNode("Day3-7", 1));
        //Meryl Leaves, Fatih enters
        day3Nodes.Add(new DialogueNode("Day3-8", new int[] { 6, 7 }, new int[] { 0, 1 }, 1));
        //Drink4 2, error in file naming but still unique names
        day3Nodes.Add(new DialogueNode("Day3-9", 1));
        day3Nodes.Add(new DialogueNode(new string[] { "Day3-9Drink4Feedback1", "Day3-9Drink4Feedback2", "Day3-9Drink4Feedback2", "Day3-9Drink4Feedback2", "Day3-9Drink4Feedback3", "Day3-9Drink4Feedback3", "Day3-9Drink4Feedback4", "Day3-9Drink4Feedback3" }, new Recipy.DrinkName[] { Recipy.DrinkName.WaterCyclone, Recipy.DrinkName.VoidOrangeJuice, Recipy.DrinkName.MoonberryJuice }, new int[] { 1, 1, 1, 1, 1, 1, 1, 1 }));
        //Drink5
        day3Nodes.Add(new DialogueNode("Day3-10", 1));
        day3Nodes.Add(new DialogueNode(new string[] { "Day3-10Correct", "Day3-10Correct", "Day3-10Correct", "Day3-10Correct", "Day3-10Correct", "Day3-10Correct", "Day3-10DCorrect", "Day3-10DCorrect" }, new Recipy.DrinkName[] { Recipy.DrinkName.Fruitmoon }, new int[] { 1, 1, 1, 1, 1, 1, 1, 1 }));
        day3Nodes.Add(new DialogueNode("Day3-11", 1));
        //Fatih Leaves
        day3Nodes.Add(new DialogueNode("Day3-12", new int[] { 7 }, new int[] { 0 }, 1));

        //end of day
        day3Nodes.Add(new DialogueNode("endofday", 1));
    }

    private void Day3()
    {
        Day3ListInit();
        SetCharacterPosition(0, 0);
        count = LoadTextSequence("Day3-1", true);
        currentDayNodes = day3Nodes;
    }
    #endregion

    #region Day4 definition  

    private void Day4ListInit()
    {
        //C+S enter
        day4Nodes.Add(new DialogueNode("FDay2", new int[] { 2 } , new int[] { 2 }, 1));     //K
        day4Nodes.Add(new DialogueNode("FDay3", 1));    //C+S
        //Drink1
        day4Nodes.Add(new DialogueNode("FDay4", 1));    //C+S
        day4Nodes.Add(new DialogueNode(new string[] { "FDay4Drink1Feedback1", "FDay4Drink1Feedback2", "FDay4Drink1Feedback2", "FDay4Drink1Feedback2", "FDay4Drink1Feedback3", "FDay4Drink1Feedback3", "FDay4Drink1Feedback4", "FDay4Drink1Feedback3" }, new Recipy.DrinkName[] { Recipy.DrinkName.Midnightmoon, Recipy.DrinkName.Fruitmoon, Recipy.DrinkName.ShootingStar }, new int[] { 1, 1, 1, 1, 1, 1, 1, 1 }));
        //Drink2
        day4Nodes.Add(new DialogueNode("FDay5", 1));    //C+S
        day4Nodes.Add(new DialogueNode(new string[] { "FDay5Drink2Feedback1", "FDay5Drink2Feedback2", "FDay5Drink2Feedback2", "FDay5Drink2Feedback2", "FDay5Drink2Feedback3", "FDay5Drink2Feedback3", "FDay5Drink2Feedback4", "FDay5Drink2Feedback3" }, new Recipy.DrinkName[] { Recipy.DrinkName.Midnightmoon, Recipy.DrinkName.Fruitmoon, Recipy.DrinkName.ShootingStar }, new int[] { 1, 1, 1, 1, 1, 1, 1, 1 }));
        //Drink3
        day4Nodes.Add(new DialogueNode("FDay6", 1));    //K
        day4Nodes.Add(new DialogueNode("FDay7", 1));    //C+S
        day4Nodes.Add(new DialogueNode("FDay8", 1));    //K
        day4Nodes.Add(new DialogueNode("FDay9", 1));    //C+S
        day4Nodes.Add(new DialogueNode("FDay10", 1));    //K
        day4Nodes.Add(new DialogueNode(new string[] { "FDay10Drink3Feedback1", "FDay10Drink3Feedback2", "FDay10Drink3Feedback2", "FDay10Drink3Feedback2", "FDay10Drink3Feedback3", "FDay10Drink3Feedback3", "FDay10Drink3Feedback4", "FDay10Drink3Feedback3" }, new Recipy.DrinkName[] { Recipy.DrinkName.Fruitmoon, Recipy.DrinkName.VoidOrangeJuice, Recipy.DrinkName.MoonberryJuice }, new int[] { 1, 1, 1, 1, 1, 1, 1, 1 }));
        //Drink4
        day4Nodes.Add(new DialogueNode("FDay11", 1));    //K
        day4Nodes.Add(new DialogueNode("FDay12", 1));    //C+S
        day4Nodes.Add(new DialogueNode("FDay13", 1));    //K
        day4Nodes.Add(new DialogueNode("FDay14", 1));    //C+S
        day4Nodes.Add(new DialogueNode("FDay15", 1));    //K
        day4Nodes.Add(new DialogueNode("FDay16", 1));    //C+S
        day4Nodes.Add(new DialogueNode("FDay17", new int[] { 2 }, new int[] { 0 }, 1));    //K, C+S Leave
        day4Nodes.Add(new DialogueNode("FDay18", new int[] { 0,8 }, new int[] { 0,1 }, 1));    //Laika Enters, K leaves
        day4Nodes.Add(new DialogueNode("FDay19", 1));    
        day4Nodes.Add(new DialogueNode("FDay20", 1));    
        day4Nodes.Add(new DialogueNode("FDay21", 1));
        day4Nodes.Add(new DialogueNode(new string[] { "FDay21Drink4Feedback1", "FDay21Drink4Feedback2", "FDay21Drink4Feedback2", "FDay21Drink4Feedback2", "FDay21Drink4Feedback3", "FDay21Drink4Feedback3", "FDay21Drink4Feedback4", "FDay21Drink4Feedback3" }, new Recipy.DrinkName[] { Recipy.DrinkName.WaterCyclone, Recipy.DrinkName.VoidOrangeJuice, Recipy.DrinkName.MoonberryJuice }, new int[] { 1, 1, 1, 1, 1, 1, 1, 1 }));
        //Drink5
        day4Nodes.Add(new DialogueNode("FDay22", 1));
        day4Nodes.Add(new DialogueNode("FDay23", 1));
        day4Nodes.Add(new DialogueNode("FDay24", 1));
        day4Nodes.Add(new DialogueNode(new string[] { "FDay24Drink5Feedback1", "FDay24Drink5Feedback2", "FDay24Drink5Feedback2", "FDay24Drink5Feedback2", "FDay24Drink5Feedback3", "FDay24Drink5Feedback3", "FDay24Drink5Feedback4", "FDay24Drink5Feedback3" }, new Recipy.DrinkName[] { Recipy.DrinkName.WaterCyclone, Recipy.DrinkName.VoidOrangeJuice, Recipy.DrinkName.MoonberryJuice }, new int[] { 1, 1, 1, 1, 1, 1, 1, 1 }));
        //Drink6
        day4Nodes.Add(new DialogueNode("FDay25", 1));
        day4Nodes.Add(new DialogueNode("FDay26", new int[] { 8, 0 }, new int[] { 3, 2 }, 1)); //Laika makes room, K enters
        day4Nodes.Add(new DialogueNode("FDay27", 1)); //@
        day4Nodes.Add(new DialogueNode("FDay28", 1)); //Laika
        day4Nodes.Add(new DialogueNode("FDay29", new int[] { 0, 8 }, new int[] { 0, 0 }, 1)); //Laika and K leave @ talking
        day4Nodes.Add(new DialogueNode("FDay30", new int[] { 3, 5 }, new int[] { 2, 3 }, 1)); //Laurie and April Enter, L talking
        day4Nodes.Add(new DialogueNode("FDay31", 1)); //A
        day4Nodes.Add(new DialogueNode("FDay32", 1)); //L
        day4Nodes.Add(new DialogueNode("FDay33", 1)); //A
        day4Nodes.Add(new DialogueNode(new string[] { "FDay33Drink6Feedback1", "FDay33Drink6Feedback2", "FDay33Drink6Feedback2", "FDay33Drink6Feedback2", "FDay33Drink6Feedback3", "FDay33Drink6Feedback3", "FDay33Drink6Feedback4", "FDay33Drink6Feedback3" }, new Recipy.DrinkName[] { Recipy.DrinkName.Moonshine, Recipy.DrinkName.Midnightmoon, Recipy.DrinkName.ShootingStar, Recipy.DrinkName.SexOnTheMoon }, new int[] { 1, 1, 1, 1, 1, 1, 1, 1 })); //A
        //Drink7                                                                                                                                                                                                                                                                                                                                                                                                                                                    
        day4Nodes.Add(new DialogueNode("FDay34", 1)); //A
        day4Nodes.Add(new DialogueNode("FDay35", 1)); //L
        day4Nodes.Add(new DialogueNode(new string[] { "FDay33Drink6Feedback1", "FDay33Drink6Feedback2", "FDay33Drink6Feedback2", "FDay33Drink6Feedback2", "FDay33Drink6Feedback3", "FDay33Drink6Feedback3", "FDay33Drink6Feedback4", "FDay33Drink6Feedback3" }, new Recipy.DrinkName[] { Recipy.DrinkName.Bloodmoon, Recipy.DrinkName.Fruitmoon, Recipy.DrinkName.ShotOfCosmicLiquer, Recipy.DrinkName.SexOnTheMoon }, new int[] { 1, 1, 1, 1, 1, 1, 1, 1 })); //L
        //Drink8
        day4Nodes.Add(new DialogueNode("FDay36", 1)); //L
        day4Nodes.Add(new DialogueNode("FDay37", 1)); //A
        day4Nodes.Add(new DialogueNode("FDay38", 1)); //L
        day4Nodes.Add(new DialogueNode("FDay39", 1)); //A
        day4Nodes.Add(new DialogueNode("FDay40", 1)); //L
        day4Nodes.Add(new DialogueNode("FDay41", 1)); //A
        day4Nodes.Add(new DialogueNode("FDay42", 1)); //L
        day4Nodes.Add(new DialogueNode("FDay43", 1)); //A
        day4Nodes.Add(new DialogueNode(new string[] { "FDay43Drink8Correct", "FDay43Drink8Correct", "FDay43Drink8Correct", "FDay43Drink8Correct", "FDay43Drink8Correct", "FDay43Drink8Correct", "FDay43Drink8DCorrect", "FDay43Drink8DCorrect" }, new Recipy.DrinkName[] { Recipy.DrinkName.Midnightmoon, Recipy.DrinkName.ShootingStar, Recipy.DrinkName.SexOnTheMoon }, new int[] { 1, 1, 1, 1, 1, 1, 1, 1 })); //A
        //Drink9
        day4Nodes.Add(new DialogueNode("FDay44", 1)); //L
        day4Nodes.Add(new DialogueNode(new string[] { "FDay44Drink9Feedback1", "FDay44Drink9Feedback2", "FDay44Drink9Feedback2", "FDay44Drink9Feedback2", "FDay44Drink9Feedback3", "FDay44Drink9Feedback3", "FDay44Drink9Feedback4", "FDay44Drink9Feedback3" }, new Recipy.DrinkName[] { Recipy.DrinkName.VoidOrangeJuice, Recipy.DrinkName.MoonberryJuice }, new int[] { 1, 1, 1, 1, 1, 1, 1, 1 })); //L
        //Drink10
        day4Nodes.Add(new DialogueNode("FDay45", 1)); //L
        day4Nodes.Add(new DialogueNode("FDay46", 1)); //A
        //day4Nodes.Add(new DialogueNode("FDay47", 1)); //empty file due to error in file creation
        day4Nodes.Add(new DialogueNode("FDay48", 1)); //L
        day4Nodes.Add(new DialogueNode("FDay49", 1)); //A
        day4Nodes.Add(new DialogueNode("FDay50", new int[] { 3, 5, 9 }, new int[] { 0, 0, 1 }, 1)); //L + A leave, Chester enters
        day4Nodes.Add(new DialogueNode("FDay51", 1)); //@
        day4Nodes.Add(new DialogueNode("FDay52", 1)); //C
        day4Nodes.Add(new DialogueNode(new string[] { "FDay52Drink10Feedback1", "FDay52Drink10Feedback2", "FDay52Drink10Feedback2", "FDay52Drink10Feedback2", "FDay52Drink10Feedback3", "FDay52Drink10Feedback3", "FDay52Drink10Feedback4", "FDay52Drink10Feedback3" }, new Recipy.DrinkName[] { Recipy.DrinkName.GlassFromTheUnderWorld, Recipy.DrinkName.VoidsRevenge }, new int[] { 1, 1, 1, 1, 1, 1, 1, 1 })); //C
        day4Nodes.Add(new DialogueNode("FDay53", 1)); //C
        day4Nodes.Add(new DialogueNode("FDay54", 1)); //@
        day4Nodes.Add(new DialogueNode("FDay55", 1)); //C
        day4Nodes.Add(new DialogueNode("FDay56", new int[] { 9 }, new int[] { 0 }, 1)); //Chester Leaves, @
        //Drink11
        day4Nodes.Add(new DialogueNode("FDay57", new int[] { 10 }, new int[] { 1 }, 1)); //Eye enters
        day4Nodes.Add(new DialogueNode(new string[] { "FDay57Drink11Feedback1", "FDay57Drink11Feedback2", "FDay57Drink11Feedback2", "FDay57Drink11Feedback2", "FDay57Drink11Feedback3", "FDay57Drink11Feedback3", "FDay57Drink11Feedback4", "FDay57Drink11Feedback3" }, new Recipy.DrinkName[] { Recipy.DrinkName.SexOnTheMoon }, new int[] { 1, 1, 1, 1, 1, 1, 1, 1 })); //E
        //Drink12
        day4Nodes.Add(new DialogueNode("FDay58", 1)); //E
        day4Nodes.Add(new DialogueNode(new string[] { "FDay58Drink12Feedback1", "FDay58Drink12Feedback2", "FDay58Drink12Feedback2", "FDay58Drink12Feedback2", "FDay58Drink12Feedback3", "FDay58Drink12Feedback3", "FDay58Drink12Feedback4", "FDay58Drink12Feedback3" }, new Recipy.DrinkName[] { Recipy.DrinkName.VoidOrangeJuice }, new int[] { 1, 1, 1, 1, 1, 1, 1, 1 })); //E
        //Drink13
        day4Nodes.Add(new DialogueNode("FDay59", 1)); //E
        day4Nodes.Add(new DialogueNode(new string[] { "FDay59Drink13Correct", "FDay59Drink13Correct", "FDay59Drink13Correct", "FDay59Drink13DCorrect", "FDay59Drink13DCorrect", "FDay59Drink13DCorrect", "FDay59Drink13DCorrect", "FDay59Drink13DCorrect" }, new Recipy.DrinkName[] { Recipy.DrinkName.VoidsRevenge, Recipy.DrinkName.GlassFromTheUnderWorld }, new int[] { 1, 1, 1, 1, 1, 1, 1, 1 })); //E
        day4Nodes.Add(new DialogueNode("FDay60", 1)); //E
        day4Nodes.Add(new DialogueNode("FDay61", new int[] { 10 }, new int[] { 4 }, 1)); //@
        day4Nodes.Add(new DialogueNode("FDay62", new int[] { 0 }, new int[] { 1 }, 1)); //K


        //end of day
        day4Nodes.Add(new DialogueNode("endofday", 1));
    }

    private void Day4()
    {
        Day4ListInit();
        SetCharacterPosition(0, 3);
        count = LoadTextSequence("FDay1", true);
        currentDayNodes = new List<DialogueNode>();
        currentDayNodes = day4Nodes;
    }
    #endregion








}
