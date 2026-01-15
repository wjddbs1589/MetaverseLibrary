using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Furniture_Alert : MonoBehaviour
{
    Animation anim;

    private void Awake()
    {
        anim = GetComponent<Animation>();
    }

    public void Play_Anim()
    {
        anim.Play();    
    }
}
