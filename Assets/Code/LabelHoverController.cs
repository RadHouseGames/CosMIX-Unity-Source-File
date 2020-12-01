using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabelHoverController : MonoBehaviour
{
    public void Start()
    {
        gameObject.SetActive(false);
    }
    public void MouseEnter()
    {
        gameObject.SetActive(true);
    }
    public void MouseExit()
    {
        gameObject.SetActive(false);
    }
}
