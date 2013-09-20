using UnityEngine;
using System.Collections;

public class Destructable : MonoBehaviour 
{
	public GameObject destructableObj;
	
	void OnMouseDown()
	{
		DestoyMe();
	}
	
	private void DestoyMe()
	{
		Instantiate(destructableObj, transform.position, transform.rotation);
		Destroy(gameObject);	
	}
	
}
