using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewBehaviour : MonoBehaviour
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
		if (Input.GetKeyDown(KeyCode.R))
		{
			SceneManager.LoadScene("Scean2");
		}
		transform.Translate(moveSpeed * Input.GetAxis("Horizontal") * Time.deltaTime, 0f, moveSpeed * Input.GetAxis("Vertical") * Time.deltaTime);
	}
	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "Coin")
		{
			Destroy(collision.gameObject);
		}

	}
}