using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";

    private float horizontalInput;
    private float verticalInput;
    private float currentSteerAngle;
    private float currentbreakForce;
    private bool isBreaking;
    private float controlMode;
    public Transform camera;


    public CameraController cameraController;

    [SerializeField] private float motorForce;
    [SerializeField] private float breakForce;
    [SerializeField] private float maxSteerAngle;

    [SerializeField] public WheelCollider frontLeftWheelCollider;
    [SerializeField] public WheelCollider frontRightWheelCollider;
    [SerializeField] public WheelCollider rearLeftWheelCollider;
    [SerializeField] public WheelCollider rearRightWheelCollider;

    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform frontRightWheeTransform;
    [SerializeField] private Transform rearLeftWheelTransform;
    [SerializeField] private Transform rearRightWheelTransform;
    public Rigidbody rigidbody;


    private bool isTurbo = false;
    private bool isNormal = true;
    private bool showhugInfo = false;


    private float nitro = 10;

    private float startTurboTime = 0;

    public csDestroyEffect nitroEffect;

    public Gui Gui;


    public float currentSpeed;


    private void Start()
    {
        Gui = FindObjectOfType<Gui>();
        nitroEffect = GetComponentInChildren<csDestroyEffect>();

        rigidbody = GetComponent<Rigidbody>();
        rigidbody.centerOfMass = new Vector3(0, 0.1f, 0);
        cameraController = FindObjectOfType<CameraController>();
    }
    private void FixedUpdate()
    {
   
        ChangeControlMode();

        GetInput();

        if (controlMode == 2) {
            rigidbody.useGravity = false;
            rigidbody.isKinematic = true;
            cameraController.Y_ANGLE_MIN = -89;
            cameraController.Y_ANGLE_MAX = 89;
            FlyControlMode();
        }
        else
        {
            cameraController.Y_ANGLE_MIN = -89;
            cameraController.Y_ANGLE_MAX = 89;
            rigidbody.useGravity = true;
            rigidbody.isKinematic = false;
            HandleMotor();
            HandleSteering();
            UpdateWheels();
        }

    }

    public void Update()
    {

        StartSpeedControl();
        UpdateGui();
        //print("2 " + frontLeftWheelCollider.motorTorque);
    }


    public void StartSpeedControl()
    {
        currentSpeed = rigidbody.velocity.magnitude;
        if (currentSpeed < 6)
        {
            motorForce = 2000;
            breakForce = 2000;
        }
        else
        {
            motorForce = 200;
            breakForce = 200;
        }
    }


    public void UpdateGui()
    {
        //Gui.ChangeSpeedText(frontLeftWheelCollider.motorTorque.ToString());
    }



    public void ChangeControlMode()
    {
        if (Input.GetKeyDown("tab"))
        {
            if (controlMode == 2)
            {
                controlMode = 0;
            }
            else
            {
                controlMode++;
            }
        }
    }




    float mainSpeed = 10f; // Regular speed.
    float shiftAdd = 25f;  // Multiplied by how long shift is held.  Basically running.
    float maxShift = 100f; // Maximum speed when holding shift.
    float camSens = .35f;  // Camera sensitivity by mouse input.
    private Vector3 lastMouse = new Vector3(Screen.width / 2, Screen.height / 2, 0); // Kind of in the middle of the screen, rather than at the top (play).
    private float totalRun = 1.0f;


    public float speed = 1;

    private void FlyControlMode()
    {


        //// Mouse input.
        //lastMouse = Input.mousePosition - lastMouse;
        //lastMouse = new Vector3(-lastMouse.y * camSens, lastMouse.x * camSens, 0);
        //lastMouse = new Vector3(transform.eulerAngles.x + lastMouse.x, transform.eulerAngles.y + lastMouse.y, 0);
        //transform.eulerAngles = lastMouse;
        //lastMouse = Input.mousePosition;

        // Keyboard commands.
        Vector3 p = getDirection();
        if (Input.GetKey(KeyCode.LeftShift))
        {
            totalRun += Time.deltaTime;
            p = p * totalRun * shiftAdd;
            p.x = Mathf.Clamp(p.x, -maxShift, maxShift);
            p.y = Mathf.Clamp(p.y, -maxShift, maxShift);
            p.z = Mathf.Clamp(p.z, -maxShift, maxShift);
        }
        else
        {
            totalRun = Mathf.Clamp(totalRun * 0.5f, 1f, 1000f);
            p = p * mainSpeed;
        }

        p = p * Time.deltaTime;
        Vector3 newPosition = transform.position;
        if (Input.GetKey(KeyCode.V))
        { //If player wants to move on X and Z axis only
            transform.Translate(p);
            newPosition.x = transform.position.x;
            newPosition.z = transform.position.z;
            transform.position = newPosition;
        }
        else
        {
            transform.Translate(p);
        }

        transform.forward = camera.transform.forward;

    }

    private Vector3 getDirection()
    {
        Vector3 p_Velocity = new Vector3();
        if (Input.GetKey(KeyCode.W))
        {
            p_Velocity += new Vector3(0, 0, 1);
        }
        if (Input.GetKey(KeyCode.S))
        {
            p_Velocity += new Vector3(0, 0, -1);
        }
        if (Input.GetKey(KeyCode.A))
        {
            p_Velocity += new Vector3(-1, 0, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            p_Velocity += new Vector3(1, 0, 0);
        }
        if (Input.GetKey(KeyCode.R))
        {
            p_Velocity += new Vector3(0, 1, 0);
        }
        if (Input.GetKey(KeyCode.F))
        {
            p_Velocity += new Vector3(0, -1, 0);
        }
        return p_Velocity;
    }

    public void resetRotation(Vector3 lookAt)
    {
        transform.LookAt(lookAt);
    }





    private void GetInput()
        {
            horizontalInput = Input.GetAxis(HORIZONTAL);
            verticalInput = Input.GetAxis(VERTICAL);
            isBreaking = Input.GetKey(KeyCode.Space);
        }


    public void CheckTurboTime()
    {
        if (!isNormal && isTurbo && ((Time.time - startTurboTime) > 5)) { 
            print("checking");
            SwitchNitroEffects();
            isTurbo = false;
            isNormal = true; 
        }


    }

    public void ComputeTurboSpeed()
    {
        CheckTurboTime();
        if (isTurbo && !isNormal)
        {
            frontLeftWheelCollider.motorTorque = nitro * verticalInput * motorForce * speed;
            frontRightWheelCollider.motorTorque = nitro * verticalInput * motorForce * speed;
        }
    }


    public void SwitchNitroEffects()
    {
        var nitroEffectLight_emission = nitroEffect.light.emission;
        var nitroEffectCore_emission = nitroEffect.core.emission;
        var nitroEffectRing_emission = nitroEffect.ring.emission;
        if (isNormal && !isTurbo)
        {
            nitroEffectLight_emission.enabled = true;
            nitroEffectCore_emission.enabled = true;
            nitroEffectRing_emission.enabled = true;
        }
        else if (!isNormal && isTurbo)
        {
            nitroEffectLight_emission.enabled = false;
            nitroEffectCore_emission.enabled = false;
            nitroEffectRing_emission.enabled = false;
        }
    }

    public void TurboSpeedMode()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && isNormal)
        {
            SwitchNitroEffects();

            print("Turbo On");
            startTurboTime = Time.time;
            isNormal = false;
            isTurbo = true;
       
        }

        ComputeTurboSpeed();
        //print("1 " + frontLeftWheelCollider.motorTorque);
    }

    public void NormalSpeedMode()
    {
        if (isNormal && !isTurbo)
        {
            frontLeftWheelCollider.motorTorque = verticalInput * motorForce * speed;
            frontRightWheelCollider.motorTorque = verticalInput * motorForce * speed;

            rearLeftWheelCollider.motorTorque = verticalInput * motorForce;
            rearRightWheelCollider.motorTorque = verticalInput * motorForce;
        }
    }

    private void HandleMotor()
    {
        NormalSpeedMode();
        TurboSpeedMode();
       
        currentbreakForce = isBreaking ? breakForce : 0f;
        ApplyBreaking();
    }

    private void ApplyBreaking()
    {
        frontRightWheelCollider.brakeTorque = currentbreakForce;
        frontLeftWheelCollider.brakeTorque = currentbreakForce;
        rearLeftWheelCollider.brakeTorque = currentbreakForce;
        rearRightWheelCollider.brakeTorque = currentbreakForce;
    }

    private void HandleSteering()
    {
        currentSteerAngle = maxSteerAngle * horizontalInput;
        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;

        if (controlMode == 1) { 
            rearLeftWheelCollider.steerAngle = currentSteerAngle;
            rearRightWheelCollider.steerAngle = currentSteerAngle;
        }
        if (controlMode == 0)
        {
            rearLeftWheelCollider.steerAngle = 0;
            rearRightWheelCollider.steerAngle = 0;
        }
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheeTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot; 
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot;
        //wheelTransform.position = pos;
    }
}