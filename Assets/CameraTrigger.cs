using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CameraValues
{
    public Vector3 referenceVector;
    [Space(5f)]
    public Transform lookTarget;
    public Vector3 offset;
    [Space(2.5f)]
    public Transform orbitTarget;
    [Space(5f)]
    public float angleCutoff;
    [Range(0,15f)]
    public float distance;
    public float facingSpeed;
    public float faceTime;
    public bool useTransformForward;
    public float lerpForce = 1;
}

public class CameraTrigger : MonoBehaviour
{
    EnigmaCamera eCam;
    public enum ChangeType {Custom, BeforeTrigger}
    public CameraValues memoryHold;

    public ChangeType onEnterValue;
    public CameraValues onEnter;
    [Space(15)]
    public ChangeType onExitValue;
    public CameraValues onExit;

    void Start()
    {}
    void OnTriggerEnter(Collider col)
    {
		if(col.transform.parent.GetComponentInChildren<EnigmaCamera>() != null)
		{
            eCam = col.transform.parent.GetComponentInChildren<EnigmaCamera>();

			memoryHold.referenceVector = eCam.referenceVector;
            memoryHold.lookTarget = eCam.lookTarget;
            memoryHold.offset = eCam.offset;
            memoryHold.orbitTarget = eCam.orbitTarget;
            memoryHold.angleCutoff = eCam.angleCutoff;
            memoryHold.distance = eCam.distance;
            memoryHold.facingSpeed = eCam.facingSpeed;
            memoryHold.faceTime = eCam.faceTime;
            memoryHold.useTransformForward = eCam.useTransformForward;
            memoryHold.lerpForce = eCam.lerpForce;

			if(onEnterValue == ChangeType.Custom)
			{
				eCam.referenceVector = onEnter.referenceVector;
                if(onEnter.lookTarget != null)
                eCam.lookTarget = onEnter.lookTarget;
                eCam.offset = onEnter.offset;
                if(onEnter.orbitTarget != null)
                eCam.orbitTarget = onEnter.orbitTarget;
                eCam.angleCutoff = onEnter.angleCutoff;
                eCam.distance = onEnter.distance;
                eCam.facingSpeed = onEnter.facingSpeed;
                eCam.faceTime = onEnter.faceTime;
                eCam.useTransformForward = onEnter.useTransformForward;
                eCam.lerpForce = onEnter.lerpForce;
			}
        }
    }

    void OnTriggerExit(Collider col)
    {
		if(col.transform.parent.GetComponentInChildren<EnigmaCamera>() != null)
		{
            eCam = col.transform.parent.GetComponentInChildren<EnigmaCamera>();

            if(onExitValue == ChangeType.BeforeTrigger)
			{
				eCam.referenceVector = memoryHold.referenceVector;
                eCam.lookTarget = memoryHold.lookTarget;
                eCam.offset = memoryHold.offset;
                eCam.orbitTarget = memoryHold.orbitTarget;
                eCam.angleCutoff = memoryHold.angleCutoff;
                eCam.distance = memoryHold.distance;
                eCam.facingSpeed = memoryHold.facingSpeed;
                eCam.faceTime = memoryHold.faceTime;
                eCam.useTransformForward = memoryHold.useTransformForward;
                eCam.lerpForce = memoryHold.lerpForce;
			}
			else
			{
				eCam.referenceVector = onExit.referenceVector;
                eCam.lookTarget = onExit.lookTarget;
                eCam.offset = onExit.offset;
                eCam.orbitTarget = onExit.orbitTarget;
                eCam.angleCutoff = onExit.angleCutoff;
                eCam.distance = onExit.distance;
                eCam.facingSpeed = onExit.facingSpeed;
                eCam.faceTime = onExit.faceTime;
                eCam.useTransformForward = onExit.useTransformForward;
                eCam.lerpForce = onExit.lerpForce;
			}
        }
    }
}
