using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoopyController : MonoBehaviour
{
    [SerializeField] public float goopJumpForce = 500;
    [SerializeField] public float goopShootForce = 500;
    [SerializeField] public float goopSpringDistance = 2;
    [SerializeField] public float goopSpringDampeningRatio = 0;
    [SerializeField] public float goopSpringFrequency = 1;
    [SerializeField] public float goopFriction = 1;
    [SerializeField] GameObject centerGoop;
    [SerializeField] GameObject _goopyStickyArrow;

    Vector3 _centerOfGoops;
    Rigidbody2D[] _goops;
    bool _pullYourselfTogether;
    bool _brokenApart;
    


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
        return FarthestDistance;
    }

    void BreakYourselfApart()
    {
        _brokenApart = true;
        foreach (var goop in _goops)
            goop.GetComponent<Goopy>().SetGoopySpringFrequency(0.001f);
    }

    void PullYourselfTogether(bool autoOverride = false)
    {
        
        if (_pullYourselfTogether || autoOverride)
        {
            GravityGoops();

            if (GetFarthestGoopDistance() <= 1)
            {
                foreach (var goop in _goops)
                    goop.GetComponent<Goopy>().SetGoopySpringFrequency(1f);
                _pullYourselfTogether = false;
                _brokenApart = false;
            }
        }
    }

    void GoopyJump()
    {
        foreach (var goop in _goops)
            goop.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, goopJumpForce));
    }

    // Update is called once per frame
    void Update()
    {
        CalcCenterOfGoops();

        if (Input.GetKey(KeyCode.Q))
            BreakYourselfApart();
        if (Input.GetKey(KeyCode.E))
            PullYourselfTogether(true);    // Overrides need for the bool, allow precise control of pulling forces.
        if (Input.GetKey(KeyCode.R))       // Automatically pull yourself together.
            _pullYourselfTogether = true;
        PullYourselfTogether();

        if (Input.GetKeyDown(KeyCode.Space) && !_brokenApart)
            GoopyJump();
        if(Input.GetKeyDown(KeyCode.Space) && Input.GetKey(KeyCode.LeftShift)) 
            GoopyJump(); // For debug/display purposes while settings are adjusted.

        if (Input.GetMouseButtonDown(0))
            FireGoopyStickyArrow();
        


        if (_goops.Length > 0)
        {
            centerGoop.transform.position = _centerOfGoops;
        }
    }

    void OnMouseDown()
    {
        FireGoopyStickyArrow();
    }

    void FireGoopyStickyArrow()
    {
        Debug.Log("splew");

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePosition - _centerOfGoops;
        direction.Normalize();
        Vector2 shootAngle = new Vector2(_centerOfGoops.x, _centerOfGoops.y) + (direction * 2);

        GameObject childSpawn = Instantiate(_goopyStickyArrow, new Vector3(shootAngle.x,
                    shootAngle.y),
                    Quaternion.identity);
        childSpawn.transform.up = direction;

        childSpawn.GetComponent<Rigidbody2D>().AddForce(direction * goopShootForce);


    }

}
