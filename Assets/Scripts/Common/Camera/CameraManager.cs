#pragma warning disable 0414, 0219, 649, 169, 618, 1570
using System;
using UnityEngine;

public class CameraManager : ZenBehaviour, IOnAwake, IOnStart, IOnUpdate, IOnFixedUpdate, IOnLateUpdate
{
    public override int ExecutionPriority => 100000;
    public override Type ObjectType => typeof(CameraManager);

	//Camera config
    [SerializeField] private UpdateType updateType = UpdateType.LateUpdate;
	public float CameraScrollSpeed = 10f;
	public float CameraZoomSpeed = 10f;
	public float CameraRotationSpeed = 10f;

	public float mousePanMultiplier = 0.1f;
	public float mouseRotationMultiplier = 0.2f;
	public float mouseZoomMultiplier = 5.0f;
	public float minZoomDistance = 20.0f;
	public float maxZoomDistance = 200.0f;
	public float smoothingFactor = 0.1f;
	public float goToSpeed = 0.1f;

	public bool useKeyboardInput = true;
	public bool useMouseInput = true;
	public bool adaptToTerrainHeight = true;
	public bool increaseSpeedWhenZoomedOut = true;
	public bool correctZoomingOutRatio = true;
	public bool smoothing = true;
	public bool allowDoubleClickMovement = false;
	public bool allowScreenEdgeMovement = true;

	public int screenEdgeSize = 10;
	public float screenEdgeSpeed = 1.0f;

	public GameObject objectToFollow;
	public Vector3 cameraTarget;

	//Internal

	private Camera cam;
    private float camV, camH, camZoom, camRot, camPitch;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

// -------------------------- Private Attributes --------------------------
    private float currentCameraDistance;
    private Vector3 lastMousePos;
    private Vector3 lastPanSpeed = Vector3.zero;
    private Vector3 goingToCameraTarget = Vector3.zero;
    private bool doingAutoMovement = false;
//private var doubleClickDetector : DoubleClickDetector;


    ///OnAwake happens once upon Instantiation, so after Awake and OnEnable
    public void OnAwake()
    {
        //if (!IsEnabled) return;
        //CameraVertical, CameraHorizontal, CameraZoom, CameraRotate, CameraPitch, CameraReset
        if (cam == null) cam = GetComponent<Camera>();
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    public void OnStart()
    {
        currentCameraDistance = minZoomDistance + ((maxZoomDistance - minZoomDistance)/2.0f);
        lastMousePos = Vector3.zero;
    }

    public void OnUpdate()
    {
        camV = Input.GetAxisRaw("CameraVertical");
        camH = Input.GetAxisRaw("CameraHorizontal");
        camZoom = Input.GetAxis("CameraZoom");
        camRot = Input.GetAxisRaw("CameraRotate");
        camPitch = Input.GetAxisRaw("CameraPitchOn");

        if (Input.GetButtonDown("CameraReset"))
        {
            transform.position = initialPosition;
            transform.rotation = initialRotation;
        }

        if (updateType == UpdateType.Update)
            MovementUpdate(Time.deltaTime);
    }

    private void MovementUpdate(float deltaTime)
    {
        UpdatePanning(deltaTime);
        UpdateRotation(deltaTime);
        UpdateZooming(deltaTime);
        UpdatePosition(deltaTime);
        UpdateAutoMovement(deltaTime);
        lastMousePos = Input.mousePosition;
    }

    public void OnFixedUpdate()
    {
        if (updateType == UpdateType.FixedUpdate)
            MovementUpdate(Time.fixedDeltaTime);
    }

    public void OnLateUpdate()
    {
        if (updateType == UpdateType.LateUpdate)
            MovementUpdate(Time.deltaTime);
    }


    public void GoTo(Vector3 position)
    {
        doingAutoMovement = true;
        goingToCameraTarget = position;
        objectToFollow = null;
    }

    public void Follow(GameObject gameObjectToFollow)
    {
        objectToFollow = gameObjectToFollow;
    }


    private void UpdatePanning(float deltaTime)
    {
        var moveVector = new Vector3(0, 0, 0);
        if (useKeyboardInput)
        {
            moveVector += new Vector3(
                camH*CameraScrollSpeed*deltaTime,
                camZoom*CameraZoomSpeed*deltaTime,
                camV*CameraScrollSpeed*deltaTime
            );
        }

        if (allowScreenEdgeMovement)
        {
            if (Input.mousePosition.x < screenEdgeSize)
            {
                moveVector.x -= screenEdgeSpeed;
            }
            else if (Input.mousePosition.x > Screen.width - screenEdgeSize)
            {
                moveVector.x += screenEdgeSpeed;
            }

            if (Input.mousePosition.y < screenEdgeSize)
            {
                moveVector.z -= screenEdgeSpeed;
            }
            else if (Input.mousePosition.y > Screen.height - screenEdgeSize)
            {
                moveVector.z += screenEdgeSpeed;
            }
        }

        if (useMouseInput)
        {
            if (Input.GetMouseButton(2) && Input.GetKey(KeyCode.LeftShift))
            {
                var deltaMousePos = (Input.mousePosition - lastMousePos);
                moveVector += new Vector3(-deltaMousePos.x, 0, -deltaMousePos.y)*mousePanMultiplier;
            }
        }

        if (moveVector != Vector3.zero)
        {
            objectToFollow = null;
            doingAutoMovement = false;
        }

        var effectivePanSpeed = moveVector;
        if (smoothing)
        {
            effectivePanSpeed = Vector3.Lerp(lastPanSpeed, moveVector, smoothingFactor);
            lastPanSpeed = effectivePanSpeed;
        }

        var oldRotation = transform.localEulerAngles.x;
        transform.localEulerAngles = new Vector3(0.0f, transform.localEulerAngles.y, transform.localEulerAngles.z);
        var panMultiplier = increaseSpeedWhenZoomedOut ? (Mathf.Sqrt(currentCameraDistance)) : 1.0f;
        cameraTarget = cameraTarget +
                               transform.TransformDirection(effectivePanSpeed)*CameraScrollSpeed*panMultiplier*
                               Time.deltaTime;
        transform.localEulerAngles = new Vector3(oldRotation, transform.localEulerAngles.y, transform.localEulerAngles.z);
    }

    private void UpdateRotation(float deltaTime)
    {
        var deltaAngleH = 0.0f;
        var deltaAngleV = 0.0f;

        if (useKeyboardInput)
        {
            deltaAngleH = camRot*deltaTime*CameraRotationSpeed;
        }

        if (useMouseInput)
        {
            if (Input.GetMouseButton(2) && !Input.GetKey(KeyCode.LeftShift))
            {
                var deltaMousePos = (Input.mousePosition - lastMousePos);
                deltaAngleH += deltaMousePos.x*mouseRotationMultiplier;
                deltaAngleV -= deltaMousePos.y*mouseRotationMultiplier;
            }
        }
        transform.localEulerAngles = new Vector3(
            Mathf.Min(80.0f,
                Mathf.Max(5.0f, transform.localEulerAngles.x + deltaAngleV*Time.deltaTime*CameraRotationSpeed)),
            transform.localEulerAngles.y + deltaAngleH*Time.deltaTime*CameraRotationSpeed,
            0
        );
    }

    private void UpdateZooming(float deltaTime)
    {
        var deltaZoom = 0.0f;

        if (useMouseInput)
        {
            var scroll = Input.GetAxis("Mouse ScrollWheel");
            deltaZoom -= scroll*mouseZoomMultiplier;
        }
        var zoomedOutRatio = correctZoomingOutRatio
            ? (currentCameraDistance - minZoomDistance)/(maxZoomDistance - minZoomDistance)
            : 0.0f;
        currentCameraDistance = Mathf.Max(minZoomDistance,
            Mathf.Min(maxZoomDistance,
                currentCameraDistance + deltaZoom*Time.deltaTime*CameraZoomSpeed*(zoomedOutRatio*2.0f + 1.0f)));
    }

    private void UpdatePosition(float deltaTime)
    {
        if (objectToFollow != null)
        {
            cameraTarget = Vector3.Lerp(cameraTarget, objectToFollow.transform.position,
                goToSpeed);
        }

        transform.position = cameraTarget;
        transform.Translate(Vector3.back*currentCameraDistance);

		if (transform.position.y < minZoomDistance || transform.position.y > maxZoomDistance)
			transform.position = new Vector3(transform.position.x, 
				Mathf.Clamp(transform.position.y, minZoomDistance, maxZoomDistance), 
				transform.position.z);
    }

    private void UpdateAutoMovement(float deltaTime)
    {
        if (doingAutoMovement)
        {
            cameraTarget = Vector3.Lerp(cameraTarget, goingToCameraTarget, goToSpeed);
            if (Vector3.Distance(goingToCameraTarget, cameraTarget) < 1.0f)
            {
                doingAutoMovement = false;
            }
        }
    }
}


public enum UpdateType
{
    Update,
    FixedUpdate,
    LateUpdate
}
