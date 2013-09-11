using UnityEngine;
using System.Collections;

public class Skeleton : MonoBehaviour 
{

    public float smooth = 1f;
    Animator animator;
    public Camera camera;
    public Transform cameraPosition;
    bool battle;
    private float RotationSpeed = -700;


    static int idleState = Animator.StringToHash("Base Layer.idle");
    static int runState = Animator.StringToHash("Base Layer.run");
    static int attackState = Animator.StringToHash("Base Layer.attack");

    private Vector3 _lastCamPosition;
    private AnimatorStateInfo currentBaseState;

    private float meshMoveSpeed = .1f;

    void Start () 
    {
        animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () 
    {

	}

    void FixedUpdate()
    {
        currentBaseState = animator.GetCurrentAnimatorStateInfo(0);

        float dx = Input.GetAxis("Mouse X");
        float dy = Input.GetAxis("Mouse Y");
 
        this.transform.Rotate(Vector3.down, dx * RotationSpeed * Time.deltaTime, Space.Self);


        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        battle = Input.GetButton("Fire1");

        animator.SetFloat("Speed", vertical);
        animator.SetBool("Battle", battle);
        
		if (currentBaseState.nameHash == runState)
		{
	        Vector3 newPosition = transform.position;
	        transform.position = newPosition + transform.forward * vertical * meshMoveSpeed;
		}

        if (currentBaseState.nameHash != attackState)
        {
            camera.transform.position = cameraPosition.position;//Vector3.Lerp(camera.transform.position, cameraPosition.position, Time.deltaTime * smooth);
            camera.transform.forward = cameraPosition.forward;//Vector3.Lerp(camera.transform.forward, cameraPosition.forward, Time.deltaTime * smooth);
        }
    }
}
