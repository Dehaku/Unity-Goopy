using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TentacleGrappleProjectile : MonoBehaviour
{
    public GoopyController _goopyController;
    public GameObject _goopyThatFiredUs;

    // Start is called before the first frame update
    void Start()
    {
        _goopyController = FindObjectOfType<GoopyController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.GetComponent<Rigidbody2D>() != null && collision.gameObject.GetComponent<Goopy>() == null)
        {
            _goopyController.TentacleProjHit(collision,_goopyThatFiredUs);
            Destroy(gameObject);
        }
    }
}
