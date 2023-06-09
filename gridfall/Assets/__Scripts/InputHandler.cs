using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour {

	public event EventHandler OnRotateAction;
	public event EventHandler OnMoveAction;

	public event EventHandler OnTurnAction;

	public InputActions _inputActions;

	private void Awake() {
		_inputActions = new InputActions();
		_inputActions.Game.Enable();

		_inputActions.Game.Rotate.performed += Rotate_performed;
		_inputActions.Game.Move.performed += Move_performed;
	}

	private void Rotate_performed(InputAction.CallbackContext context) {
		OnRotateAction?.Invoke(this, EventArgs.Empty);
	}

	private void Move_performed(InputAction.CallbackContext context) {
		OnMoveAction?.Invoke(this, EventArgs.Empty);
	}

	public Vector2 GetMovementVectorNormalized() {
		Vector2 inputVector = _inputActions.Game.Move.ReadValue<Vector2>();
		inputVector = inputVector.normalized;
		return inputVector;
	}

	public int GetRotationDirection() {
		return (int)_inputActions.Game.Rotate.ReadValue<float>();
	}

}
