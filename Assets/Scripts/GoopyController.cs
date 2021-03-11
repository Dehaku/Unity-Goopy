using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoopyController : MonoBehaviour
{
    [SerializeField] public float goopSpringDistance = 2;
    [SerializeField] public float goopSpringDampeningRatio = 0;
    [SerializeField] public float goopSpringFrequency = 1;
    [SerializeField] public float goopFriction = 1;

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTransformParentChanged()
    {
        
    }

}
