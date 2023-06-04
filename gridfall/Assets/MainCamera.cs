using UnityEngine;

public class MainCamera : MonoBehaviour
{

	[SerializeField] private Transform _targetObject;

	private void Update() {
		transform.position = _targetObject.position;
	}
}
