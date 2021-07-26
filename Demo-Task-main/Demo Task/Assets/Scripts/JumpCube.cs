using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//[RedquiredComponent(typeof(Rigidbody))]
public class JumpCube : MonoBehaviour
{
    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = transform.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            float jumpVelocity = 5f;
            rb.velocity = Vector3.up * jumpVelocity;
        }
    }
}
