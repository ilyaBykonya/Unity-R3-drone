using R3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Assets.Game.Ship
{
    public class ShipController: MonoBehaviour {
        [Serializable]
        public struct HoverInfo {
            [field: SerializeField] public string Name { get; private set; }
            [field: SerializeField] public Hover Hover { get; private set; }
            [field: SerializeField] public PositionSensor PositionSensor { get; private set; }
            [field: SerializeField] public Slider PowerIndicator { get; private set; }

            public HoverInfo(string name, Hover hover, PositionSensor positionSensor, Slider powerIndicator) {
                Name = name;
                Hover = hover;
                PositionSensor = positionSensor;
                PowerIndicator = powerIndicator;
            }
        }
        private CompositeDisposable PositionSensorSubscriptions = new();
        [SerializeField] private List<HoverInfo> _hovers = new();
        public IReadOnlyList<HoverInfo> Hovers => _hovers;


        private void Start()
        {
            var stream = Observable.CombineLatest(Hovers.Select(sensor => sensor.PositionSensor.Value))
                 .Chunk(TimeSpan.FromSeconds(0.5), UnityTimeProvider.Update)
                 .Where(changes => changes.Length > 0)
                 .Select(changes => changes[changes.Length - 1]);

            stream.Subscribe(values => Debug.Log($"Sensor positions: [{values}]")).AddTo(PositionSensorSubscriptions);
            stream.Subscribe(ProcessPositionSensorValue).AddTo(PositionSensorSubscriptions);
        }

        private void ProcessPositionSensorValue(Vector3[] positionSensorMeasurement) {
            var MinHoverPower = Hovers.Select(hover => hover.Hover.MaxPower).Min();
            var shipYMeasurements = positionSensorMeasurement.Select(position => position.y);
            var MaxHeight = shipYMeasurements.Max();
            var MinHeight = shipYMeasurements.Min();
            var HeightDifference = MaxHeight - MinHeight;

            for(int index = 0; index < Hovers.Count; index++) {
                var AdditionalPowerPercent = 1.0f - (positionSensorMeasurement[index].y - MinHeight) / HeightDifference;
                Hovers[index].Hover.CurrentPower = MinHoverPower * (0.95f + (0.04f * AdditionalPowerPercent));
                Hovers[index].PowerIndicator.value = AdditionalPowerPercent;
            }
        }
    }
}
