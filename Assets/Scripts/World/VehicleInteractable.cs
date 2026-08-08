// Neon Cipher — Vehicle Interactable + Driver
using NeonCipher.InputLayer;
using NeonCipher.Player;
using NeonCipher.Vehicle;
using UnityEngine;

namespace NeonCipher.World.Interaction
{
    public sealed class VehicleInteractable : MonoBehaviour, IInteractable
    {
        public VehicleController Vehicle;
        public VehicleKind Kind;
        public string Prompt => Vehicle != null && Vehicle.Occupied ? "Exit" : $"Enter {Kind}";

        public void Interact(PlayerController player)
        {
            if (Vehicle == null) return;
            if (!Vehicle.Occupied) MountPlayer(player); else UnmountPlayer(player);
        }

        private void MountPlayer(PlayerController player)
        {
            Vehicle.Enter();
            player.gameObject.SetActive(false);
            var driverGo = new GameObject("[VehicleDriver]");
            driverGo.transform.SetParent(Vehicle.transform, false);
            var d = driverGo.AddComponent<VehicleDriver>();
            d.Vehicle = Vehicle; d.Player = player; d.Interactable = this;

            var cam = Object.FindObjectOfType<NeonCipher.Camera.ThirdPersonCameraRig>();
            if (cam != null)
            {
                cam.Target = Vehicle.transform;
                cam.Distance = Kind == VehicleKind.Drone ? 8f : 6f;
                cam.Height = Kind == VehicleKind.Bike ? 1.2f : 1.6f;
                cam.SnapYawTo(Vehicle.transform.forward);
            }
        }

        private void UnmountPlayer(PlayerController player)
        {
            Vehicle.Exit();
            var driver = Vehicle.GetComponentInChildren<VehicleDriver>();
            if (driver != null) Destroy(driver.gameObject);
            player.gameObject.SetActive(true);
            player.transform.position = Vehicle.transform.position + Vehicle.transform.right * 2.0f + Vector3.up * 0.5f;
            var cam = Object.FindObjectOfType<NeonCipher.Camera.ThirdPersonCameraRig>();
            if (cam != null) { cam.Target = player.transform; cam.Distance = 4.5f; cam.Height = 1.65f; }
        }
    }

    public sealed class VehicleDriver : MonoBehaviour
    {
        public VehicleController Vehicle;
        public PlayerController Player;
        public VehicleInteractable Interactable;
        private IInputProvider _input;
        private float _exitCooldown;

        private void Awake()
        {
#if ENABLE_INPUT_SYSTEM
            _input = new UnityInputReader();
#endif
            _exitCooldown = 0.5f;
        }
        private void Update()
        {
            if (Vehicle == null || _input == null) return;
            _exitCooldown -= Time.deltaTime;
            var move = _input.Move;
            Vehicle.SetControl(move.x, move.y, _input.Crouch ? 1f : 0f);
            if (_input.Interact && _exitCooldown <= 0f) Interactable.Interact(Player);
        }
    }
}
