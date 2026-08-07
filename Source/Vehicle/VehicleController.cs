// =====================================================================
//  Neon Cipher — Vehicle (cars, bikes, patrol drones)
//  File:    VehicleController.cs
//  Notes:   Arcade-friendly physics. Realistic-ish but stable on mobile.
//           Works for any 4-wheel Rigidbody or rigid-road-bike chassis.
// =====================================================================
using UnityEngine;

namespace NeonCipher.Vehicle
{
    public enum VehicleKind { Car, Bike, Drone }

    [RequireComponent(typeof(Rigidbody))]
    public sealed class VehicleController : MonoBehaviour
    {
        [Header("Drivetrain")]
        [SerializeField] private VehicleKind _kind = VehicleKind.Car;
        [SerializeField] private WheelCollider[] _frontWheels;
        [SerializeField] private WheelCollider[] _rearWheels;
        [SerializeField] private float _maxSteer = 28f;
        [SerializeField] private float _maxTorque = 1200f;
        [SerializeField] private float _maxBrake = 3500f;
        [SerializeField] private float _topKmh = 220f;

        [Header("Damping")]
        [SerializeField] private float _steerLerp = 8f;
        [SerializeField] private float _throttleLerp = 6f;

        public bool Occupied { get; private set; }
        public float SpeedKmh => rigidbody.velocity.magnitude * 3.6f;
        public VehicleKind Kind => _kind;
        public Transform MountPoint { get; private set; }

        private Rigidbody rigidbody;
        private float steer, throttle, brake;

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
            rigidbody.centerOfMass = new Vector3(0, -0.6f, 0);
            MountPoint = new GameObject("MountPoint").transform;
            MountPoint.SetParent(transform); MountPoint.localPosition = Vector3.up * 1.1f;
        }

        public void Enter()  => Occupied = true;
        public void Exit()   => Occupied = false;

        public void SetControl(float steerInput, float throttleInput, float brakeInput)
        {
            steer = Mathf.Lerp(steer, Mathf.Clamp(steerInput, -1f, 1f) * _maxSteer, Time.deltaTime * _steerLerp);
            float power = (_kind == VehicleKind.Bike) ? _maxTorque * 0.7f : _maxTorque;
            throttle = Mathf.Lerp(throttle, Mathf.Clamp(throttleInput, -1f, 1f) * power, Time.deltaTime * _throttleLerp);
            brake = Mathf.Clamp01(brakeInput) * _maxBrake;
        }

        private void FixedUpdate()
        {
            float speedKmh = SpeedKmh;
            float speedFrac = Mathf.Clamp01(speedKmh / _topKmh);

            // Steer with speed-progressive dampening
            float steerAngle = steer * Mathf.Lerp(1f, 0.4f, speedFrac);
            foreach (var w in _frontWheels)  { w.steerAngle = steerAngle; }
            foreach (var w in _rearWheels)   { w.motorTorque = throttle; w.brakeTorque = brake; }

            if (_rearWheels.Length == 0 && _frontWheels.Length > 0)
            {
                for (int i = 0; i < _frontWheels.Length; i++)
                {
                    _frontWheels[i].motorTorque = throttle * (i == 0 ? 0.6f : 0.4f);
                    _frontWheels[i].brakeTorque = brake;
                }
            }
        }
    }
}
