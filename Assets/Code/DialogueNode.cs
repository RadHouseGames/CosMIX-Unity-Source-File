using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueNode 
{
    public bool hasDrink, movesChar;
    public Recipy.DrinkName[] likedDrinks;
    public string[] nextLinks;
    public int[] indexShift;
    public int[] character, targetAnchor;

    //Note Various constructors exist to allow variety of configuration of nodes for timesaving in certain edge cases



    public DialogueNode(string next, int shift)
    {
        hasDrink = false;
        movesChar = false;
        nextLinks = new string[] { next };
        indexShift = new int[] { shift };
    }

    public DialogueNode(string next, int[] ch, int[] target, int shift)
    {
        hasDrink = false;
        movesChar = true;
        nextLinks = new string[] { next };
        character = ch;
        targetAnchor = target;
        indexShift = new int[] { shift };
    }


    public DialogueNode(string[] nextList, Recipy.DrinkName[] drinkList, int[] shiftList)
    {
        hasDrink = true;
        movesChar = false;
        likedDrinks = drinkList;
        nextLinks = nextList;
        indexShift = shiftList;
    }

    public DialogueNode(string[] nextList, Recipy.DrinkName[] drinkList, int[] ch, int[] target, int[] shiftList)
    {
        hasDrink = true;
        movesChar = true;
        likedDrinks = drinkList;
        nextLinks = nextList;
        character = ch;
        targetAnchor = target;
        indexShift = shiftList;
    }
}
