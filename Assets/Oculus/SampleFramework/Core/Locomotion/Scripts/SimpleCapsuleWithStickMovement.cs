using System;
using UnityEngine;

public class SimpleCapsuleWithStickMovement : MonoBehaviour
{
	public bool EnableLinearMovement = true;
	public bool EnableRotation = true;
	public bool HmdRotatesPlayer = true;
	public bool RotationEitherThumbstick = false;
	public float RotationAngle = 45.0f;
	public float Speed;
	public OVRCameraRig CameraRig;

	private Vector3 moveDir;
	private Rigidbody rb;
	[SerializeField] private float maxSpeed;
	[SerializeField] float counterForceFactor = 10f;
	[SerializeField] private float rotationSpeed=5;
	[SerializeField] private Transform eyeAnchor;

	public event Action CameraUpdated;
	public event Action PreCharacterMove;

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
		if (CameraRig == null) CameraRig = GetComponentInChildren<OVRCameraRig>();
	}

	private void FixedUpdate()
	{
        if (CameraUpdated != null) CameraUpdated();
        if (PreCharacterMove != null) PreCharacterMove();

        if (HmdRotatesPlayer) RotatePlayerToHMD();
		if (EnableLinearMovement) StickMovement();
		if (EnableRotation) SnapTurn();
		
		CounterMovement();
	}

	private void CounterMovement()
	{
		Vector2 movementInput = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
		bool noInput = Mathf.Approximately(Vector3.SqrMagnitude(movementInput), 0);
		bool oppositeInput = Vector3.Dot(rb.velocity, movementInput) <= 0;
		if (noInput)
		{
			Vector3 counterForce = rb.velocity * (-0.99f);
			rb.AddForce(counterForce);
		}
		// else if (oppositeInput)
		// {
		// 	Vector3 counterForce = moveDir * (Time.fixedDeltaTime * counterForceFactor * 0.99f);
		// 	rb.AddForce(counterForce);
		// 	Debug.Log(counterForce.magnitude);
		// }
	}
	
    private void RotatePlayerToHMD()
    {
		Transform root = CameraRig.trackingSpace;
		Transform centerEye = CameraRig.centerEyeAnchor;

		Vector3 prevPos = root.position;
		Quaternion prevRot = root.rotation;

		Quaternion centerEyeRotation = centerEye.rotation;
		transform.rotation = Quaternion.Euler(centerEyeRotation.eulerAngles.x, centerEyeRotation.eulerAngles.y, centerEyeRotation.eulerAngles.z);

		root.position = prevPos;
		root.rotation = prevRot;
    }

    private void StickMovement()
	{
		Quaternion ort = CameraRig.centerEyeAnchor.rotation;
		// Vector3 ortEuler = ort.eulerAngles;
		// ortEuler.z = ortEuler.x = 0f;
		// ort = Quaternion.Euler(ortEuler);

		moveDir = Vector3.zero;
		Vector2 primaryAxis = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
		moveDir += ort * (primaryAxis.x * Vector3.right);
		moveDir += ort * (primaryAxis.y * Vector3.forward);
		//_rigidbody.MovePosition(_rigidbody.transform.position + moveDir * Speed * Time.fixedDeltaTime);
		rb.AddForce(moveDir * (Speed * Time.fixedDeltaTime));
		Debug.Log((moveDir * (Speed * Time.fixedDeltaTime)).magnitude);
		if(rb.velocity.sqrMagnitude > maxSpeed*maxSpeed)
		{
			rb.velocity = rb.velocity.normalized * maxSpeed;
		}
	}

	private void SnapTurn()
	{
		if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickLeft) || 
		    (RotationEitherThumbstick && OVRInput.Get(OVRInput.Button.PrimaryThumbstickLeft)))
		{
			transform.Rotate(Vector3.up, 
				-RotationAngle*Time.deltaTime*rotationSpeed);
		}
		else if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickRight) ||
		         (RotationEitherThumbstick && OVRInput.Get(OVRInput.Button.PrimaryThumbstickRight)))
		{
			transform.Rotate(Vector3.up,
				RotationAngle * Time.deltaTime * rotationSpeed);
		}
		
		if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickUp) ||
		         (RotationEitherThumbstick && OVRInput.Get(OVRInput.Button.PrimaryThumbstickUp)))
		{
			transform.Rotate(-Vector3.right,
				RotationAngle * Time.deltaTime * rotationSpeed);
		}
		
		else if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickDown) ||
		         (RotationEitherThumbstick && OVRInput.Get(OVRInput.Button.PrimaryThumbstickDown)))
		{
			transform.Rotate(-Vector3.right,
				-RotationAngle * Time.deltaTime * rotationSpeed);
		}
	}
}
