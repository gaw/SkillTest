using UnityEngine;
using System.Collections;

public class CharacterInputController : MonoBehaviour {
	
	private CharacterCollider character;
	private CharacterMotor motor;
	private CharacterMotorSwimming waterMotor;
	
	private Map map;
	private float jumpPressedTime = -100;

	// Use this for initialization
	void Awake() {
		character = GetComponent<CharacterCollider>();
		motor = GetComponent<CharacterMotor>();
		waterMotor = GetComponent<CharacterMotorSwimming>();
		map = (Map) GameObject.FindObjectOfType( typeof(Map) );
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		direction = Vector3.ClampMagnitude(direction, 1);
		
		if(IsInWater()) {
			waterMotor.enabled = true;
			motor.enabled = false;
			
			waterMotor.inputEmersion = Input.GetButton("Jump");
			waterMotor.inputMoveDirection = transform.TransformDirection(direction);
			
			
		} else {
			waterMotor.enabled = false;
			motor.enabled = true;
			
			motor.inputMoveDirection = transform.TransformDirection(direction);
			
			if(Input.GetButtonDown("Jump")) {
				jumpPressedTime = Time.time;
			}
			if( !Input.GetButton("Jump") ) {
				jumpPressedTime = -100;
			}
			motor.inputJump = Time.time - jumpPressedTime <= 0.2f;
			motor.holdingInputJump = Input.GetButton("Jump");
		}
	}
	
	private bool IsInWater() {
		Vector3 bottom = transform.position;
		Vector3 top = bottom + Vector3.up*character.height;
		Vector3 pos = Vector3.Lerp(bottom, top, 0.2f);
		return map.GetBlock( Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), Mathf.RoundToInt(pos.z) ).IsFluid();
	}
	
}
