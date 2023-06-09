using UnityEngine;

public class MainCamera : MonoBehaviour
{

	[SerializeField] private Transform _targetObject;

	private void Start() {
		transform.parent = _targetObject;

		transform.localScale = Vector3.one;
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
	}
}
