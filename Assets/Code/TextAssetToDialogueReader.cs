using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class TextAssetToDialogueReader
{
    private static TextAsset dialogueTextAsset;
    private static char[] textBoxDelimiter = new char[] { '\r', '\n' };
    private static string[] textBoxes;
    private static Queue<string> formattedTextBoxes;

    private static TextBoxDialogue dialoguePrinter;
    private static CharacterMangerScript chManager;
    private static GameObject obj;

    private static string _charactername;
    public static string characterName
    {
        get
        {
            return _charactername;
        }
        set
        {
            _charactername = value;
        }
    }

    public static int dayNumber
    {
        get
        {
            return dayNumber;
        }
        set
        {
            dayNumber = value;
            
        }
    }
    



    private static void LoadText(string assetName)
    {
        dialogueTextAsset = Resources.Load(assetName) as TextAsset;
        dialogueTextAsset.text.Replace("\r\n", "\n").Replace("\r", "\n");
        textBoxes = dialogueTextAsset.text.Split(textBoxDelimiter, StringSplitOptions.RemoveEmptyEntries);
        Debug.Log("Reading File: " + assetName);
    }

    private static void FormatText()
    {
        formattedTextBoxes = new Queue<string>();
        foreach (string textBox in textBoxes)
        {
            formattedTextBoxes.Enqueue(textBox);
        }
    }

    public static Queue<string> ReadText(string assetName)
    {
        LoadText(assetName);

        FormatText();

        return formattedTextBoxes;
    }
}
