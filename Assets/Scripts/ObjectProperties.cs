using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectProperties : MonoBehaviour
{
    [Tooltip("Whether the object can be stuck to or not.")]
    public bool surfaceStickable = true; // Whether the object can be stuck to or not.
    [Tooltip("Climbing assist, ONLY use if the object is stickable.")]
    public bool stickableClimbGravityHelp = true; // This is to seperate intended climbable elements from props.
    


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
