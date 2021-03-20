using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoopyController : MonoBehaviour
{
    [SerializeField] float _defaultGravity = 1;
    [SerializeField] float _hangGravity = 0.25f;
    [SerializeField] float _movementForce = 5;
    public float goopJumpForce = 500;
    public float goopShootForce = 500;
    public float goopSpringDistance = 2;
    public float goopSpringDampeningRatio = 0;
    public float goopSpringFrequency = 1;
    public float goopFriction = 1;
    [SerializeField] GameObject centerGoop;
    [SerializeField] GameObject goopyFace;
    [SerializeField] GameObject _goopyStickyArrow;
    [SerializeField] float _outerParachuteDrag = 1.5f;
    [SerializeField] float _innerParachuteDrag = 3f;

    public bool isInVent;
    public float ventPower;
    public Vector2 ventDirection;
    public bool stickyMode;
    public float stickyBreakForce = 20;
    public float stickyFrequency = 1;
    public float stickyDampening = 0;
    public Vector2[] OrdinalDir = new[] {
            new Vector2(0,1),  // N
            new Vector2(1,1),  // NE
            new Vector2(1,0),  // E
            new Vector2(1,-1), // SE
            new Vector2(0,-1), // S
            new Vector2(-1,-1),// SW
            new Vector2(-1,0), // W
            new Vector2(-1,1), // NW
            new Vector2(0,0)   // C
        };

    Vector3 _centerOfGoops;
    Rigidbody2D[] _goops;
    Vector2 _goopsVelocity;
    bool _pullYourselfTogether;
    bool _brokenApart;
    bool _parachuteMode;
    Vector2 _directionNearest;
    Vector2 _directionNearestAverage;
    bool _SurfaceN, _SurfaceNE, _SurfaceE, _SurfaceSE, _SurfaceS, _SurfaceSW, _SurfaceW, _SurfaceNW;
    Vector2 _desiredFaceDirection;
    Vector2 _currentFaceDirection;






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
        {
            goop.GetComponent<Rigidbody2D>().AddForce(moveDirection);
            // Check angle of movement, magnitude, and desired move direction before allowing GoopyMove to add speed.
            /*
            Rigidbody2D _rigidbody2D = goop.GetComponent<Rigidbody2D>();
            if (_rigidbody2D.velocity.magnitude > 5)
                continue;
            */
        }
            
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

    void BreakStickyMode()
    {
        foreach (var goop in _goops)
        {
            var springJoint2DCollection = goop.gameObject.GetComponents<SpringJoint2D>();
            foreach (var springJoint2D in springJoint2DCollection)
            {
                if (springJoint2D.breakForce == stickyBreakForce)
                {
                    Destroy(springJoint2D);
                }
            }
        }
    }

    void Spin(bool clockwise = false)
    {
        foreach (var goop in _goops)
        {
            Rigidbody2D _rigidbody2D = goop.GetComponent<Rigidbody2D>();
            if (_rigidbody2D.velocity.magnitude > 5)
                continue;

            var goopv2 = _rigidbody2D.position - new Vector2(_centerOfGoops.x, _centerOfGoops.y);
            var goopAngle = Mathf.Atan2(goopv2.y, goopv2.x);
            float WeirdAngle = Vector2.Angle(_centerOfGoops, _rigidbody2D.position);
            Vector2 direction = new Vector2(_centerOfGoops.x, _centerOfGoops.y) - _rigidbody2D.position;
            Vector2 perpDirection = Vector2.Perpendicular(direction);
            perpDirection.Normalize();
           
            if (clockwise)
                _rigidbody2D.AddForce(perpDirection * 5);
            else
                _rigidbody2D.AddForce(-perpDirection * 5);
        }
    }

    void NearestSurfaceLogic()
    {
        RaycastHit2D[] raycastHit2D = new RaycastHit2D[10];
        ContactFilter2D contactFilter2D = new ContactFilter2D();
        contactFilter2D.NoFilter();
        //contactFilter2D.SetLayerMask(LayerMask.NameToLayer("Wall"));
        float castRange = 1f;
        Vector2 averageAngle = new Vector2(0,0);

        _SurfaceN = false;
        _SurfaceNE = false;
        _SurfaceE = false;
        _SurfaceSE = false;
        _SurfaceS = false;
        _SurfaceSW = false;
        _SurfaceW = false;
        _SurfaceNW = false;

        foreach (var dir in OrdinalDir)
        {
            bool hitOtherThanGoopy = false;
            bool angleOnce = false;
            raycastHit2D = new RaycastHit2D[10];
            Physics2D.Raycast(_centerOfGoops, dir, contactFilter2D, raycastHit2D, castRange);

            foreach (var hit in raycastHit2D)
            {
                if (hit.collider == null || hit.collider.name.StartsWith("Goopy"))
                    continue;
                hitOtherThanGoopy = true;
                if(!angleOnce)
                {
                    averageAngle += dir;
                    angleOnce = true;
                }
            }

            if(hitOtherThanGoopy)
            {
                if (dir == new Vector2(0, 1)) // N
                    _SurfaceN = true;
                if (dir == new Vector2(1, 1)) // NE
                    _SurfaceNE = true;
                if (dir == new Vector2(1, 0)) // E
                    _SurfaceE = true;
                if (dir == new Vector2(1, -1)) // SE
                    _SurfaceSE = true;
                if (dir == new Vector2(0, -1)) // S
                    _SurfaceS = true;
                if (dir == new Vector2(-1, -1)) // SW
                    _SurfaceSW = true;
                if (dir == new Vector2(-1, 0)) // W
                    _SurfaceW = true;
                if (dir == new Vector2(-1, 1)) // NW
                    _SurfaceNW = true;
            }

            if (hitOtherThanGoopy)
                Debug.DrawRay(_centerOfGoops, dir * castRange, Color.green);
            else
                Debug.DrawRay(_centerOfGoops, dir * castRange, Color.red);
        }

        Debug.DrawRay(_centerOfGoops, averageAngle * castRange * 2, Color.blue);
        averageAngle.Normalize();
        Debug.DrawRay(_centerOfGoops, averageAngle * castRange * 2, Color.gray);
        _directionNearestAverage = averageAngle;
    }

    void GoopyFaceLogic()
    {
        _currentFaceDirection.Normalize();
        _currentFaceDirection = _currentFaceDirection + (_desiredFaceDirection / 8);
        if (_currentFaceDirection.x < 0)
        {
            goopyFace.GetComponent<SpriteRenderer>().flipX = true;
        }
        else
            goopyFace.GetComponent<SpriteRenderer>().flipX = false;

        goopyFace.transform.up = _currentFaceDirection;
        if (!_brokenApart)
        {
            goopyFace.transform.position = new Vector3(_centerOfGoops.x, _centerOfGoops.y, -10);
            _desiredFaceDirection = -_directionNearestAverage;

            Vector2 goopVelo = _goops[0].velocity;
            goopVelo.Normalize();
            goopVelo = Vector2.Perpendicular(goopVelo);
            _desiredFaceDirection += goopVelo * 0.1f;

            if(_directionNearestAverage == new Vector2(0,0))
            {
                if (_desiredFaceDirection.y < 0)
                    _desiredFaceDirection.y = -_desiredFaceDirection.y;
            }
        }
        else
            goopyFace.transform.position = new Vector3(-100000, 0);
    }

    void GoopyGravity(float newGravity)
    {
        foreach (var goop in _goops)
            goop.GetComponent<Rigidbody2D>().gravityScale = newGravity;
    }

    void HangGravity()
    {
        if (_SurfaceN && !_SurfaceS)
            GoopyGravity(_hangGravity);
        else
            GoopyGravity(_defaultGravity);

    }

    // Update is called once per frame
    void Update()
    {
        _goops = GetComponentsInChildren<Rigidbody2D>(); // It is CRITICAL that this goes first. I have no idea why null checks don't work before this is ran once. 2021-3-13
        if (_goops.Length == 1)
            _goops[0].GetComponent<Goopy>().SplitIntoMiniChildren();


        CalcCenterOfGoops();
        GoopyFaceLogic();
        NearestSurfaceLogic();
        HangGravity();

        if (Input.GetKey(KeyCode.A))
        {
            GoopyMove(new Vector2(-_movementForce/4, 0));
            if (!_brokenApart)
            {
                if (_SurfaceN || _SurfaceNW || _SurfaceNE)
                {
                    Spin(true);
                }
                if (_SurfaceS || _SurfaceSW || _SurfaceSE)
                {
                    Spin(false);
                }

                /*
                if(_SurfaceN)
                {
                    Spin(true);
                }
                else if(_SurfaceS)
                {
                    Spin(false);
                }
                */

                //if (_directionNearestAverage.y > 0)
                //    Spin(true);
                //else
                //    Spin(false);
            }
                
        }
            
        if (Input.GetKey(KeyCode.D))
        {
            GoopyMove(new Vector2(_movementForce / 4, 0));
            
            if(!_brokenApart)
            {
                if (_SurfaceN || _SurfaceNW || _SurfaceNE)
                {
                    Spin(false);
                }
                if (_SurfaceS || _SurfaceSW || _SurfaceSE)
                {
                    Spin(true);
                }

                //if (_directionNearestAverage.y > 0)
                //    Spin(false);
                //else
                //    Spin(true);
            }
                
        }
            
        if (Input.GetKey(KeyCode.W) && _directionNearestAverage != new Vector2())
        {
            GoopyMove(new Vector2(0, _movementForce / 4));

            if (!_brokenApart)
            {
                if (_SurfaceE || _SurfaceNE || _SurfaceSE)
                {
                    Spin(true);
                }
                if (_SurfaceW || _SurfaceNW || _SurfaceSW)
                {
                    Spin(false);
                }
                //if (_directionNearestAverage.x > 0)
                //    Spin(true);
                //else
                //    Spin(false);
            }
                
        }
            
        if (Input.GetKey(KeyCode.S))
        {

            GoopyMove(new Vector2(0, -_movementForce));

            if (!_brokenApart)
            {
                if (_SurfaceE || _SurfaceNE || _SurfaceSE)
                {
                    Spin(false);
                }
                if (_SurfaceW || _SurfaceNW || _SurfaceSW)
                {
                    Spin(true);
                }
                //if (_directionNearestAverage.x > 0)
                //    Spin(false);
                //else
                //    Spin(true);
            }
            
        }
            

        /*
        
        if (Input.GetKeyDown(KeyCode.Space) && !_brokenApart)
            GoopyJump();
        if(Input.GetKeyDown(KeyCode.Space) && Input.GetKey(KeyCode.LeftShift)) 
            GoopyJump(); // For debug/display purposes while settings are adjusted.

        if (Input.GetMouseButtonDown(0))
            FireGoopyStickyArrow();

        */

        /*
        
        if (Input.GetKey(KeyCode.S) && !_brokenApart)
            SetGoopyFrequency(0.25f);
        if (Input.GetKeyUp(KeyCode.S) && !_brokenApart)
            SetGoopyFrequency(goopSpringFrequency);

        */

        if (Input.GetKey(KeyCode.Space) && !_brokenApart)
            SetGoopyFrequency(10f);
        if (Input.GetKeyUp(KeyCode.Space) && !_brokenApart)
            SetGoopyFrequency(goopSpringFrequency);

        if (Input.GetKey(KeyCode.F) && !_brokenApart)
            GoopyParachute(); //_parachuteMode = !_parachuteMode;
        if (Input.GetKeyUp(KeyCode.F) && !_brokenApart)
            GoopyParachute(true);

        VentLogic();

        if (Input.GetKey(KeyCode.LeftShift))
            stickyMode = true;
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            stickyMode = false;
            BreakStickyMode();
        }

        if (Input.GetKey(KeyCode.LeftArrow))
            Spin(false);
        if (Input.GetKey(KeyCode.RightArrow))
            Spin(true);


        //else if (!_brokenApart)

        _goopsVelocity = _goops[0].gameObject.GetComponent<Rigidbody2D>().velocity;





        if (Input.GetKeyDown(KeyCode.F) && Input.GetKey(KeyCode.LeftShift) && !_brokenApart)
            GoopyParachute(true); //_parachuteMode = !_parachuteMode;




        if (Input.GetKey(KeyCode.Q))
            BreakYourselfApart();
        if (Input.GetKey(KeyCode.E))
            PullYourselfTogether(true);    // Overrides need for the bool, allow precise control of pulling forces.
        if (Input.GetKey(KeyCode.R))       // Automatically pull yourself together.
            _pullYourselfTogether = true;
        PullYourselfTogether();

        
        
        


        if (_goops.Length > 0)
        {
            centerGoop.transform.position = _centerOfGoops;
        }
    }

    private void OnGUI()
    {
        if(_goopsVelocity.magnitude > 88)
            GUI.Label(new Rect(25, 5, 260, 40), "Are you trying to go to a parallel world?");
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
