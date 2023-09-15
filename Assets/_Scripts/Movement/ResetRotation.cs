using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetRotation : MonoBehaviour
{
	
    private Transform player;
    private Vector3 pointTo;

    private void Start()
    {
        player = GetComponent<Transform>();
        if(SceneManager.GetActiveScene().name == "MainMenu")
            pointTo = new Vector3(0,0,0);
        else
            pointTo = new Vector3(0,180,0);
    }

    private void Update()
    {
	    if(OVRInput.Get(OVRInput.Button.SecondaryThumbstick))
	    {
		    ResetRotationToForward();
	    }
	    
    }
    
    private void ResetRotationToForward()
	{
	    // Calculate the direction from the current position to the target position
	    Vector3 angleToTarget =   pointTo - player.rotation.eulerAngles;
	    

	    player.Rotate(angleToTarget.x, angleToTarget.y, angleToTarget.z);
	}
    
}
