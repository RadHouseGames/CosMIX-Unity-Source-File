using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDragHandler : MonoBehaviour, IDragHandler, IEndDragHandler
{
    [Header("Obj Refs")]
    public DrinkManager drinkManager;
    public Animator animator;
       

    private Vector3 startPos;
    public void OnDrag(PointerEventData eventData)
    {
        GetComponentInParent<Animator>().enabled = false;
        gameObject.transform.parent.SetAsLastSibling();
        transform.position = Input.mousePosition;        
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        foreach (RaycastResult raycastResult in results)
        {
            if (raycastResult.gameObject.tag == "Ingredient")
            {
                StaticContoller.ingredientSeletect = raycastResult.gameObject.name;
            }
        }
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        transform.position = startPos;
        GetComponentInParent<Animator>().enabled = true;
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
    public void Start()
    {
        startPos = transform.position;
    }
}
