using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoopyController : MonoBehaviour
{
    [SerializeField] float _movementForce = 5;
    [SerializeField] public float goopJumpForce = 500;
    [SerializeField] public float goopShootForce = 500;
    [SerializeField] public float goopSpringDistance = 2;
    [SerializeField] public float goopSpringDampeningRatio = 0;
    [SerializeField] public float goopSpringFrequency = 1;
    [SerializeField] public float goopFriction = 1;
    [SerializeField] GameObject centerGoop;
    [SerializeField] GameObject _goopyStickyArrow;
    [SerializeField] float _outerParachuteDrag = 1.5f;
    [SerializeField] float _innerParachuteDrag = 3f;

    public bool isInVent;
    public float ventPower;
    public Vector2 ventDirection;

    Vector3 _centerOfGoops;
    Rigidbody2D[] _goops;
    bool _pullYourselfTogether;
    bool _brokenApart;
    bool _parachuteMode;

    
    


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

    void SetGoopyFrequency(float frequency)
    {
        foreach (var goop in _goops)
            goop.GetComponent<Goopy>().SetGoopySpringFrequency(frequency);
    }

    void GoopyJump()
    {
        foreach (var goop in _goops)
            goop.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, goopJumpForce));
    }

    void GoopyMove(Vector2 moveDirection)
    {
        foreach (var goop in _goops)
            goop.GetComponent<Rigidbody2D>().AddForce(moveDirection);
    }

    void GoopyParachute(bool resetStuff = false)
    {
        // if (_parachuteMode)
        {
            var direction = _goops[0].gameObject.GetComponent<Rigidbody2D>().velocity;
            direction.Normalize();
            direction = (direction * 1) + new Vector2(_centerOfGoops.x, _centerOfGoops.y);

            var v2 = direction - new Vector2(_centerOfGoops.x,_centerOfGoops.y);
            var velocityAngle = Mathf.Atan2(v2.y, v2.x);

            foreach (var goop in _goops)
            {

                var goopv2 = goop.GetComponent<Rigidbody2D>().position - new Vector2(_centerOfGoops.x, _centerOfGoops.y);
                var goopAngle = Mathf.Atan2(goopv2.y, goopv2.x);

                if (goopAngle >= velocityAngle - (3.14 / 2) && goopAngle <= velocityAngle + (3.14 / 2))
                {
                    _parachuteMode = true;
                    goop.GetComponent<SpriteRenderer>().color = Color.red;
                    goop.GetComponent<Rigidbody2D>().drag = _outerParachuteDrag;
                    SetGoopyFrequency(0.4f);
                    if (goopAngle >= velocityAngle - (3.14 / 4) && goopAngle <= velocityAngle + (3.14 / 4))
                        goop.GetComponent<Rigidbody2D>().drag = _innerParachuteDrag;
                }
                else
                {
                    goop.GetComponent<SpriteRenderer>().color = Color.white;
                    goop.GetComponent<Rigidbody2D>().drag = 0f;
                    SetGoopyFrequency(1f);
                }

                if(resetStuff)
                {
                    goop.GetComponent<SpriteRenderer>().color = Color.white;
                    goop.GetComponent<Rigidbody2D>().drag = 0f;
                    SetGoopyFrequency(1f);
                    _parachuteMode = false;

                }
            }
        }
    }

    void OnGUI()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        var v2 = mousePosition - _centerOfGoops;
        var angle = Mathf.Atan2(v2.y, v2.x);

        //Output the angle found above
        GUI.Label(new Rect(25, 25, 200, 40), "Angle Between Objects" + angle);
    }

    void VentLogic()
    {
        if(_parachuteMode && isInVent)
        {
            foreach (var goop in _goops)
            {
                goop.AddForce(ventDirection * ventPower);
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        CalcCenterOfGoops();

        if (Input.GetKey(KeyCode.A))
            GoopyMove(new Vector2(-_movementForce, 0));
        if (Input.GetKey(KeyCode.D))
            GoopyMove(new Vector2(_movementForce, 0));

        if (Input.GetKey(KeyCode.S) && !_brokenApart)
            SetGoopyFrequency(0.25f);
        if (Input.GetKeyUp(KeyCode.S) && !_brokenApart)
            SetGoopyFrequency(goopSpringFrequency);

        if (Input.GetKey(KeyCode.W) && !_brokenApart)
            SetGoopyFrequency(10f);
        if (Input.GetKeyUp(KeyCode.W) && !_brokenApart)
            SetGoopyFrequency(goopSpringFrequency);

        if (Input.GetKey(KeyCode.F) && !_brokenApart)
            GoopyParachute(); //_parachuteMode = !_parachuteMode;
        if (Input.GetKeyUp(KeyCode.F) && !_brokenApart)
            GoopyParachute(true);

        VentLogic();
        //else if (!_brokenApart)
            
        
        
        
        
        
        
        if (Input.GetKeyDown(KeyCode.F) && Input.GetKey(KeyCode.LeftShift) && !_brokenApart)
            GoopyParachute(true); //_parachuteMode = !_parachuteMode;




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
