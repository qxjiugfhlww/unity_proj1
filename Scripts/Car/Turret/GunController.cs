using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    [Tooltip("Should turret rotate in the FixedUpdate rather than Update?")]
    public bool runRotationsInFixed = false;

    [Header("Objects")]
    [Tooltip("Transform used to provide the horizontal rotation of the turret.")]
    public Transform turretBase;
    [Tooltip("Transform used to provide the vertical rotation of the barrels. Must be a child of the TurretBase.")]
    public Transform turretBarrels;
    public Transform turretSpine;

    [Header("Rotation Limits")]
    [Tooltip("Turn rate of the turret's base and barrels in degrees per second.")]
    public float turnRate = 30.0f;
    [Tooltip("When true, turret rotates according to left/right traverse limits. When false, turret can rotate freely.")]
    public bool limitTraverse = false;
    [Tooltip("When traverse is limited, how many degrees to the left the turret can turn.")]
    [Range(0.0f, 180.0f)]
    public float leftTraverse = 60.0f;
    [Tooltip("When traverse is limited, how many degrees to the right the turret can turn.")]
    [Range(0.0f, 180.0f)]
    public float rightTraverse = 60.0f;
    [Tooltip("How far up the barrel(s) can rotate.")]
    [Range(0.0f, 90.0f)]
    public float elevation = 60.0f;
    [Tooltip("How far down the barrel(s) can rotate.")]
    [Range(0.0f, 90.0f)]
    public float depression = 5.0f;

    [Header("Utilities")]
    [Tooltip("Show the arcs that the turret can aim through.\n\nRed: Left/Right Traverse\nGreen: Elevation\nBlue: Depression")]
    public bool showArcs = false;
    [Tooltip("When game is running in editor, draws a debug ray to show where the turret is aiming.")]
    public bool showDebugRay = true;

    private Vector3 aimPoint;

    private bool aiming = true;
    private bool atRest = false;


    public LayerMask groundLayer = 1 << 0;

    public Transform cameraTransform;

    /// <summary>
    /// Turret is no longer aiming at anything, returns to resting position, and stops rotating.
    /// </summary>
    public bool Idle { get { return !aiming; } }

    /// <summary>
    /// Turret is idle and in a resting position.
    /// </summary>
    public bool AtRest { get { return atRest; } }

    private void Start()
    {

        
    }

    private void Update()
    {



        aimPoint = cameraTransform.TransformPoint(Vector3.forward * 100.0f);

        GetDistance();
        if (!runRotationsInFixed)
        {
            RotateTurret();
        }

        if (showDebugRay)
            DrawDebugRays();
    }

    private void FixedUpdate()
    {
        if (runRotationsInFixed)
        {
            RotateTurret();
        }
    }

    /// <summary>
    /// Give the turret a position to aim at. If not idle, it will rotate to aim at this point.
    /// </summary>
    public void SetAimpoint(Vector3 position)
    {
        aiming = true;
        aimPoint = position;
    }

    /// <summary>
    /// When idle, turret returns to resting position, will not track an aimpoint, and rotations stop updating.
    /// </summary>
    public void SetIdle(bool idle)
    {
        aiming = !idle;

        if (aiming)
            atRest = false;
    }

    /// <summary>
    /// Attempts to automatically assign the turretBase and turretBarrels transforms. Will search for a transform
    /// named "Base" for turretBase and a transform named "Barrels" for the turretBarrels.
    /// </summary>
    public void AutoPopulateBaseAndBarrels()
    {
        // Don't allow this while ingame.
        if (!Application.isPlaying)
        {
            turretBase = transform.Find("Base");
            if (turretBase != null)
                turretBarrels = turretBase.Find("Barrels");
        }
        else
        {
            Debug.LogWarning(name + ": Turret cannot auto-populate transforms while game is playing.");
        }
    }

    /// <summary>
    /// Sets the turretBase and turretBarrels transforms to null.
    /// </summary>
    public void ClearTransforms()
    {
        // Don't allow this while ingame.
        if (!Application.isPlaying)
        {
            turretBase = null;
            turretBarrels = null;
        }
        else
        {
            Debug.LogWarning(name + ": Turret cannot clear transforms while game is playing.");
        }
    }

    private void RotateTurret()
    {
        if (aiming)
        {
            RotateBase();
            RotateBarrels();
        }
        else if (!atRest)
        {
            atRest = RotateToIdle();
        }
    }

    private void RotateBase()
    {

        //turretBase.transform.rotation = Quaternion.Euler(turretBase.transform.eulerAngles.x, turretBase.transform.eulerAngles.y, cameraTransform.eulerAngles.y);
        //turretSpine.transform.rotation = Quaternion.Euler(turretSpine.transform.eulerAngles.x, turretSpine.transform.eulerAngles.y, turretSpine.transform.eulerAngles.z);


        //Vector3 relativePos = target.position - transform.position;

        /*

        // TODO: Turret needs to rotate the long way around if the aimpoint gets behind
        // it and traversal limits prevent it from taking the shortest rotation.
        if (turretBase != null)
        {
            // Note, the local conversion has to come from the parent.
            Vector3 localTargetPos = transform.InverseTransformPoint(aimPoint);
            localTargetPos.y = 0.0f;

            // Clamp target rotation by creating a limited rotation to the target.
            // Use different clamps depending if the target is to the left or right of the turret.
            Vector3 clampedLocalVec2Target = localTargetPos;
            if (limitTraverse)
            {
                if (localTargetPos.x >= 0.0f)
                    clampedLocalVec2Target = Vector3.RotateTowards(Vector3.forward, localTargetPos, Mathf.Deg2Rad * rightTraverse, float.MaxValue);
                else
                    clampedLocalVec2Target = Vector3.RotateTowards(Vector3.forward, localTargetPos, Mathf.Deg2Rad * leftTraverse, float.MaxValue);
            }

            // Create new rotation towards the target in local space.
            Quaternion rotationGoal = Quaternion.LookRotation(clampedLocalVec2Target);
            Quaternion newRotation = Quaternion.RotateTowards(turretBase.localRotation, rotationGoal, turnRate * Time.deltaTime);

            // Set the new rotation of the base.
            turretBase.localRotation = newRotation;
        }
        */
    }


    float heading = 0;
    private void RotateBarrels()
    {

        

        //Vector3 relativePos = target.position - transform.position;

        // the second argument, upwards, defaults to Vector3.up
        //Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
        //transform.rotation = rotation;


        var new_rot = Quaternion.Lerp(turretBarrels.transform.rotation, cameraTransform.rotation, Time.deltaTime * 2);

        //Vector3 dir = hit.point - transform.position;
        Vector3 dir = (cameraTransform.forward).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = lookRotation.eulerAngles;
        //turretBarrels.transform.rotation = Quaternion.Euler(rotation.x + 90, rotation.y, rotation.z);

        turretBarrels.transform.rotation = Quaternion.Lerp(turretBarrels.transform.rotation, Quaternion.Euler(rotation.x + 90, rotation.y, rotation.z), Time.deltaTime * 2);


        //Quaternion direction = Quaternion.LookRotation(movement);
        // WORK WRONG
        //turretBarrels.transform.rotation = Quaternion.Lerp(turretBarrels.transform.rotation, Quaternion.LookRotation(cameraTransform.forward), 2 * Time.deltaTime);

        //turretBarrels.transform.position = transform.position + cameraTransform.forward * Input.GetAxis("Mouse Y") * 2 + cameraTransform.right * Input.GetAxis("Mouse X") * 2;
        //turretBarrels.transform.rotation = new_rot;
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //print("ray " + new_rot);

        //turretBarrels.transform.LookAt(cameraTransform);




        
        //var quat =  Quaternion.Euler(cameraTransform.eulerAngles.x + 90, turretBarrels.transform.eulerAngles.y, turretBarrels.transform.eulerAngles.z);
        //print("quat" + quat);
        //turretBarrels.transform.rotation = quat;




        /*
        // TODO: A target position directly to the turret's right will cause the turret
        // to attempt to aim straight up. This looks silly and on slow moving turrets can
        // cause delays on targeting. This is why barrels have a boosted rotation speed.
        if (turretBase != null && turretBarrels != null)
        {
            // Note, the local conversion has to come from the parent.
            Vector3 localTargetPos = turretBase.InverseTransformPoint(aimPoint);
            localTargetPos.x = 0.0f;

            // Clamp target rotation by creating a limited rotation to the target.
            // Use different clamps depending if the target is above or below the turret.
            Vector3 clampedLocalVec2Target = localTargetPos;
            if (localTargetPos.y >= 0.0f)
                clampedLocalVec2Target = Vector3.RotateTowards(Vector3.forward, localTargetPos, Mathf.Deg2Rad * elevation, float.MaxValue);
            else
                clampedLocalVec2Target = Vector3.RotateTowards(Vector3.forward, localTargetPos, Mathf.Deg2Rad * depression, float.MaxValue);

            // Create new rotation towards the target in local space.
            Quaternion rotationGoal = Quaternion.LookRotation(clampedLocalVec2Target);
            Quaternion newRotation = Quaternion.RotateTowards(turretBarrels.localRotation, rotationGoal, 2.0f * turnRate * Time.deltaTime);

            // Set the new rotation of the barrels.
            turretBarrels.localRotation = newRotation;
        
        }*/
    }

    private void GetDistance()
    {



        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        //float distance = Vector3.Distance(hit.collider.transform.position, hit.collider.transform.position);


        int maskOfPlane = 1 << 8;
        //if (Physics.Raycast(ray, out hit, maskOfPlane))
        //if (!Physics.Raycast(ray, out hit, 100))
        if (Physics.Raycast(ray, out hit, 5.0F))
        {
            //one of coordiantes being always zero for aligned plane
            var position = hit.transform.position;//this is relative to 0,0,0
            var relativePosition = transform.position - position;
            //relative to a gameObject other
        }
        
    }


    private bool RotateToIdle()
    {
        bool baseFinished = false;
        bool barrelsFinished = false;

        if (turretBase != null)
        {
            Quaternion newRotation = Quaternion.RotateTowards(turretBase.localRotation, Quaternion.identity, turnRate * Time.deltaTime);
            turretBase.localRotation = newRotation;

            if (turretBase.localRotation == Quaternion.identity)
                baseFinished = true;
        }

        if (turretBarrels != null)
        {
            Quaternion newRotation = Quaternion.RotateTowards(turretBarrels.localRotation, Quaternion.identity, 2.0f * turnRate * Time.deltaTime);
            turretBarrels.localRotation = newRotation;

            if (turretBarrels.localRotation == Quaternion.identity)
                barrelsFinished = true;
        }

        return (baseFinished && barrelsFinished);
    }

    private void DrawDebugRays()
    {
        //Debug.DrawRay(cameraTransform.position, cameraTransform.forward * 100.0f);



        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        Physics.Raycast(ray, out hit, 10);

        //Vector3 dir = hit.point - transform.position;
        //Quaternion lookRotation = Quaternion.LookRotation(dir);
        //Vector3 rotation = lookRotation.eulerAngles;
        //TurretY.rotation = Quaternion.Euler(0f, rotation.y, 0f);


        Vector3 dir = (hit.point - cameraTransform.position).normalized;
        Debug.DrawLine(turretBase.position, turretBase.position + cameraTransform.forward * 10, Color.red);
        //if (turretBarrels != null)
        //    Debug.DrawRay(turretBarrels.position, turretBarrels.forward * 100.0f);
        //else if (turretBase != null)
        //    Debug.DrawRay(turretBase.position, turretBase.forward * 100.0f);
    }
}
