using UnityEngine;
using System.Collections;

public class RotateBodyParts : MonoBehaviour {
	
	public Transform r_upperArm;
	public Transform r_foreArm;
	public Transform r_thigh, l_thigh;
	public Transform r_toe, l_toe;
	public Transform spine, spine1;
	
	private float aor, ini_h, ini_d, cur_d, r_toe_ini_y, l_toe_ini_y;
	private Vector3 ini_pose, cur_pose, point;
	private float poseCheck;
	private Quaternion r_thigh_ini_rot, l_thigh_ini_rot, ini_rot, spine1_ini_rot;
	private int cur_stage;
	private bool keyboard, left, right, down, up;
	// Use this for initialization
	void Start () {
		aor	= 0f;
		ini_d= Vector3.Distance(r_toe.position, l_toe.position);
		cur_stage = 0;
		ini_pose = transform.position - spine1.position;
		ini_pose.Normalize();
		r_toe_ini_y = r_toe.position.y;
		l_toe_ini_y = l_toe.position.y;
		ini_h = transform.position.y;
		r_thigh_ini_rot = r_thigh.rotation;
		l_thigh_ini_rot = l_thigh.rotation;
		ini_rot = transform.rotation;
		keyboard = true;
		right = false;
		left = false;
		down = false;
		up   = false; 
		spine1_ini_rot = spine1.rotation;
		point = new Vector3(0.2f,0.1f,0.5f);
	}
	
	// Update is called once per frame
	void Update () 
	{
		cur_pose = transform.position - spine1.position;
		cur_pose.Normalize();
		poseCheck = Vector3.Dot(ini_pose, cur_pose);
		TestMovement();
		
		if (keyboard == true) 
		{
			float h_move = Input.GetAxis("Horizontal");				
			if (h_move < 0)
			{
				cur_stage = 1;
				keyboard = false;
				left = true;
			}
			else if (h_move > 0)
			{
				cur_stage = 1;
				keyboard = false;
				right = true;
			}
			float v_move = Input.GetAxis("Vertical");
			if (v_move < 0)
			{
				cur_stage = 1;
				keyboard = false;
				down = true;
			}
			else if (v_move >0)
			{
				cur_stage = 1;
				keyboard = false;
				up = true;
			}
		}
		
		if (left == true) 
		{
			MoveLeft();
		}
		if (right == true)
		{
			MoveRight();
		}
		if (down == true)
		{	
			MoveForward();
		}
		if (up == true)
		{
			MoveBackward();
		}
	}
	
	// rotate an object fix amount per frame 
	// degree per frame  : 1f
	// axis of rotation  : Vector3.forward
	float RotatePartPointAngle(Transform part, Vector3 point, float angle, float aor)
	{
		float sign;
		sign = angle/Mathf.Abs(angle);
		
		if (aor < Mathf.Abs(angle))
		{
			part.RotateAround(point, Vector3.forward, sign*1f);
			aor += 1f;
		}
		return aor;
	}
	
	void RotateArm()
	{
		//aor_rua = RotatePartAngle(r_upperArm, -1f, 90f, aor_rua);
		//aor_rfa = RotatePartAngle(r_foreArm, -1f, 110f, aor_rfa);
	}

	void TestMovement()
	{
		// clockwise is positive rotation
		
		switch (Input.inputString)
		{
		case "1":
			//rotate right thigh clockwise around the pelvis
			r_thigh.RotateAround(transform.position, Vector3.forward, 1f);
			break;
		case "2":
			//rotate right thigh counter clockwise around the pelvis
			r_thigh.RotateAround(transform.position, Vector3.forward, -1f);
			break;
		case "3":
			//rotate whole body counter clockwise around left toe
			transform.RotateAround(l_toe.position, Vector3.forward , -1f);
			break;
		case "4":
			//rotate whole body clockwise around left toe
			transform.RotateAround(l_toe.position, Vector3.forward , 1f);
			break;
		case "5":
			// rotate everything aroud the pelvis except the right thigh
			transform.RotateAround(transform.position, Vector3.forward, 1f);
			r_thigh.RotateAround(transform.position, Vector3.forward, -1f);
			break;
		case "6":
			// rotate left thigh clockwise around the body
			l_thigh.RotateAround(transform.position, transform.forward, 1f);
			break;
		case "7":
			// rotate body aroudn right toe
			transform.RotateAround(r_toe.position, Vector3.forward, -2f);
			break;
		case "8":
			transform.RotateAround(r_toe.position, Vector3.forward, 2f);
			break;
		case "0":
			cur_stage++;
			break;
		}
	}

	void MoveLeft()
	{
		switch (cur_stage)
		{
		case 1:	// first stage, move right leg up
			r_thigh.RotateAround(transform.position, transform.forward, 2f);
			aor += 2f;
			if (aor > 9f)
			{
				cur_stage++;
				aor = 0f;
			}
			break;
		case 2: // second stage, rotate body counter clockwise around left toe
				// till the right toe is on the ground
			transform.RotateAround(l_toe.position, transform.forward , -2f);
			spine1.rotation = spine1_ini_rot;
			if (Mathf.Abs(r_toe.position.y - r_toe_ini_y) < 0.01)
			{
				cur_stage++;
			}
			break;
		case 3:	// third stage, rotate body counter clockwise around the right toe 
				// till the right thigh come to initial rotation
			transform.RotateAround(r_toe.position, Vector3.forward, -2f);
			if (r_thigh_ini_rot == r_thigh.rotation)
			{
				cur_stage++;
			}
			break;
		case 4: // rotate left thigh clockwise till left thigh initial rotation
			l_thigh.RotateAround(transform.position, transform.forward, 2f);
			if (l_thigh.rotation == l_thigh_ini_rot)
			{
				cur_stage++;
			}
			break;
		case 5:	// forth stage, rotate body clockwise around the right toe 
				// except the right thigh till body have the initial rotation
			transform.RotateAround(transform.position, transform.forward, 2f);
			r_thigh.RotateAround(transform.position, transform.forward, -2f);
			l_thigh.RotateAround(transform.position, transform.forward, -2f);
			spine1.rotation = spine1_ini_rot;
			if (transform.rotation == ini_rot)
			{
				cur_stage = 0;
				keyboard = true;
				left = false;
			}
			break;
		}
		
	}
	
	void MoveRight()
	{
		switch (cur_stage)
		{
		case 1:	// first stage, move left leg up
			l_thigh.RotateAround(transform.position, transform.forward, -2f);
			aor += 2f;
			if (aor > 9f)
			{
				cur_stage++;
				aor = 0f;
			}
			break;
		case 2: // second stage, rotate body counter clockwise around left toe
				// till the right toe is on the ground
			transform.RotateAround(r_toe.position, transform.forward , 2f);
			spine1.rotation = spine1_ini_rot;
			if (l_toe.position.y - l_toe_ini_y < 0.01)
			{
				cur_stage++;
			}
			break;
		case 3:	// third stage, rotate body counter clockwise around the right toe 
				// till the right thigh come to initial rotation
			transform.RotateAround(l_toe.position, transform.forward, 2f);
			if (l_thigh_ini_rot == l_thigh.rotation)
			{
				cur_stage++;
			}
			break;
		case 4:	// forth stage, rotate body clockwise around the right toe 
				// except the right thigh till body have the initial rotation
			transform.RotateAround(transform.position, transform.forward, -2f);
			l_thigh.RotateAround(transform.position, transform.forward, 2f);
			spine1.rotation = spine1_ini_rot;
			if (transform.rotation == ini_rot)
			{
				cur_stage = 0;
				keyboard = true;
				right = false;
			}
			break;
		}
		
	}

	void MoveForward()
	{
		switch (cur_stage)
		{
		case 1:	// first stage, move right thigh forward
			r_thigh.RotateAround(transform.position, transform.right, -2f);
			aor += 2f;
			if (aor > 19f)
			{
				cur_stage++;
				aor = 0f;
			}
			break;
		case 2: // second stage, rotate body forward
				// till the right toe is on the ground
			transform.RotateAround(l_toe.position, transform.right , 2f);
			spine1.rotation = spine1_ini_rot;
			if (r_toe.position.y - r_toe_ini_y < 0.01)
			{
				cur_stage++;
			}
			break;
		case 3:	// third stage, rotate body forward around the right toe 
				// till the right thigh come to initial rotation
			transform.RotateAround(r_toe.position, transform.right, 2f);		
			if (r_thigh_ini_rot == r_thigh.rotation)
			{
				cur_stage++;
			}
			break;
		case 4: // rotate left thigh forward till left thigh initial rotation
			l_thigh.RotateAround(transform.position, transform.right, -2f);		
			if (l_thigh_ini_rot == l_thigh.rotation)
			{
				cur_stage++;
			}
			break;
		case 5:	// forth stage, rotate body clockwise around the right toe 
				// except the right thigh till body have the initial rotation
			transform.RotateAround(transform.position, transform.right, -2f);
			r_thigh.RotateAround(transform.position, transform.right, 2f);
			l_thigh.RotateAround(transform.position, transform.right, 2f);
			spine1.rotation = spine1_ini_rot;
			if (transform.rotation == ini_rot)
			{
				cur_stage=0;
				keyboard = true;
				down = false;
			}
			break;
		}
		
	}
	
	void MoveBackward()
	{
		switch (cur_stage)
		{
		case 1:	// first stage, move right thigh backward
			r_thigh.RotateAround(transform.position, transform.right, 2f);
			aor += 2f;
			if (aor > 19f)
			{
				cur_stage++;
				aor = 0f;
			}
			break;
		case 2: // second stage, rotate body backward
				// till the right toe is on the ground
			transform.RotateAround(l_toe.position, transform.right , -2f);
			spine1.rotation = spine1_ini_rot;
			if (r_toe.position.y - r_toe_ini_y < 0.01)
			{
				cur_stage++;
			}
			break;
		case 3:	// third stage, rotate body backward around the right toe 
				// till the right thigh come to initial rotation
			transform.RotateAround(r_toe.position, transform.right, -2f);		
			if (r_thigh_ini_rot == r_thigh.rotation)
			{
				cur_stage++;
			}
			break;
		case 4: // rotate left thigh backward till left thigh initial rotation
			l_thigh.RotateAround(transform.position, transform.right, 2f);		
			if (l_thigh_ini_rot == l_thigh.rotation)
			{
				cur_stage++;
			}
			break;
		case 5:	// forth stage, rotate body clockwise around the right toe 
				// except the right thigh till body have the initial rotation
			transform.RotateAround(transform.position, transform.right, -2f);
			r_thigh.RotateAround(transform.position, transform.right, 2f);
			l_thigh.RotateAround(transform.position, transform.right, 2f);
			spine1.rotation = spine1_ini_rot;
			if (transform.rotation == ini_rot)
			{
				cur_stage=0;
				keyboard = true;
				up = false;
			}
			break;
		}
		
	}
}
