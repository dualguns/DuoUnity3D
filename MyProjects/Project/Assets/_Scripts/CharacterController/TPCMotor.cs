using UnityEngine;
using System.Collections;

public class TPCMotor : MonoBehaviour
{
	public static TPCMotor Instance;
	public float MoveSpeed = 10f;
	public Vector3 MoveVector { get; set; }
	
	void Awake()
	{
		Instance = this;
	}
	
	public void UpdateMotor()
	{
		SnapAlignCharacterWithCamera();
		ProcessMotion();
	}

	void SnapAlignCharacterWithCamera()
	{
		if (MoveVector.x != 0 ||
			MoveVector.z != 0)
		{
			transform.rotation = Quaternion.Euler(
				transform.eulerAngles.x,
				Camera.mainCamera.transform.eulerAngles.y,
				transform.eulerAngles.z);
		}
	}
	
	void ProcessMotion()
	{
		// Transform MoveVector to World Space
		MoveVector = transform.TransformDirection(MoveVector);
		
		// Normalize MoveVector if Magnitude > 1
		if (MoveVector.magnitude > 1)
		{
			MoveVector = Vector3.Normalize(MoveVector);
		}
		
		// Multiply MoveVector by MoveSpeed
		MoveVector *= MoveSpeed;
		
		// Multiply MoveVector by DeltaTime
		MoveVector *= Time.deltaTime;
		
		// Move the Character in World Space
		TPCController.CharacterController.Move(MoveVector);
		
	}
	

}
