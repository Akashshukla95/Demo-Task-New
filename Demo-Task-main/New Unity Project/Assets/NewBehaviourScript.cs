using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
	public float moveSpeed;
	public GameObject WinText;
    // Start is called before the first frame update
    void Start()
    {
		moveSpeed = 4f;
    }

    // Update is called once per frame
    void Update()
    {
		transform.Translate(moveSpeed * Input.GetAxis("Horizontal") * Time.deltaTime, 0f, moveSpeed * Input.GetAxis("Vertical") * Time.deltaTime);
    }
}
