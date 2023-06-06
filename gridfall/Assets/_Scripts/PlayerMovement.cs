using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

	public static PlayerMovement Instance { get; private set; }

	[SerializeField] private InputHandler _inputHandler;

	[Header("Movement Settings"), Space(5)]
	[SerializeField] private int _moveDistance = 1;
	[SerializeField] [Tooltip("In Seconds")] private float _moveTime = 1f;
	[SerializeField] [Tooltip("In Seconds")] private float _rotationTime = 1f;


	private bool _isMoving = false;

	private void Awake() {
		if (Instance != null) Debug.LogWarning("There is more than one Player instance in the scene!");
		else Instance = this;
	}

	private void Start() {
		_inputHandler.OnMoveAction += InputHandler_OnMoveAction;
		_inputHandler.OnRotateAction += InputHandler_OnRotateAction;
	}

	private void InputHandler_OnMoveAction(object sender, System.EventArgs e) {
		if (!_isMoving) {
			_isMoving = true;
			StartCoroutine(Move(_inputHandler.GetMovementVectorNormalized()));
		}
	}

	private void InputHandler_OnRotateAction(object sender, System.EventArgs e) {
		if (!_isMoving) {
			_isMoving = true;
			StartCoroutine(Rotate(_inputHandler.GetRotationDirection()));
		}
	}

	private IEnumerator Move(Vector2 direction) {
		Vector3 previousPosition = transform.position;
		Vector3 targetDirection = new Vector3(direction.x, 0, direction.y);

		// Fix direction to 4 directions
		if (Mathf.Abs(targetDirection.x) > Mathf.Abs(targetDirection.z)) {
			targetDirection.z = 0;
			targetDirection.x = Mathf.Ceil(direction.x);
		} else {
			targetDirection.x = 0;
			targetDirection.z = Mathf.Ceil(direction.y);
		}

		// Raycast check for object in the way	

		// Finds target location based off y rotation
		Vector3 targetLocation = transform.position + (Quaternion.Euler(0, transform.eulerAngles.y, 0) * new Vector3(direction.x, 0, direction.y)) * _moveDistance;
		
		float timer = 0f;	
		while (Vector3.Distance(transform.position, targetLocation) > 0.05f) {
			transform.position = Vector3.Lerp(previousPosition, targetLocation, timer / _moveTime);
			timer += Time.deltaTime;
			yield return null;
		}
		transform.position = targetLocation;
		FixPosition();
		_isMoving = false;
	}

	private IEnumerator Rotate(int direction) {
		Vector3 previousRotation = transform.eulerAngles;

		// Gets target rotation
		Vector3 targetRotation = transform.eulerAngles + new Vector3(0, 90 * direction, 0);
		Vector3 fixedTargetRotation = targetRotation;

		// Fixes rotation for while check to work
		if (targetRotation.y < 0) fixedTargetRotation.y += 360;
		if (targetRotation.y >= 360) fixedTargetRotation.y -= 360;

		float timer = 0f;
		while (Vector3.Distance(transform.eulerAngles, fixedTargetRotation) > 0.05f) {
			transform.eulerAngles = Vector3.Lerp(previousRotation, targetRotation, timer / _rotationTime);
			timer += Time.deltaTime;
			yield return null;
		}
		transform.eulerAngles = targetRotation;
		FixPosition();
		_isMoving = false;
	}

	private void FixPosition() {
		// Rounds position to nearest int
		transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), Mathf.Round(transform.position.z));
	}
}