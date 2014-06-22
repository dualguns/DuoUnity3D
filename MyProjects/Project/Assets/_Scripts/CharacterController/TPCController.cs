using UnityEngine;
using System.Collections;

public class TPCController : MonoBehaviour 
{
	public static CharacterController CharacterController;
	public static TPCController Instance;
	
	void Awake () 
	{
		CharacterController = GetComponent("CharacterController") as CharacterController;
		Instance = this;
	}
	
	void Update() {
		
		if (Camera.mainCamera == null)
		{
			return;
		}
		
		GetLocomotionInput();
		
		TPCMotor.Instance.UpdateMotor();
	}
	
	void GetLocomotionInput()
	{
		var deadZone = 0.1f;
		
		TPCMotor.Instance.MoveVector = Vector3.zero;
		
		if (Input.GetAxis("Vertical") > deadZone ||
			Input.GetAxis("Vertical") < -deadZone)
		{
			TPCMotor.Instance.MoveVector += new Vector3(0, 0, Input.GetAxis("Vertical"));
		}
		if (Input.GetAxis("Horizontal") > deadZone ||
			Input.GetAxis("Horizontal") < -deadZone)
		{
			TPCMotor.Instance.MoveVector += new Vector3(Input.GetAxis("Horizontal"), 0, 0);
		}
		
	}
}
