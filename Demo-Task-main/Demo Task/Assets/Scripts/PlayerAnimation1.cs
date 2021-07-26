using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation1 : MonoBehaviour
{
    public Animator anim;
    // Update is called once per frame
    void Update()
    {
        anim.SetFloat("Vertical", Input.GetAxis("Vertical"));
        //anim.SetFloat("Horizontal", Input.GetAxis("Horizontal"));
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            anim.Play("Right Turn");
        }
        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            anim.Play("Left Turn");
        }
    }
}
