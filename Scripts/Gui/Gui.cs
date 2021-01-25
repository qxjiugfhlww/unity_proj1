using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gui : MonoBehaviour
{
    public Image Panel;
    public Text Speed_text;
    public CarController carController;
    // Start is called before the first frame update
    void Start()
    {
        carController = FindObjectOfType<CarController>();
    }

    // Update is called once per frame
    void Update()
    {

        ChangeSpeedText(carController.currentSpeed.ToString());
    }


    public void ChangeSpeedText(string text)
    {
        Speed_text.text = text;
    }

}
