using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoopyController : MonoBehaviour
{
    [SerializeField] public float goopSpringDistance = 2;
    [SerializeField] public float goopSpringDampeningRatio = 0;
    [SerializeField] public float goopSpringFrequency = 1;
    [SerializeField] public float goopFriction = 1;
    [SerializeField] GameObject centerGoop;

    Vector3 _centerOfGoops;
    Rigidbody2D[] _goops;

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void CalcCenterOfGoops()
    {
        _goops = GetComponentsInChildren<Rigidbody2D>();

        if (_goops.Length == 0)
        {
            _centerOfGoops = new Vector3();
            return;
        }
            
        foreach (var goop in _goops)
            _centerOfGoops += goop.transform.position;

        _centerOfGoops /= _goops.Length + 1;
    }

    void GravityGoops()
    {
        foreach (var goop in _goops)
        {
            var _rigidbody2D = goop.GetComponent<Rigidbody2D>();
            var currentPosition = _rigidbody2D.position;
            Vector2 direction = new Vector2(_centerOfGoops.x,_centerOfGoops.y) - currentPosition;
            direction.Normalize();

            _rigidbody2D.AddForce(direction * 5);
        }
    }

    // Update is called once per frame
    void Update()
    {
        CalcCenterOfGoops();

        if (Input.GetKey(KeyCode.Q))
            GravityGoops();

        if(_goops.Length > 0)
        {
            centerGoop.transform.position = _centerOfGoops;
        }
    }

}
