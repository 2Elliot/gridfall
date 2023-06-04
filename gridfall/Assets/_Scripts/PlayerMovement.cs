using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

	public static PlayerMovement Instance { get; private set; }

	[SerializeField] private InputHandler _inputHandler;

	[Header("Movement Settings"), Space(5)]
	[SerializeField] private float _moveDistance = 1f;
	[SerializeField] [Tooltip("In Seconds")] private float _moveTime = 1f;

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
		// Fix direction to 4 directions
		if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y)) {
			direction.y = 0;
			direction.x = Mathf.Ceil(direction.x);
		} else {
			direction.x = 0;
			direction.y = Mathf.Ceil(direction.y);
		}

		// Raycast check for object in the way
		Vector3 targetLocation = transform.position + (Quaternion.Euler(0, transform.rotation.y, 0) * new Vector3(direction.x, 0, direction.y)) * _moveDistance; // Fixes targetLocation from previous rotation
		float timer = 0f;
		
		while (Vector3.Distance(transform.position, targetLocation) > 0.05f) {
			transform.position = Vector3.Lerp(transform.position, targetLocation, timer / _moveTime);
			timer += Time.deltaTime;
			yield return null;
		}
		transform.position = targetLocation;
		_isMoving = false;
	}

	private IEnumerator Rotate(int direction) {
		// Raycast check for object in the way
		Vector3 targetRotation = transform.eulerAngles + new Vector3(0, 90 * direction, 0);
		float timer = 0f;
		while (Vector3.Distance(transform.eulerAngles, targetRotation) > 0.05f) {
			transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, targetRotation, timer / _moveTime);
			timer += Time.deltaTime;
			yield return null;
		}
		transform.eulerAngles = targetRotation;
		_isMoving = false;
	}
}