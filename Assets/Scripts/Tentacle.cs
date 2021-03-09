using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Tentacle : MonoBehaviour
{
    public Sprite sprite;
    List<GameObject> _lines = new List<GameObject>();

    [SerializeField] int _amountOfLines = 10;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < _amountOfLines; i++)
        {
            _lines.Add(new GameObject("Tentacle Line" + i) );
            SpriteRenderer renderer = _lines.Last().AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.transform.localScale = new Vector3(0.25f, 1, 1);
            
            Rigidbody2D rigidbody2D = _lines.Last().AddComponent<Rigidbody2D>();
            rigidbody2D.gravityScale = 0;
            
            rigidbody2D.position += new Vector2(0, i*0.8f);
            BoxCollider2D boxCollider2D = _lines.Last().AddComponent<BoxCollider2D>();
            
            _lines.Last().AddComponent<RobotJoint>();


        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            foreach (GameObject go in _lines)
            {
                go.transform.position = new Vector3(go.transform.position.x, go.transform.position.y + 10, go.transform.position.z);
                // Rigidbody2D rigidbody2D = go.GetComponent<Rigidbody2D>();
                //rigidbody2D.position.x = 10;
            }
        }
    }

}
