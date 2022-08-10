using System;
using UnityEngine;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine.Assertions;
#if UNITY_EDITOR
using UnityEngine.SceneManagement;
#endif

public class LocomotionController : MonoBehaviour
{
    public OVRCameraRig CameraRig;
    //public CharacterController CharacterController;
    public CapsuleCollider CharacterController;
	//public OVRPlayerController PlayerController;
	public SimpleCapsuleWithStickMovement PlayerController;

    void Start()
    {
        if(CameraRig == null)
        {
            CameraRig = FindObjectOfType<OVRCameraRig>();
        }
        Assert.IsNotNull(CameraRig);
#if UNITY_EDITOR
        OVRPlugin.SendEvent("locomotion_controller", (SceneManager.GetActiveScene().name == "Locomotion").ToString(), "sample_framework");
#endif
	}
}
