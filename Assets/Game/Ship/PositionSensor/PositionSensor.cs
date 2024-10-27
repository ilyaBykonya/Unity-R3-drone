using UnityEngine;
using R3;
using System;

public class PositionSensor : MonoBehaviour {
    public Observable<Vector3> Value { get; private set; } = null;
    void Awake() => Value = Observable.Interval(TimeSpan.FromSeconds(.1), UnityTimeProvider.Update).Select(_ => transform.position);
}
