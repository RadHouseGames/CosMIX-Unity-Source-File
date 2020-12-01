using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FruitStack : MonoBehaviour
{
    [Header("Refs")]
    public DrinkManager drinkManager;
    public List<Image> stackPoints = new List<Image>();

    [Header("Ingredients Sprites")]
    public Sprite blank;
    public Sprite moonpotato;
    public Sprite blazer;
    public Sprite redmoonberry;
    public Sprite voidorange;
    public Sprite cosmicliqure;
    public Sprite spacelemon;
    public Sprite stardirt;
    public Sprite chemibloom;
    public Sprite infernalwine;

    public void Start()
    {
        List<Image> images = new List<Image>();
        foreach(Image image in stackPoints)
        {
            images.Insert(0, image);
        }
        stackPoints = images;
    }
    public void Update()
    {
        int count = 0;
        foreach(Image image in stackPoints)
        {
            image.sprite = blank;
        }
        foreach (KeyValuePair<Recipy.Ingredient, int> ingPair in drinkManager.GetCurrentMix())
        {
            for(int i = 1; i <= ingPair.Value; i++)
            {
                stackPoints[count].sprite = GetSprite(ingPair.Key);
                count += 1;
                if (count >= stackPoints.Count)
                {
                    return;
                }                
            }
        }
    }

    private Sprite GetSprite(Recipy.Ingredient ingredient)
    {
        switch (ingredient)
        {
            case Recipy.Ingredient.MoonPotato:
                return moonpotato;
            case Recipy.Ingredient.BlazerIce:
                return blazer;
            case Recipy.Ingredient.RedMoonberry:
                return redmoonberry;
            case Recipy.Ingredient.VoidOrange:
                return voidorange;
            case Recipy.Ingredient.CosmicLiqueur:
                return cosmicliqure;
            case Recipy.Ingredient.SpaceLemon:
                return spacelemon;
            case Recipy.Ingredient.StarDirt:
                return stardirt;
            case Recipy.Ingredient.ChemiBloom:
                return chemibloom;
            case Recipy.Ingredient.InfernalWine:
                return infernalwine;
        }
        return null;
    }
}
