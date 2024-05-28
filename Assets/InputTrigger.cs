using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InputValues
{
     public bool canMove = true;
     public InputReference inputReference;
     public GameObject referenceObject;
     public float splineExaggeration = 1;
}

public class InputTrigger : MonoBehaviour
{
     public InputPlayer currentInput;
     public enum ChangeType {Custom, BeforeTrigger}
     public InputValues memoryHold;

     public ChangeType onEnterValue;
     public InputValues onEnter;
     [Space(15)]
     public ChangeType onExitValue;
     public InputValues onExit;
	
    void Start()
    {

    }

    void OnTriggerEnter(Collider col)
    {
	if(col.gameObject.GetComponent<InputPlayer>() != null)
	{
	    //currentInput = col.gameObject.GetComponent<InputPlayer>();
	    memoryHold.canMove = currentInput.canMove;
     	    memoryHold.inputReference = currentInput.inputReference;
   	    memoryHold.referenceObject = currentInput.referenceObject;
    	    memoryHold.splineExaggeration = currentInput.splineExaggeration;
	    if(onEnterValue == ChangeType.Custom)
	    {
	        currentInput.canMove = onEnter.canMove;
	        currentInput.inputReference = onEnter.inputReference;
       	        currentInput.referenceObject = onEnter.referenceObject;
	        currentInput.splineExaggeration = onEnter.splineExaggeration;
	    }
	}
    }

    void OnTriggerExit(Collider col)
    {
	if(col.gameObject.GetComponent<InputPlayer>() != null)
	{
	    //currentInput = col.gameObject.GetComponent<InputPlayer>();
	    if(onExitValue == ChangeType.BeforeTrigger)
	    {
		currentInput.canMove = memoryHold.canMove;
		currentInput.inputReference = memoryHold.inputReference;
		currentInput.referenceObject = memoryHold.referenceObject;
		currentInput.splineExaggeration = memoryHold.splineExaggeration;
	    }
 	    else
	    {
		currentInput.canMove = onExit.canMove;
		currentInput.inputReference = onExit.inputReference;
		currentInput.referenceObject = onExit.referenceObject;
		currentInput.splineExaggeration = onExit.splineExaggeration;
	    }
	}
    }
}
