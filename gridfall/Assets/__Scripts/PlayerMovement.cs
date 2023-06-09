using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

	public static PlayerMovement Instance { get; private set; }

	[SerializeField] private InputHandler _inputHandler;

	[Header("Movement Settings")]
	[SerializeField] private int _moveDistance = 1;
	[SerializeField] [Tooltip("In Seconds")] private float _moveTime = 1f;
	[SerializeField] [Tooltip("In Seconds")] private float _rotationTime = 1f;

	[Header("Locking Settings")]
	// [SerializeField] private float movementLockingDistance = 0.3f;
	[SerializeField] private float rotationLockingDistance = 10f;

	[Header("Raycast Settings")]
	[SerializeField] private LayerMask _layerMask;
	[SerializeField] private float _raycastDistance = 2f;
	[SerializeField] private Transform _raycastStartingLocation;

	[Header("Bounce Settings")]
	[SerializeField] private bool _bounceFeedback = true;
	[SerializeField] [Tooltip("A fraction of Move Distance")] private float _bounceDistance = 0.2f;
	[SerializeField] private float _bounceTime = 0.2f;

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

		targetDirection = (Quaternion.Euler(0, transform.eulerAngles.y, 0) * new Vector3(direction.x, 0, direction.y));

		if (CheckForCollision(targetDirection)) {
			if (!_bounceFeedback) {
				_isMoving = false;
				yield break;
			}

			StartCoroutine(Bounce(targetDirection));
			yield break;
		}

		// Finds target location based off y rotation
		Vector3 targetLocation = transform.position + targetDirection * _moveDistance;
		
		float timer = 0f;	
		while (Vector3.Distance(transform.position, targetLocation) > 0.05f) {
			transform.position = Vector3.Lerp(previousPosition, targetLocation, timer / _moveTime);
			timer += Time.deltaTime;
			yield return null;
		}
		transform.position = targetLocation;
		FixPosition();

		// Repeate movement if input is still being held
		// TODO fix this
		if (_inputHandler._inputActions.Game.Move.ReadValue<Vector2>().magnitude > 0.5f) StartCoroutine(Move(_inputHandler.GetMovementVectorNormalized()));
		else _isMoving = false;
	}

	private IEnumerator Bounce(Vector3 direction) {
		Vector3 targetLocation = transform.position + direction * (_bounceDistance / _moveDistance);
		Vector3 previousPosition = transform.position;

		// Bounce too
		float timer = 0f;
		while (Vector3.Distance(transform.position, targetLocation) > 0.05f) {
			transform.position = Vector3.Lerp(previousPosition, targetLocation, timer / _bounceTime);
			timer += Time.deltaTime;
			yield return null;
		}
		transform.position = targetLocation;

		// Bounce away
		timer = 0f;
		while (Vector3.Distance(transform.position, previousPosition) > 0.05f) {
			transform.position = Vector3.Lerp(targetLocation, previousPosition, timer / _bounceTime);
			timer += Time.deltaTime;
			yield return null;
		}
		transform.position = previousPosition;
		FixPosition();
		_isMoving = false;
		yield break;
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

	private bool CheckForCollision(Vector3 direction) {
		var ray = new Ray(_raycastStartingLocation.position, direction);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, _raycastDistance, _layerMask)) return true;
		else return false;
	}

	private void FixPosition() {
		// Rounds position to nearest int
		transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), Mathf.Round(transform.position.z));
		
		// Round rotation to nearest cardinal direction
		for (int i = 0; i < 360; i += 90) {
			if (transform.eulerAngles.y > i - rotationLockingDistance && transform.eulerAngles.y < i + rotationLockingDistance) transform.rotation = Quaternion.Euler(new Vector3(0, i, 0));
		}
	}
}