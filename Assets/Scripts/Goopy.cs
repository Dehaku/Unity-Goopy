using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goopy : MonoBehaviour
{
    public GameObject goopyPrefab;

    Rigidbody2D _rigidbody2D;

    [SerializeField] float _movementForce = 5;
    [SerializeField] float _jumpForce = 500;
    [SerializeField] int _splitCount = 6;
    [SerializeField] float _splitRadius = 5f;

    void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        
    }

    // Start is called before the first frame update
    void Start()
    {
        goopyPrefab = (GameObject)Resources.Load("Goopy");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
            _rigidbody2D.AddForce(new Vector2(-_movementForce, 0));
        if (Input.GetKey(KeyCode.RightArrow))
            _rigidbody2D.AddForce(new Vector2(_movementForce, 0));
        if (Input.GetKeyDown(KeyCode.Space) && 
            !Input.GetKey(KeyCode.LeftShift) &&
            !Input.GetKey(KeyCode.LeftControl))
            _rigidbody2D.AddForce(new Vector2(0, _jumpForce));
        if (Input.GetKeyDown(KeyCode.Space) && Input.GetKey(KeyCode.LeftShift))
            SpawnMiniChild();

        if (Input.GetKeyDown(KeyCode.Space) && Input.GetKey(KeyCode.LeftControl))
            SplitIntoMiniChildren();


    }

    void OnMouseDown()
    {
        GameObject childSpawn = Instantiate(goopyPrefab, new Vector3(_rigidbody2D.position.x,
                    _rigidbody2D.position.y + 10),
                    Quaternion.identity);
        childSpawn.gameObject.transform.localScale = transform.localScale;

        // gameObject.SetActive(false);
        Destroy(gameObject);
    }
    void SpawnMiniChild()
    {
        GameObject childSpawn = Instantiate(goopyPrefab, new Vector3(_rigidbody2D.position.x,
                    _rigidbody2D.position.y + 10),
                    Quaternion.identity);
        childSpawn.gameObject.transform.localScale = new Vector3(0.25f,0.25f,0.25f);
    }

    void SplitIntoMiniChildren()
    { 
        for (int i =0; i < _splitCount; i++)
        {
            float angle = i * Mathf.PI * 2 / _splitCount;
            float x = Mathf.Cos(angle) * _splitRadius;
            float y = Mathf.Sin(angle) * _splitRadius;
            Vector3 pos = transform.position + new Vector3(x, y);
            // float angleDegrees = -angle * Mathf.Rad2Deg;
            // Quaternion rot = Quaternion.Euler(0, angleDegrees, 0);
            GameObject childSpawn = Instantiate(goopyPrefab, pos, Quaternion.identity);
            childSpawn.gameObject.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
        }
    }


}
