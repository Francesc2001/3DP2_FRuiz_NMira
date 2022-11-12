using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PortalButton : MonoBehaviour
{
    public UnityEvent m_Event;
    public UnityEvent m_Event2;

    public Animator animator;
    public string boolName = "myBool";
    
    public void SetBool(bool value)
    {
        animator.SetBool(boolName, value);
    }
    
}
