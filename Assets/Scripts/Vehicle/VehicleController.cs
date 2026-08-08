// Neon Cipher — Vehicle Controller (car, motorbike, drone)
using UnityEngine;

namespace NeonCipher.Vehicle
{
    public enum VehicleKind { Car, Bike, Drone }

    [RequireComponent(typeof(Rigidbody))]
    public sealed class VehicleController : MonoBehaviour
    {
        [SerializeField] private VehicleKind _kind = VehicleKind.Car;
        [SerializeField] private WheelCollider[] _frontWheels;
        [SerializeField] private WheelCollider[] _rearWheels;
        [SerializeField] private float _maxSteer = 28f, _maxTorque = 1200f, _maxBrake = 3500f, _topKmh = 220f;
        [SerializeField] private float _droneLift = 12f, _droneMaxSpeed = 22f, _droneTurnSpeed = 90f;
        [SerializeField] private float _steerLerp = 8f, _throttleLerp = 6f;

        private Rigidbody _rb;
        private float _steer, _throttle, _brake, _droneYaw;
        public bool Occupied { get; private set; }
        public VehicleKind Kind => _kind;
        public Transform MountPoint { get; private set; }
        public float SpeedKmh => _rb == null ? 0f : _rb.velocity.magnitude * 3.6f;
        public void SetKind(VehicleKind k) => _kind = k;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.centerOfMass = new Vector3(0f, -0.6f, 0f);
            var mp = new GameObject("MountPoint");
            mp.transform.SetParent(transform); mp.transform.localPosition = Vector3.up * 1.1f;
            MountPoint = mp.transform;
            if (_kind == VehicleKind.Drone) _rb.useGravity = false;
        }
        public void Enter() => Occupied = true;
        public void Exit() => Occupied = false;

        public void SetControl(float steer, float throttle, float brake)
        {
            _steer = Mathf.Lerp(_steer, Mathf.Clamp(steer, -1f, 1f) * _maxSteer, Time.deltaTime * _steerLerp);
            float p = (_kind == VehicleKind.Bike) ? _maxTorque * 0.7f : _maxTorque;
            _throttle = Mathf.Lerp(_throttle, Mathf.Clamp(throttle, -1f, 1f) * p, Time.deltaTime * _throttleLerp);
            _brake = Mathf.Clamp01(brake) * _maxBrake;
        }

        private void FixedUpdate()
        {
            if (_kind == VehicleKind.Drone) { TickDrone(); return; }
            float frac = Mathf.Clamp01(SpeedKmh / Mathf.Max(1f, _topKmh));
            float st = _steer * Mathf.Lerp(1f, 0.4f, frac);
            if (_frontWheels != null) foreach (var w in _frontWheels) if (w != null) w.steerAngle = st;
            if (_rearWheels != null && _rearWheels.Length > 0)
                foreach (var w in _rearWheels) if (w != null) { w.motorTorque = _throttle; w.brakeTorque = _brake; }
            else if (_frontWheels != null)
                foreach (var w in _frontWheels) if (w != null) { w.motorTorque = _throttle * 0.5f; w.brakeTorque = _brake; }
        }

        private void TickDrone()
        {
            _droneYaw += _steer * _droneTurnSpeed * Time.fixedDeltaTime;
            var yq = Quaternion.Euler(0f, _droneYaw, 0f);
            _rb.MoveRotation(yq);
            var target = yq * Vector3.forward * Mathf.Clamp(_throttle / Mathf.Max(1f, _maxTorque), -1f, 1f) * _droneMaxSpeed;
            target.y = _droneLift - _brake * 0.005f;
            _rb.velocity = Vector3.Lerp(_rb.velocity, target, 4f * Time.fixedDeltaTime);
        }
    }
}
