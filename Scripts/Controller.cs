using UnityEngine;
using System.Collections;

public class Controller : MonoBehaviour
{

	public WheelCollider[] WColForward;
	public WheelCollider[] WColBack;

	public float maxSteer = 30; //1
	public float maxAccel = 25; //2
	public float maxBrake = 50; //3


	// Use this for initialization
	void Start()
	{

	}


	void FixedUpdate()
	{

		float accel = 0;
		float steer = 0;

		accel = Input.GetAxis("Vertical");
		steer = Input.GetAxis("Horizontal");   

		CarMove(accel, steer); 

	}

	private void CarMove(float accel, float steer)
	{ 

		foreach (WheelCollider col in WColForward)
		{ 
			col.steerAngle = steer * maxSteer; 
		}

		if (accel == 0)
		{
			foreach (WheelCollider col in WColBack)
			{  
				col.brakeTorque = maxBrake; 
			}

		}
		else
		{ 

			foreach (WheelCollider col in WColBack)
			{ 
				col.brakeTorque = 0; 
				col.motorTorque = accel * maxAccel; 
			}

		}



	}
}


//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Controller : MonoBehaviour
//{

//    public float Motorforce, Steerforce, Brakeforce;
//    public WheelCollider FR_L_Wheel, FR_R_Wheel, RE_L_Wheel, RE_R_Wheel;

//    public float WheelSpeed = 20;
//    public float WheelRpmMax = 20;
//    private float WheelRotateAngle = 20;


//    // Start is called before the first frame update
//    void Start()
//    {

//    }


//    // Update is called once per frame
//    void Update()
//    {

//        float v = Input.GetAxis("Vertical") * Motorforce;
//        float h = Input.GetAxis("Horizontal") * Steerforce;
//        print(v);

//        RE_L_Wheel.motorTorque = v;
//        RE_R_Wheel.motorTorque = v;

//        FR_L_Wheel.steerAngle = h;
//        FR_R_Wheel.steerAngle = h;

//        if (Input.GetKey(KeyCode.Space))
//        {
//            RE_L_Wheel.brakeTorque = Brakeforce;
//            RE_R_Wheel.brakeTorque = Brakeforce;
//        }

//        if (Input.GetKeyUp(KeyCode.Space))
//        {
//            RE_R_Wheel.brakeTorque = 0;
//            RE_L_Wheel.brakeTorque = 0;
//        }

//        if (Input.GetAxis("Vertical") == 0)
//        {
//            //RE_L_Wheel.brakeTorque = Brakeforce;
//            //RE_R_Wheel.brakeTorque = Brakeforce;
//        }
//        else
//        {
//            RE_R_Wheel.brakeTorque = 0;
//            RE_L_Wheel.brakeTorque = 0;
//        }

//    }

//}
