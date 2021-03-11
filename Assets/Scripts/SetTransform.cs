using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This exists because I do not yet know how to make the effect camera follow the other cameras.

public class SetTransform : MonoBehaviour
{
    [SerializeField] GameObject _linkedGameObject;
    [SerializeField] bool _customZ;
    [SerializeField] float _z;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_linkedGameObject == null)
            return;

            
        if (_customZ)
            gameObject.transform.position = new Vector3(_linkedGameObject.transform.position.x,
                _linkedGameObject.transform.position.y,
                _z);
        else 
            gameObject.transform.position = _linkedGameObject.transform.position;
    }
}
