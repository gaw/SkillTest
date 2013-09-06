using UnityEngine;
using System.Collections;

public class NetAction1 : MonoBehaviour 
{
	void Update () 
	{
		if (Network.isServer)
		{
		    Vector3 moveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		    float speed = 5;
		    transform.Translate(speed * moveDir * Time.deltaTime);
		}
	}
}
