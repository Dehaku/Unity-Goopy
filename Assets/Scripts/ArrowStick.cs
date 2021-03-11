using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowStick : MonoBehaviour
{
    Rigidbody2D _rigidbody2D;
    bool hasStuck;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody2D = transform.parent.gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // _rigidbody2D.AddForce(new Vector2(0, 1000));

        if (hasStuck)
            return;

        _rigidbody2D.isKinematic = true;
        _rigidbody2D.velocity = new Vector2(0,0);
        _rigidbody2D.freezeRotation = true;

        _rigidbody2D.gameObject.transform.parent = collision.gameObject.transform;
        
        hasStuck = true;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // _rigidbody2D.AddForce(new Vector2(0, 1000));

        if (hasStuck)
            return;

        _rigidbody2D.isKinematic = true;
        _rigidbody2D.velocity = new Vector2(0, 0);
        _rigidbody2D.freezeRotation = true;

        _rigidbody2D.gameObject.transform.parent = collision.gameObject.transform;

        hasStuck = true;
    }

}
