using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressureVent : MonoBehaviour
{
    [SerializeField] public float ventPressure;
    [SerializeField] public Vector2 ventDirection;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void OnTriggerEnter2D(Collider2D collision)
    {

        GoopyController goopyController = FindObjectOfType<GoopyController>();

        goopyController.ventDirection = ventDirection;
        goopyController.ventPower = ventPressure;
        goopyController.isInVent = true;

        
        // PressureVent vent = collision.gameObject.GetComponent<PressureVent>();
        // _ventPower = vent.ventPressure;
        // _ventDirection = vent.ventDirection;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GoopyController goopyController = FindObjectOfType<GoopyController>();
        goopyController.isInVent = false;
    }

}
