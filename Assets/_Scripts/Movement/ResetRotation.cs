using System;
using UnityEngine;

public class ResetRotation : MonoBehaviour
{
	
	[SerializeField] private GameObject player;
	
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
	    Vector3 angleToTarget =   - player.transform.rotation.eulerAngles;
	    

	    player.transform.Rotate(angleToTarget.x, angleToTarget.y, angleToTarget.z);
	}
    
}
