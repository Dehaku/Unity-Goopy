using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GoopyCamera : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        object _targets = FindObjectOfType<Cinemachine.CinemachineTargetGroup>(); // .AddMember
                                                                                  // _targets.add



    }
}
