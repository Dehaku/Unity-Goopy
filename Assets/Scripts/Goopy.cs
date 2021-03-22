using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goopy : MonoBehaviour
{
    public GameObject goopyPrefab;

    Rigidbody2D _rigidbody2D;

    [SerializeField] int _splitCount = 6;
    [SerializeField] float _splitRadius = 2f;

    void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        FindObjectOfType<Cinemachine.CinemachineTargetGroup>().AddMember(gameObject.transform, 1f, 2f);
    }

    // Start is called before the first frame update
    void Start()
    {
        goopyPrefab = (GameObject)Resources.Load("Goopy");
        AttachToAllOthers();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z)) // Debug purposes, don't depend on this.
            _rigidbody2D.AddForce(new Vector2(0, 500));

        if (Input.GetKeyDown(KeyCode.Space) && Input.GetKey(KeyCode.LeftControl))
            SplitIntoMiniChildren();

        /*
        if (Input.GetKeyDown(KeyCode.M))
            SetGoopySpringFrequency(0.001f);
        if (Input.GetKeyDown(KeyCode.N))
            SetGoopySpringFrequency(1f);
        if (Input.GetKeyDown(KeyCode.X))
            UpdateGoopySpringFromSerialize();
        */
    }

    void SpawnMiniChild()
    {
        GameObject childSpawn = Instantiate(goopyPrefab, new Vector3(_rigidbody2D.position.x,
                    _rigidbody2D.position.y + 10),
                    Quaternion.identity);
        childSpawn.gameObject.transform.localScale = new Vector3(0.25f,0.25f,0.25f);
    }

    public void SplitIntoMiniChildren()
    { 
        for (int i =0; i < _splitCount; i++)
        {
            float angle = i * Mathf.PI * 2 / _splitCount;
            float x = Mathf.Cos(angle) * _splitRadius;
            float y = Mathf.Sin(angle) * _splitRadius;
            Vector3 pos = transform.position + new Vector3(x, y);
            
            var oldParent = gameObject.transform.parent;
            Destroy(gameObject);

            GameObject childSpawn = Instantiate(goopyPrefab, pos, Quaternion.identity);
            childSpawn.gameObject.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            childSpawn.gameObject.transform.parent = oldParent;
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        bool isSticky = gameObject.transform.parent.GetComponent<GoopyController>().stickyMode;

        if (!isSticky)
            return;

        Goopy goopy = collision.collider.GetComponent<Goopy>();
        if (goopy != null)
            return;

        ObjectProperties stickiness = collision.collider.GetComponent<ObjectProperties>();
        if (stickiness != null) // By default, everything is stickable, so to prevent ObjectProperty spam, we don't need it for grabbability.
            if (!stickiness.surfaceStickable)
                return;



        float stickyBreakforce = gameObject.transform.parent.GetComponent<GoopyController>().stickyBreakForce;
        float stickyFrequency = gameObject.transform.parent.GetComponent<GoopyController>().stickyFrequency;
        float stickyDampening = gameObject.transform.parent.GetComponent<GoopyController>().stickyDampening;

        var springs = gameObject.GetComponents<SpringJoint2D>();
        bool alreadySticky = false;
        foreach (var spring in springs)
        {
            if (spring.breakForce == stickyBreakforce)
            {
                alreadySticky = true;
                break;
            }
            
        }
        if (alreadySticky)
            return;

        SpringJoint2D springJoint2D = gameObject.AddComponent<SpringJoint2D>();
        springJoint2D.enableCollision = true;
        springJoint2D.connectedBody = collision.collider.gameObject.GetComponent<Rigidbody2D>();
        springJoint2D.breakForce = stickyBreakforce;
        springJoint2D.frequency = stickyFrequency;
        springJoint2D.dampingRatio = stickyDampening;
        springJoint2D.connectedAnchor = collision.contacts[0].point;
        springJoint2D.autoConfigureConnectedAnchor = true;
        // springJoint2D.
    }


    public void SetGoopySpringFrequency(float frequency)
    {
        var _goops = FindObjectsOfType<Goopy>();
        foreach (var goop in _goops)
        {
            var springJoint2DCollection = goop.gameObject.GetComponents<SpringJoint2D>();
            foreach (var springJoint2D in springJoint2DCollection)
            {
                springJoint2D.frequency = frequency;
            }

        }
    }

    void UpdateGoopySpringFromSerialize()
    {
        var _goops = FindObjectsOfType<Goopy>();
        var _goopyController = FindObjectOfType<GoopyController>();
        foreach (var goop in _goops)
        {
            var springJoint2DCollection = goop.gameObject.GetComponents<SpringJoint2D>();
            foreach (var springJoint2D in springJoint2DCollection)
            {
                springJoint2D.frequency = _goopyController.goopSpringFrequency;
                // springJoint2D.distance = _goopyController.goopSpringDistance;
                springJoint2D.dampingRatio = _goopyController.goopSpringDampeningRatio;
            }

        }
    }

    

    void AttachToAllOthers()
    {
        var _goops = FindObjectsOfType<Goopy>();

        if (_goops.Length <= 1)
            return;

        foreach (var goop in _goops)
        {
            if (goop.gameObject == gameObject)
                continue;
            SpringJoint2D springJoint2D = gameObject.AddComponent<SpringJoint2D>();
            springJoint2D.enableCollision = true;
            // springJoint2D.gameObject = goop.gameObject;
            springJoint2D.connectedBody = goop.gameObject.GetComponent<Rigidbody2D>();
            // springJoint2D.dampingRatio = 1f;
            springJoint2D.frequency = 1f;

            // goop.gameObject.AddComponent<SpringJoint2D>();

        }
    }


}
