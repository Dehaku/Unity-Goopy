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
    bool _pullYourselfTogether;

    
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

    float GetFarthestGoopDistance()
    {
        float FarthestDistance = 0;
        foreach (var goop in _goops)
        {
            float distanceBetween = Vector2.Distance(goop.transform.position, _centerOfGoops);
            if (distanceBetween > FarthestDistance)
                FarthestDistance = distanceBetween;
        }
        Debug.Log(FarthestDistance);
        return FarthestDistance;
    }

    void BreakYourselfApart()
    {
        foreach (var goop in _goops)
            goop.GetComponent<Goopy>().SetGoopySpringFrequency(0.001f);
    }

    void PullYourselfTogether()
    {
        // if (_pullYourselfTogether)
        {
            GravityGoops();

            if (GetFarthestGoopDistance() <= 1)
            {
                foreach (var goop in _goops)
                    goop.GetComponent<Goopy>().SetGoopySpringFrequency(1f);
                _pullYourselfTogether = false;
            }


        }
    }

    // Update is called once per frame
    void Update()
    {
        CalcCenterOfGoops();

        if (Input.GetKey(KeyCode.Q))
            BreakYourselfApart();        // GravityGoops();
        if (Input.GetKey(KeyCode.E))
            PullYourselfTogether();    // _pullYourselfTogether = true;




        if (_goops.Length > 0)
        {
            centerGoop.transform.position = _centerOfGoops;
        }
    }

}
