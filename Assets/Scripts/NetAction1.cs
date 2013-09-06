using UnityEngine;
using System.Collections;

public class NetAction1 : MonoBehaviour 
{
	Vector3 lastPosition;
	float minimumMovement = .05f;
	
	void Awake()
	{
	    if (!networkView.isMine)
		{
	        enabled = false;
		}
	}
	
	void Update () 
	{
		if (networkView.isMine)
		{
		    Vector3 moveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		    float speed = 5;
		    transform.Translate(speed * moveDir * Time.deltaTime);
//			if (Vector3.Distance(transform.position, lastPosition) > minimumMovement)
//			{
//			    lastPosition = transform.position;
//			    networkView.RPC("SetPosition", RPCMode.Others, transform.position);
//			}
		}
	}
	
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
	    if (stream.isWriting)
	    {
	    Vector3 myPosition = transform.position;
	    stream.Serialize(ref myPosition);
	    }
	    else
	    {
	        Vector3 receivedPosition = Vector3.zero;
	        stream.Serialize(ref receivedPosition); //"Decode" it and receive it
	        transform.position = receivedPosition;
	    }
	}
	
	
//	[RPC]
//	void SetPosition(Vector3 newPosition)
//	{
//	    transform.position = newPosition;
//	}
	
//	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
//	{
//	    if (stream.isWriting)
//		{
//		  Vector3 pos = transform.position;
//		  stream.Serialize(ref pos);
//		}
//		else
//		{
//	      Vector3 receivedPosition = Vector3.zero;
//	      stream.Serialize(ref receivedPosition);
//	      transform.position = receivedPosition;
//		}
//	}
}
