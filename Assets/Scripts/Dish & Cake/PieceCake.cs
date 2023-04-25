using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceCake : MonoBehaviour
{
    Animator animator;

    AnimationClip animation1;
    AnimationClip animation2;
    AnimationClip animation3;
    AnimationClip animation4;

    string currentState;

    const string CINNAMON_0 = "Cinnamon Rolls";
    const string CINNAMON_1 = "Cinnamon Rolls 1";
    const string CINNAMON_2 = "Cinnamon Rolls 2";
    const string CINNAMON_3 = "Cinnamon Rolls 3";
    const string CINNAMON_4 = "Cinnamon Rolls 4";

    public string[] cinnamon = new string[4];

    private void Start()
    {
        animator = gameObject.GetComponent<Animator>();

        //ver donde colocar esto, porque deberias poder pasar, el num de torta que queres mover.
        //

        //ChangeAnimationState(CINNAMON_1);

        //if(!IsAnimationPlaying(animator, CINNAMON_0))
        //{
        //    ChangeAnimationState(CINNAMON_1);
        //}
    } 

    public string ReturnCinnamon(int numPiece)
    {
        switch (numPiece)
        {
            case 0:
                return CINNAMON_1;
            case 1:
                return CINNAMON_2;
            case 2:
                return CINNAMON_3;
            case 3:
                return CINNAMON_4;
        }
        return null;
    }

    public void ChangeAnimationState(string newState)/*, int num)*/
    {
        //newState = newState + " " + num.ToString();

        if (newState == currentState)
        {
            return;
        }

        animator.Play(newState);
        currentState = newState; 
    }

    //check if a specific animation is playing
    public bool IsAnimationPlaying(Animator animator, string stateName)
    {
        if(animator.GetCurrentAnimatorStateInfo(0).IsName(stateName) && 
           animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            return true;
        }
        else { return false; }
    }
}
