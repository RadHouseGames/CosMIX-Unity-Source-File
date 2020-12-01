using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDropHandler : MonoBehaviour, IDropHandler
{
    [Header("Obj Refs")]
    public DrinkManager drinkManager;
    public Animator animator;

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("Dropping Item");
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        foreach (RaycastResult raycastResult in results)
        {
            if (raycastResult.gameObject.tag == "Mixer")
            {
                drinkManager.AddIngredient(StaticContoller.ingredientSeletect);
                animator.Play("Shake");
            }
        }
    }

}
