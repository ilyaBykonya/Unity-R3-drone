using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public class Hover : MonoBehaviour {
    private Rigidbody HoverRigidbody = null;

    [field: SerializeField][field: Range(0, 10000)] public float MaxPower { get; private set; } = 0;
    [SerializeField][Gast.Utils.ReadOnlyField.ReadOnly][FormerlySerializedAs("Pow")] private float _currentPower;
    public float CurrentPower { 
        get => _currentPower; 
        set { 
            _currentPower = Mathf.Clamp(value, 0, MaxPower); 
        } 
    }


    private void Start() => HoverRigidbody = GetComponent<Rigidbody>();
    private void Update() {
        HoverRigidbody.AddRelativeForce(new Vector3(0, CurrentPower * Time.deltaTime, 0), ForceMode.Acceleration);
    }
}
