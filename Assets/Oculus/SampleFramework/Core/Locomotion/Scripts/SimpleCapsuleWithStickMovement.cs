using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor.Experimental;
using UnityEngine;

public class SimpleCapsuleWithStickMovement : MonoBehaviour
{
	public bool EnableLinearMovement = true;
	public bool EnableRotation = true;
	public bool HMDRotatesPlayer = true;
	public bool RotationEitherThumbstick = false;
	public float RotationAngle = 45.0f;
	public float Speed = 0.0f;
	public OVRCameraRig CameraRig;

	private bool ReadyToSnapTurn;
	private Vector3 moveDir;
	private Rigidbody _rigidbody;
	[SerializeField] private float maxSpeed;
	[SerializeField] float counterForceFactor = 10f;

	public event Action CameraUpdated;
	public event Action PreCharacterMove;

	private void Awake()
	{
		_rigidbody = GetComponent<Rigidbody>();
		if (CameraRig == null) CameraRig = GetComponentInChildren<OVRCameraRig>();
	}

	private void FixedUpdate()
	{
        if (CameraUpdated != null) CameraUpdated();
        if (PreCharacterMove != null) PreCharacterMove();

        if (HMDRotatesPlayer) RotatePlayerToHMD();
		if (EnableLinearMovement) StickMovement();
		if (EnableRotation) SnapTurn();
		CounterMovement();
	}

	private void CounterMovement()
	{
		Vector2 movementInput = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
		bool noInput = Mathf.Approximately(Vector3.SqrMagnitude(movementInput), 0);
		bool oppositeInput = Vector3.Dot(_rigidbody.velocity, movementInput) <= 0;
		if (noInput)
		{
			Vector3 counterForce = _rigidbody.velocity * (-0.99f);
			_rigidbody.AddForce(counterForce);
		}
		else if (oppositeInput)
		{
			
			Vector3 counterForce = moveDir * (Speed * Time.fixedDeltaTime * counterForceFactor);
			_rigidbody.AddForce(counterForce);
		}
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
		_rigidbody.AddForce(moveDir * (Speed * Time.fixedDeltaTime));
		if(_rigidbody.velocity.sqrMagnitude > maxSpeed*maxSpeed)
		{
			_rigidbody.velocity = _rigidbody.velocity.normalized * maxSpeed;
		}
	}

	private void SnapTurn()
	{
		if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickLeft) ||
			(RotationEitherThumbstick && OVRInput.Get(OVRInput.Button.PrimaryThumbstickLeft)))
		{
			transform.RotateAround(CameraRig.centerEyeAnchor.position, Vector3.up, -RotationAngle);
		}
		else if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickRight) ||
			(RotationEitherThumbstick && OVRInput.Get(OVRInput.Button.PrimaryThumbstickRight)))
		{
			transform.RotateAround(CameraRig.centerEyeAnchor.position, Vector3.up, RotationAngle);
		}
	}
}
