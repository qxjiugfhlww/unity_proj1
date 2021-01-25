using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Lightbug.LaserMachine
{
    public class LaserMachine : MonoBehaviour {
        struct LaserElement 
        {
            public Transform transform;        
            public LineRenderer lineRenderer;
            public GameObject sparks;
            public bool impact;
        };

        LaserElement laserElement;

        public Transform cameraTransform;
    

        [Header("External Data")]
    
        [SerializeField] LaserData m_data;

        [Tooltip("This variable is true by default, all the inspector properties will be overridden.")]
        [SerializeField] bool m_overrideExternalProperties = true;

        [SerializeField] LaserProperties m_inspectorProperties = new LaserProperties();
    

        LaserProperties m_currentProperties;// = new LaserProperties();
        
        float m_time = 0;
        bool m_active = true;
        bool m_assignLaserMaterial;
        bool m_assignSparks;
  		
    

        void OnEnable()
        {
            m_currentProperties = m_overrideExternalProperties ? m_inspectorProperties : m_data.m_properties;
        

            m_currentProperties.m_initialTimingPhase = Mathf.Clamp01(m_currentProperties.m_initialTimingPhase);
            m_time = m_currentProperties.m_initialTimingPhase * m_currentProperties.m_intervalTime;
        

            float angleStep = m_currentProperties.m_angularRange / m_currentProperties.m_raysNumber;        

            m_assignSparks = m_data.m_laserSparks != null;
            m_assignLaserMaterial = m_data.m_laserMaterial != null;


            laserElement = new LaserElement();

            GameObject newObj = new GameObject("lineRenderer");

            if( m_currentProperties.m_physicsType == LaserProperties.PhysicsType.Physics2D )
                newObj.transform.position = (Vector2)transform.position;
            else
                newObj.transform.position = transform.position;

            newObj.transform.rotation = transform.rotation;
            //newObj.transform.Rotate( Vector3.up , i * angleStep );
            newObj.transform.position += newObj.transform.forward * m_currentProperties.m_minRadialDistance;

            newObj.AddComponent<LineRenderer>();

            if( m_assignLaserMaterial )
                newObj.GetComponent<LineRenderer>().material = m_data.m_laserMaterial;

            newObj.GetComponent<LineRenderer>().receiveShadows = false;
            newObj.GetComponent<LineRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            newObj.GetComponent<LineRenderer>().startWidth = m_currentProperties.m_rayWidth;
            newObj.GetComponent<LineRenderer>().useWorldSpace = true;
            newObj.GetComponent<LineRenderer>().SetPosition(0, newObj.transform.position);
            newObj.GetComponent<LineRenderer>().SetPosition(1, newObj.transform.position + transform.forward * m_currentProperties.m_maxRadialDistance);
            newObj.transform.SetParent(transform);
            
            if( m_assignSparks )
            {
                GameObject sparks = Instantiate(m_data.m_laserSparks);
                sparks.transform.SetParent(newObj.transform);
                sparks.SetActive(false);
                laserElement.sparks = sparks;
            }

            laserElement.transform = newObj.transform;
            laserElement.lineRenderer = newObj.GetComponent<LineRenderer>();
            laserElement.impact = false;

            Vector3 dir = (cameraTransform.forward).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            Vector3 rotation = lookRotation.eulerAngles;
                //turretBarrels.transform.rotation = Quaternion.Euler(rotation.x + 90, rotation.y, rotation.z);

            //laserElement.transform.rotation =  Quaternion.Euler(rotation.x, rotation.y, rotation.z);
            laserElement.transform.Rotate(-90.0f, 0.0f, 0.0f, Space.Self);


        }



        void Update () {

            if (m_currentProperties.m_intermittent)
            {
                m_time += Time.deltaTime;

                if (m_time >= m_currentProperties.m_intervalTime)
                {
                    m_active = !m_active;
                    m_time = 0;
                    return;
                }
            }

            RaycastHit hitInfo3D;


            //if ( m_currentProperties.m_rotate )
            //{
            //    if (m_currentProperties.m_rotateClockwise) { 
            //        laserElement.transform.RotateAround(transform.position, transform.up, Time.deltaTime * m_currentProperties.m_rotationSpeed);    //rotate around Global!!
            //    }
            //    else
            //        laserElement.transform.RotateAround(transform.position, transform.up, -Time.deltaTime * m_currentProperties.m_rotationSpeed);
            //}

            if (Input.GetKey(KeyCode.L))
                m_active = !m_active;

            if (m_active)
            {
                laserElement.lineRenderer.enabled = true;
               
                laserElement.lineRenderer.SetPosition(0, laserElement.transform.position);
                
                if (m_currentProperties.m_physicsType == LaserProperties.PhysicsType.Physics3D)
                {
                    Physics.Linecast(laserElement.transform.position, laserElement.transform.position + laserElement.transform.forward * m_currentProperties.m_maxRadialDistance,out hitInfo3D ,m_currentProperties.m_layerMask);  


                    if (hitInfo3D.collider)
                    {
                    
                        laserElement.lineRenderer.SetPosition(1, hitInfo3D.point);
                        if( m_assignSparks )
                        {
                            laserElement.sparks.transform.position = hitInfo3D.point; //new Vector3(rhit.point.x, rhit.point.y, transform.position.z);
                            laserElement.sparks.transform.rotation = Quaternion.LookRotation( hitInfo3D.normal ) ;
                        }

                            /*
                            EXAMPLE : In this line you can add whatever functionality you want, 
                            for example, if the hitInfoXD.collider is not null do whatever thing you wanna do to the target object.
                            DoAction();
                            */

                    }
                    else
                    {
                        //laserElement.lineRenderer.SetPosition(1, Vector3.Lerp(laserElement.transform.position, laserElement.transform.position + laserElement.transform.forward * m_currentProperties.m_maxRadialDistance, Time.deltaTime * 3));
                        laserElement.lineRenderer.SetPosition(1, laserElement.transform.position + laserElement.transform.forward * m_currentProperties.m_maxRadialDistance);

                    }

                    if( m_assignSparks )
                        laserElement.sparks.SetActive( hitInfo3D.collider != null );
                }
            }
            else
            {
                laserElement.lineRenderer.enabled = false;
                if( m_assignSparks )
                    laserElement.sparks.SetActive(false);
            }
        
        }

	
    }


}
