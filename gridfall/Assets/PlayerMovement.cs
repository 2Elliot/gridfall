using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

	public static PlayerMovement Instance { get; private set; }

	[SerializeField] private InputHandler _inputHandler;

	[Header("Movement Settings"), Space(5)]
	[SerializeField] [Tooltip("In Seconds")] private float _moveDistance = 1f;
	[SerializeField] private float _moveSpeed = 1f;

	private bool _isMoving = false;

	private void Awake() {
		if (Instance != null) Debug.LogWarning("There is more than one Player instance in the scene!");
		else Instance = this;
	}

	private void Start() {
		_inputHandler.OnMoveAction += _inputHandler_OnMoveAction;
	}

	private void _inputHandler_OnMoveAction(object sender, System.EventArgs e) {
		if (!_isMoving) {
			_isMoving = true;
			StartCoroutine(Move(_inputHandler.GetMovementVectorNormalized()));
		}
	}

	private IEnumerator Move(Vector2 direction) {
		// Raycast check for object in the way
		Vector3 targetLocation = transform.position + new Vector3(direction.x, 0, direction.y) * _moveDistance;
		float timer = 0f;

		while (Vector3.Distance(transform.position, targetLocation) > 0.05f) {
			transform.position = Vector3.Lerp(transform.position, targetLocation, timer / _moveSpeed);
			timer += Time.deltaTime * _moveSpeed;
			yield return null;
		}
		_isMoving = false;
	}
}