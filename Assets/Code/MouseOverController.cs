using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseOverController : MonoBehaviour
{
    public Animator animator;
    public GameObject label;

    public void Start()
    {
        if (label != null) { label.SetActive(false); }        
    }
    public void MouseEnter()
    {
        animator.SetBool("MouseOver", true);
        if (label != null) { label.SetActive(true); }
    }
    public void MouseExit()
    {
        animator.SetBool("MouseOver", false);
        Invoke("Hide", 0.5f);
    }   
    
    private void Hide()
    {
        if (label != null) { label.SetActive(false); }
    }
}
