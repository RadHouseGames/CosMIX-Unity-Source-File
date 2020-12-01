using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//this class will manage all of the vents on screen for mixing and creating drinks

public class DrinkManager : MonoBehaviour
{
    [Header("Object Lists")]
    public List<Transform> shakeBack = new List<Transform>();
    public Dictionary<Transform, Vector3> startPositions = new Dictionary<Transform, Vector3>();

    [Header("Object References")]
    public UnityEngine.UI.Image shakeFillBar;
    public Button diliverObject;
    public TextMeshProUGUI moneyText;
    public GameObject charManagerObj;
    private CharacterMangerScript charManagerComponent;

    [Header("Screens & Popups")]
    public List<GameObject> screensRaw = new List<GameObject>();
    private Dictionary<string, GameObject> screens = new Dictionary<string, GameObject>();

    private List<Recipy> drinkRecipys = new List<Recipy>();//list of possable drinks to make
    private Dictionary<Recipy.Ingredient, int> prices = new Dictionary<Recipy.Ingredient, int>();

    [Header("Recipy Book")]
    public List<GameObject> recipyPages = new List<GameObject>();
    private int recipyIndex = 0;

    [Header("Shake Screen")]
    public Animator shakeCan;

    [Header("Result Objs")]
    public Animator resutsAnimator;
    public UnityEngine.UI.Image resultsImage;

    [Header("Drink Images")]
    public Sprite moonshine;
    public Sprite bloodmoon;
    public Sprite fruitmoon;
    public Sprite midnightmoon;
    public Sprite sexonthemoon;
    public Sprite shootingstar;
    public Sprite voidsrevenge;
    public Sprite voidorangejuice;
    public Sprite moonberryjuice;
    public Sprite shotofcosmicliqure;
    public Sprite glassferomtheunderworld;
    public Sprite watercyclone;

    private int money = 40;

    //preserve details
    private Dictionary<Recipy.Ingredient, int> currentMix = new Dictionary<Recipy.Ingredient, int>();//the current pool
    private int shake = 0;

    //store the result here
    private Recipy.DrinkName drinkName = Recipy.DrinkName.NULL;
    private Recipy.DrinkQuality drinkQuality = Recipy.DrinkQuality.NULL;
       
    public Dictionary<Recipy.Ingredient, int> GetCurrentMix() { return currentMix; }
    private bool redline;
    private bool recipyOpen;
    private bool shakeOpen;

    public void Start()
    {
        //set start positions
        foreach(Transform transform in shakeBack)
        {
            startPositions.Add(transform, transform.position);
        }
        //Get charManagerComponent
        charManagerComponent = charManagerObj.GetComponent<CharacterMangerScript>();


        InvokeRepeating("MoneyUpdate", 1, 1);
        //convert the screens raw into a dictioary
        foreach (GameObject obj in screensRaw)
        {
            screens.Add(obj.name, obj);
        }

        //set recipy book to good
        foreach (GameObject page in recipyPages)
        {
            page.SetActive(false);
        }
        recipyPages[recipyIndex].SetActive(true);

        //populate the recipys list
        Dictionary<Recipy.Ingredient, int> ingredients = new Dictionary<Recipy.Ingredient, int>();
        ingredients.Add(Recipy.Ingredient.MoonPotato, 1);
        ingredients.Add(Recipy.Ingredient.BlazerIce, 2);
        ingredients.Add(Recipy.Ingredient.CosmicLiqueur, 2);
        drinkRecipys.Add(new Recipy(Recipy.DrinkName.Moonshine, ingredients, 50));

        ingredients = new Dictionary<Recipy.Ingredient, int>();
        ingredients.Add(Recipy.Ingredient.RedMoonberry, 2);
        ingredients.Add(Recipy.Ingredient.VoidOrange, 1);
        ingredients.Add(Recipy.Ingredient.BlazerIce, 1);
        ingredients.Add(Recipy.Ingredient.CosmicLiqueur, 1);
        drinkRecipys.Add(new Recipy(Recipy.DrinkName.Bloodmoon, ingredients, 70));

        ingredients = new Dictionary<Recipy.Ingredient, int>();
        ingredients.Add(Recipy.Ingredient.VoidOrange, 1);
        ingredients.Add(Recipy.Ingredient.RedMoonberry, 1);
        ingredients.Add(Recipy.Ingredient.SpaceLemon, 1);
        ingredients.Add(Recipy.Ingredient.BlazerIce, 1);
        ingredients.Add(Recipy.Ingredient.CosmicLiqueur, 1);
        drinkRecipys.Add(new Recipy(Recipy.DrinkName.Fruitmoon, ingredients, 80));

        ingredients = new Dictionary<Recipy.Ingredient, int>();
        ingredients.Add(Recipy.Ingredient.StarDirt, 2);
        ingredients.Add(Recipy.Ingredient.BlazerIce, 1);
        ingredients.Add(Recipy.Ingredient.CosmicLiqueur, 1);
        drinkRecipys.Add(new Recipy(Recipy.DrinkName.Midnightmoon, ingredients, 50));

        ingredients = new Dictionary<Recipy.Ingredient, int>();
        ingredients.Add(Recipy.Ingredient.CosmicLiqueur, 3);
        ingredients.Add(Recipy.Ingredient.ChemiBloom, 1);
        drinkRecipys.Add(new Recipy(Recipy.DrinkName.SexOnTheMoon, ingredients, 30));

        ingredients = new Dictionary<Recipy.Ingredient, int>();
        ingredients.Add(Recipy.Ingredient.CosmicLiqueur, 2);
        ingredients.Add(Recipy.Ingredient.BlazerIce, 2);
        ingredients.Add(Recipy.Ingredient.StarDirt, 1);
        drinkRecipys.Add(new Recipy(Recipy.DrinkName.ShootingStar, ingredients, 50));

        ingredients = new Dictionary<Recipy.Ingredient, int>();
        ingredients.Add(Recipy.Ingredient.StarDirt, 1);
        ingredients.Add(Recipy.Ingredient.VoidOrange, 2);
        ingredients.Add(Recipy.Ingredient.InfernalWine, 1);
        drinkRecipys.Add(new Recipy(Recipy.DrinkName.VoidsRevenge, ingredients, 20));

        ingredients = new Dictionary<Recipy.Ingredient, int>();
        ingredients.Add(Recipy.Ingredient.VoidOrange, 1);
        drinkRecipys.Add(new Recipy(Recipy.DrinkName.VoidOrangeJuice, ingredients, 20));

        ingredients = new Dictionary<Recipy.Ingredient, int>();
        ingredients.Add(Recipy.Ingredient.RedMoonberry, 1);
        drinkRecipys.Add(new Recipy(Recipy.DrinkName.MoonberryJuice, ingredients, 20));

        ingredients = new Dictionary<Recipy.Ingredient, int>();
        ingredients.Add(Recipy.Ingredient.CosmicLiqueur, 1);
        drinkRecipys.Add(new Recipy(Recipy.DrinkName.ShotOfCosmicLiquer, ingredients, 0));

        ingredients = new Dictionary<Recipy.Ingredient, int>();
        ingredients.Add(Recipy.Ingredient.InfernalWine, 1);
        drinkRecipys.Add(new Recipy(Recipy.DrinkName.GlassFromTheUnderWorld, ingredients, 0));

        ingredients = new Dictionary<Recipy.Ingredient, int>();
        ingredients.Add(Recipy.Ingredient.BlazerIce, 1);
        drinkRecipys.Add(new Recipy(Recipy.DrinkName.WaterCyclone, ingredients, 20));

        //set teh prices for ingredients
        prices.Add(Recipy.Ingredient.MoonPotato, 3);
        prices.Add(Recipy.Ingredient.BlazerIce, 1);
        prices.Add(Recipy.Ingredient.RedMoonberry, 1);

        prices.Add(Recipy.Ingredient.VoidOrange, 2);
        prices.Add(Recipy.Ingredient.CosmicLiqueur, 3);

        prices.Add(Recipy.Ingredient.SpaceLemon, 2);
        prices.Add(Recipy.Ingredient.StarDirt, 10);

        prices.Add(Recipy.Ingredient.ChemiBloom, 3);
        prices.Add(Recipy.Ingredient.InfernalWine, 20);
    }
    
    private void ResetShakeBacks()
    {
        foreach(KeyValuePair<Transform, Vector3> pair in startPositions)
        {
            pair.Key.position = pair.Value;
        }
    }
    public void Update()
    {
        if (shake >= 100) { shake = 100; }
        shakeFillBar.fillAmount = (float)shake / 100;
        if (currentMix.Count <= 0) { diliverObject.interactable = false; } else { diliverObject.interactable = true; }
        moneyText.text = "£" + money.ToString();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OpenScreen("DrinkShelve");
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (recipyOpen)
            {
                OpenScreen("DrinkShelve");
                recipyOpen = false;
                shakeOpen = false;
            }
            else
            {
                OpenScreen("RecipyBook");
                recipyOpen = true;
                shakeOpen = false;
            }           
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (shakeOpen)
            {
                OpenScreen("DrinkShelve");
                shakeOpen = false;
                recipyOpen = false;
            }
            else
            {
                OpenScreen("ShakeMake");
                shakeOpen = true;
                recipyOpen = false;
            }
        }
        if (Input.GetKeyDown(KeyCode.KeypadEnter)|| Input.GetKeyDown(KeyCode.Return))
        {
            if (currentMix.Count <= 0) { return; }
            Serve();
        }
    }

    private void MoneyUpdate()
    {
        if (money <= 0)
        {
            if (redline)
            {
                moneyText.color = Color.red;
                redline = false;
            }
            else
            {
                moneyText.color = Color.white;
                redline = true;
            }
        }
    }



    public void Pay(int amount)
    {
        money += amount;
    }
    public void OpenScreen(string screenName)
    {
        if (!screens.ContainsKey(screenName)) { return; }
        foreach (KeyValuePair<string, GameObject> screen in screens)
        {
            screen.Value.SetActive(false);
        }
        screens["DrinkShelve"].SetActive(true);
        screens[screenName].SetActive(true);
        ResetShakeBacks();
    }
    public void AddIngredient(string ingredient)
    {
        shake = shake / 2;
        Recipy.Ingredient ingredientOut = Recipy.Ingredient.BlazerIce;
        switch (ingredient)
        {
            case "Moon Potato":
                ingredientOut = Recipy.Ingredient.MoonPotato;
                money -= prices[Recipy.Ingredient.MoonPotato];
                break;
            case "Blazar Ice":
                ingredientOut = Recipy.Ingredient.BlazerIce;
                money -= prices[Recipy.Ingredient.BlazerIce];
                break;
            case "Red Moonberry":
                ingredientOut = Recipy.Ingredient.RedMoonberry;
                money -= prices[Recipy.Ingredient.RedMoonberry];
                break;
            case "Void Orange":
                ingredientOut = Recipy.Ingredient.VoidOrange;
                money -= prices[Recipy.Ingredient.VoidOrange];
                break;
            case "Cosmic Liqueur":
                ingredientOut = Recipy.Ingredient.CosmicLiqueur;
                money -= prices[Recipy.Ingredient.CosmicLiqueur];
                break;
            case "Space Lemon":
                ingredientOut = Recipy.Ingredient.SpaceLemon;
                money -= prices[Recipy.Ingredient.SpaceLemon];
                break;
            case "Star Dirt":
                ingredientOut = Recipy.Ingredient.StarDirt;
                money -= prices[Recipy.Ingredient.StarDirt];
                break;
            case "Chemi Bloom":
                ingredientOut = Recipy.Ingredient.ChemiBloom;
                money -= prices[Recipy.Ingredient.ChemiBloom];
                break;
            case "Infernal Wine":
                ingredientOut = Recipy.Ingredient.InfernalWine;
                money -= prices[Recipy.Ingredient.InfernalWine];
                break;
        }
        if (currentMix.ContainsKey(ingredientOut))
        {
            currentMix[ingredientOut] += 1;
        }
        else
        {
            currentMix.Add(ingredientOut, 1);
        }
        Debug.Log("Added" + ingredientOut.ToString());
    }
    public void EmptyBin()
    {
        currentMix = new Dictionary<Recipy.Ingredient, int>();
        shake = 0;
    }
    public void Shake(int amount)
    {
        shake += amount;
        shakeCan.Play("Shake");
    }
    public void Serve()
    {
        if (drinkName != Recipy.DrinkName.NULL) { return; }

        drinkName = GetDrink();

        int desiredShake = GetShakeAmount(drinkName);
        if (shake == desiredShake)
        {
            drinkQuality = Recipy.DrinkQuality.Perfect;
        }
        else if (shake <= desiredShake + 10 && shake >= desiredShake - 10) 
        {
            drinkQuality = Recipy.DrinkQuality.Good;
        }
        else if (shake <= desiredShake + 20 && shake >= desiredShake - 20)
        {
            drinkQuality = Recipy.DrinkQuality.Bad;
        }
        else
        {
            drinkQuality = Recipy.DrinkQuality.Sludge;
        }

        //send the drink out
        resultsImage.sprite = GetSprite(drinkName);
        Debug.Log("DrinkName : " + drinkName.ToString());
        Debug.Log("SpriteName : " + GetSprite(drinkName).name);
        resutsAnimator.Play("Start");

        //send this to ryan
        charManagerComponent.ServeOrder(drinkName, drinkQuality);

        Debug.Log("Finished Drink : " + drinkQuality.ToString() + " " + drinkName.ToString());
        //reset teh current mix
        currentMix = new Dictionary<Recipy.Ingredient, int>();
        drinkName = Recipy.DrinkName.NULL;
        shake = 0;

        
    }
    private Sprite GetSprite(Recipy.DrinkName drinkName)
    {
        switch (drinkName)
        {
            case Recipy.DrinkName.Moonshine:
                return moonshine;
            case Recipy.DrinkName.Bloodmoon:
                return bloodmoon;
            case Recipy.DrinkName.Fruitmoon:
                return fruitmoon;
            case Recipy.DrinkName.Midnightmoon:
                return midnightmoon;
            case Recipy.DrinkName.SexOnTheMoon:
                return sexonthemoon;
            case Recipy.DrinkName.ShootingStar:
                return shootingstar;
            case Recipy.DrinkName.VoidsRevenge:
                return voidsrevenge;
            case Recipy.DrinkName.VoidOrangeJuice:
                return voidorangejuice;
            case Recipy.DrinkName.MoonberryJuice:
                return moonberryjuice;
            case Recipy.DrinkName.ShotOfCosmicLiquer:
                return shotofcosmicliqure;
            case Recipy.DrinkName.GlassFromTheUnderWorld:
                return glassferomtheunderworld;
            case Recipy.DrinkName.WaterCyclone:
                return watercyclone;
        }
        return moonshine;
    }
    private Recipy.DrinkName GetDrink()
    {
        foreach (Recipy recipy in drinkRecipys)
        {
            int success = 0;
            int reqSuccess = recipy.ingredients.Count;

            foreach (KeyValuePair<Recipy.Ingredient, int> ingredientPair in currentMix)
            {
                if (recipy.ingredients.ContainsKey(ingredientPair.Key))
                {
                    if (recipy.ingredients[ingredientPair.Key] == ingredientPair.Value)
                    {
                        success += 1;
                    }
                }
            }
            if (success == reqSuccess)
            {
                return recipy.drinkName;
            }
        }        
        return Recipy.DrinkName.Sludge;
    }
    private int GetShakeAmount(Recipy.DrinkName drinkName)
    {
        foreach(Recipy recipy in drinkRecipys)
        {
            if (recipy.drinkName == drinkName)
            {
                return recipy.shake;
            }
        }
        return 0;
    }
    //recipy book functions
    public void ChangePage(int val)
    {
        recipyIndex += val;
        if (recipyIndex <= -1) { recipyIndex = 0; }
        if (recipyIndex >= recipyPages.Count) { recipyIndex = recipyPages.Count - 1; }
        foreach(GameObject page in recipyPages)
        {
            page.SetActive(false);
        }
        recipyPages[recipyIndex].SetActive(true);
    }
    public void BinIngeredients()
    {
        currentMix = new Dictionary<Recipy.Ingredient, int>();
    }
}
[Serializable]
public class Recipy
{
    public DrinkName drinkName;
    public Dictionary<Ingredient, int> ingredients = new Dictionary<Ingredient, int>();
    public int shake;//0-100

    public Recipy(DrinkName drinkNameIN, Dictionary<Ingredient, int> ingredientsIN, int shakeIN)
    {
        drinkName = drinkNameIN;
        ingredients = ingredientsIN;
        shake = shakeIN;
    }

    

    public enum DrinkName
    {
        Moonshine,
        Bloodmoon,
        Fruitmoon,
        Midnightmoon,
        SexOnTheMoon,
        ShootingStar,
        VoidsRevenge,
        VoidOrangeJuice,
        MoonberryJuice,
        ShotOfCosmicLiquer,
        GlassFromTheUnderWorld,
        WaterCyclone,
        Sludge,
        NULL
    }
    public enum Ingredient
    {
        MoonPotato,
        BlazerIce,
        RedMoonberry,
        VoidOrange,
        CosmicLiqueur,
        SpaceLemon,
        StarDirt,
        ChemiBloom,
        InfernalWine,
        NULL
    }
    public enum DrinkQuality
    {
        Perfect,
        Good,
        Bad,
        Sludge,
        NULL
    }
}