using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public partial class Tracker : MonoBehaviour
{
    public GameObject[] turrets;

    public virtual void Update()
    {
        foreach (GameObject turret in (this.turrets as GameObject[]))
            turret.SendMessage("Target", this.transform.position);
    }

}
