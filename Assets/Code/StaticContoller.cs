using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticContoller
{
    public static string ingredientSeletect = null;

    public static Recipy.Ingredient GetIngredientEnum(string ingredient)
    {
        switch (ingredient)
        {
            case "Moon Potato":
                return Recipy.Ingredient.MoonPotato;
            case "Blazar Ice":
                return Recipy.Ingredient.BlazerIce;
            case "Red Moonberry":
                return Recipy.Ingredient.RedMoonberry;
            case "Void Orange":
                return Recipy.Ingredient.VoidOrange;
            case "Cosmic Liqueur":
                return Recipy.Ingredient.CosmicLiqueur;
            case "Space Lemon":
                return Recipy.Ingredient.SpaceLemon;
            case "Star Dirt":
                return Recipy.Ingredient.StarDirt;
            case "Chemi Bloom":
                return Recipy.Ingredient.ChemiBloom;
            case "Infernal Wine":
                return Recipy.Ingredient.InfernalWine;
        }
        return Recipy.Ingredient.NULL;
    }


}
