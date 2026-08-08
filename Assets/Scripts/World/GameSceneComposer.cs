// Neon Cipher — Runtime Scene Composer
// Builds ORIGINAL player, vehicles, and a prototype city out of Unity
// primitives so the project runs without any external art asset.
using System.Collections.Generic;
using NeonCipher.Camera;
using NeonCipher.Core;
using NeonCipher.Mission;
using NeonCipher.Player;
using NeonCipher.Traffic;
using NeonCipher.UI;
using NeonCipher.Vehicle;
using NeonCipher.World.Interaction;
using UnityEngine;

namespace NeonCipher.World
{
    public sealed class GameSceneComposer : MonoBehaviour
    {
        public PlayerController Player { get; private set; }
        public ThirdPersonCameraRig CameraRig { get; private set; }
        public VehicleController Car { get; private set; }
        public VehicleController Bike { get; private set; }
        public HudController Hud { get; set; }

        private readonly List<GameObject> _spawned = new();

        public void BuildAll(HudController hud)
        {
            Hud = hud;
            BuildEnvironment();
            BuildCity();
            BuildPlayer();
            BuildCamera();
            BuildCar();
            BuildBike();
            BuildMission();
        }

        private void BuildEnvironment()
        {
            RenderSettings.ambientLight = new Color(0.28f, 0.32f, 0.42f);
            RenderSettings.fog = true;
            RenderSettings.fogColor = new Color(0.12f, 0.14f, 0.20f);
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogDensity = 0.005f;

            var sun = new GameObject("Sun");
            var light = sun.AddComponent<Light>();
            light.type = LightType.Directional;
            light.color = new Color(1f, 0.95f, 0.85f);
            light.intensity = 1.1f;
            light.shadows = LightShadows.Soft;
            sun.transform.rotation = Quaternion.Euler(50f, 40f, 0f);
            _spawned.Add(sun);

            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.localScale = new Vector3(80, 1, 80);
            Paint(ground, new Color(0.08f, 0.11f, 0.13f));
            _spawned.Add(ground);
        }

        private void BuildCity()
        {
            BuildRoad(new Vector3(0, 0.02f, 0), new Vector3(700, 0.02f, 12));
            BuildRoad(new Vector3(0, 0.02f, 0), new Vector3(12, 0.02f, 700));
            BuildRoad(new Vector3(0, 0.02f, 200), new Vector3(500, 0.02f, 10));
            BuildRoad(new Vector3(0, 0.02f, -200), new Vector3(500, 0.02f, 10));
            BuildRoad(new Vector3(200, 0.02f, 0), new Vector3(10, 0.02f, 500));
            BuildRoad(new Vector3(-200, 0.02f, 0), new Vector3(10, 0.02f, 500));

            BuildBuilding(new Vector3(80, 20, 80), new Vector3(40, 40, 40), "Tower A", new Color(0.10f, 0.12f, 0.18f));
            BuildBuilding(new Vector3(-80, 15, 80), new Vector3(30, 30, 30), "Tower B", new Color(0.14f, 0.16f, 0.22f));
            BuildBuilding(new Vector3(80, 12, -80), new Vector3(30, 24, 30), "Apartment", new Color(0.16f, 0.18f, 0.20f));
            BuildBuilding(new Vector3(-80, 10, -80), new Vector3(24, 20, 24), "Warehouse", new Color(0.14f, 0.10f, 0.10f));
            BuildBuilding(new Vector3(160, 8, 30), new Vector3(24, 16, 24), "Police HQ", new Color(0.10f, 0.20f, 0.45f));
            BuildBuilding(new Vector3(160, 8, -30), new Vector3(24, 16, 24), "Hospital", new Color(0.20f, 0.45f, 0.35f));
            BuildBuilding(new Vector3(-160, 8, 30), new Vector3(24, 16, 24), "Mall", new Color(0.35f, 0.20f, 0.45f));
            BuildBuilding(new Vector3(-160, 8, -30), new Vector3(24, 16, 24), "Fuel Station", new Color(0.50f, 0.35f, 0.10f));

            for (int x = -3; x <= 3; x++)
                for (int z = -1; z <= 1; z++)
                {
                    if (Mathf.Abs(x) < 1 && Mathf.Abs(z) < 1) continue;
                    BuildBuilding(new Vector3(x * 20 + 260, 3, z * 20 + 160), new Vector3(12, 6, 12),
                                  $"House_{x}_{z}", new Color(0.20f, 0.18f, 0.15f));
                }

            for (int i = 0; i < 6; i++)
                BuildBuilding(new Vector3(-260 + i * 30, 6, -160), new Vector3(24, 12, 20),
                              $"Factory_{i}", new Color(0.16f, 0.12f, 0.08f));

            var park = GameObject.CreatePrimitive(PrimitiveType.Cube);
            park.name = "Park"; park.transform.position = new Vector3(0, 0.05f, 260);
            park.transform.localScale = new Vector3(120, 0.1f, 60);
            Paint(park, new Color(0.15f, 0.35f, 0.18f)); _spawned.Add(park);
            for (int i = 0; i < 20; i++)
                BuildTree(new Vector3(Random.Range(-50, 50), 0, 260 + Random.Range(-25, 25)));

            var beach = GameObject.CreatePrimitive(PrimitiveType.Cube);
            beach.name = "Beach"; beach.transform.position = new Vector3(-260, 0.05f, 260);
            beach.transform.localScale = new Vector3(80, 0.1f, 80);
            Paint(beach, new Color(0.85f, 0.78f, 0.55f)); _spawned.Add(beach);

            var sea = GameObject.CreatePrimitive(PrimitiveType.Cube);
            sea.name = "Sea"; sea.transform.position = new Vector3(-260, -0.5f, 340);
            sea.transform.localScale = new Vector3(200, 1, 60);
            Paint(sea, new Color(0.06f, 0.15f, 0.28f)); _spawned.Add(sea);

            BuildBuilding(new Vector3(-260, 3, 200), new Vector3(60, 6, 20), "Port Warehouse",
                          new Color(0.35f, 0.25f, 0.12f));

            BuildBridge(new Vector3(-100, 6, 340), new Vector3(120, 3, 12));
            BuildTunnel(new Vector3(0, 0, -260), new Vector3(60, 8, 20));

            for (int x = -3; x <= 3; x++) { BuildStreetLight(new Vector3(x * 80, 0, 6.5f)); BuildStreetLight(new Vector3(x * 80, 0, -6.5f)); }
            for (int z = -3; z <= 3; z++) { BuildStreetLight(new Vector3(6.5f, 0, z * 80)); BuildStreetLight(new Vector3(-6.5f, 0, z * 80)); }
            BuildTrafficLight(new Vector3(15, 0, 15));
            BuildTrafficLight(new Vector3(-15, 0, 15));
            BuildTrafficLight(new Vector3(15, 0, -15));
            BuildTrafficLight(new Vector3(-15, 0, -15));

            BuildBuilding(new Vector3(40, 4, 40), new Vector3(14, 8, 14), "Safehouse",
                          new Color(0.25f, 0.22f, 0.35f));
        }

        private void BuildRoad(Vector3 c, Vector3 size)
        {
            var r = GameObject.CreatePrimitive(PrimitiveType.Cube);
            r.name = "Road"; r.transform.position = c; r.transform.localScale = size;
            Paint(r, new Color(0.14f, 0.14f, 0.16f)); _spawned.Add(r);
            var line = GameObject.CreatePrimitive(PrimitiveType.Cube);
            line.name = "Lane"; line.transform.position = c + new Vector3(0, 0.02f, 0);
            line.transform.localScale = size.z > size.x
                ? new Vector3(0.25f, 0.03f, size.z * 0.98f)
                : new Vector3(size.x * 0.98f, 0.03f, 0.25f);
            Paint(line, new Color(1f, 0.95f, 0.5f)); _spawned.Add(line);
        }

        private GameObject BuildBuilding(Vector3 pos, Vector3 size, string name, Color color)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = "Bldg_" + name; go.transform.position = pos; go.transform.localScale = size;
            Paint(go, color); _spawned.Add(go); return go;
        }

        private void BuildTree(Vector3 pos)
        {
            var trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            trunk.transform.position = pos + new Vector3(0, 1.5f, 0);
            trunk.transform.localScale = new Vector3(0.4f, 1.5f, 0.4f);
            Paint(trunk, new Color(0.30f, 0.20f, 0.12f)); _spawned.Add(trunk);
            var leaves = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            leaves.transform.position = pos + new Vector3(0, 3.5f, 0);
            leaves.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
            Paint(leaves, new Color(0.10f, 0.45f, 0.15f)); _spawned.Add(leaves);
        }

        private void BuildBridge(Vector3 c, Vector3 size)
        {
            var deck = GameObject.CreatePrimitive(PrimitiveType.Cube);
            deck.transform.position = c; deck.transform.localScale = size;
            Paint(deck, new Color(0.35f, 0.35f, 0.40f)); _spawned.Add(deck);
            for (int i = -1; i <= 1; i += 2)
            {
                var rail = GameObject.CreatePrimitive(PrimitiveType.Cube);
                rail.transform.position = c + new Vector3(0, 1f, i * size.z * 0.45f);
                rail.transform.localScale = new Vector3(size.x, 1f, 0.5f);
                Paint(rail, new Color(0.60f, 0.60f, 0.65f)); _spawned.Add(rail);
            }
        }

        private void BuildTunnel(Vector3 c, Vector3 size)
        {
            for (int i = -1; i <= 1; i += 2)
            {
                var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                wall.transform.position = c + new Vector3(0, size.y * 0.5f, i * size.z * 0.5f);
                wall.transform.localScale = new Vector3(size.x, size.y, 0.8f);
                Paint(wall, new Color(0.30f, 0.30f, 0.35f)); _spawned.Add(wall);
            }
            var top = GameObject.CreatePrimitive(PrimitiveType.Cube);
            top.transform.position = c + new Vector3(0, size.y, 0);
            top.transform.localScale = new Vector3(size.x, 0.8f, size.z);
            Paint(top, new Color(0.24f, 0.24f, 0.28f)); _spawned.Add(top);
        }

        private void BuildStreetLight(Vector3 pos)
        {
            var pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pole.transform.position = pos + new Vector3(0, 3f, 0);
            pole.transform.localScale = new Vector3(0.15f, 3f, 0.15f);
            Paint(pole, new Color(0.20f, 0.20f, 0.24f)); _spawned.Add(pole);
            var bulb = new GameObject("Bulb");
            bulb.transform.position = pos + new Vector3(0, 6f, 0);
            var lt = bulb.AddComponent<Light>();
            lt.type = LightType.Point; lt.range = 12f; lt.intensity = 1.4f;
            lt.color = new Color(1f, 0.85f, 0.55f); _spawned.Add(bulb);
        }

        private void BuildTrafficLight(Vector3 pos)
        {
            var go = new GameObject("TrafficLight");
            go.transform.position = pos;
            var pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pole.transform.SetParent(go.transform, false);
            pole.transform.localPosition = new Vector3(0, 2.5f, 0);
            pole.transform.localScale = new Vector3(0.2f, 2.5f, 0.2f);
            Paint(pole, new Color(0.20f, 0.20f, 0.24f));
            var head = GameObject.CreatePrimitive(PrimitiveType.Cube);
            head.transform.SetParent(go.transform, false);
            head.transform.localPosition = new Vector3(0, 5f, 0);
            head.transform.localScale = new Vector3(0.6f, 1.4f, 0.4f);
            Paint(head, new Color(0.10f, 0.10f, 0.12f));
            go.AddComponent<TrafficLightController>();
            go.AddComponent<NeonCipher.Hacking.SignalOverrideableLight>();
            _spawned.Add(go);
        }

        private void Paint(GameObject go, Color c)
        {
            var mr = go.GetComponent<MeshRenderer>();
            if (mr == null) return;
            var shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            if (shader == null) return;
            var mat = new Material(shader);
            mat.color = c;
            mr.sharedMaterial = mat;
        }

        private void BuildPlayer()
        {
            var root = new GameObject("Player");
            root.tag = "Player";
            root.transform.position = new Vector3(0, 1, 0);

            var body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            body.transform.SetParent(root.transform, false);
            body.transform.localPosition = new Vector3(0, 1, 0);
            body.transform.localScale = new Vector3(0.6f, 0.9f, 0.6f);
            Paint(body, new Color(0.35f, 0.55f, 0.75f));
            var bc = body.GetComponent<CapsuleCollider>(); if (bc != null) Object.Destroy(bc);

            var head = GameObject.CreatePrimitive(PrimitiveType.Cube);
            head.transform.SetParent(root.transform, false);
            head.transform.localPosition = new Vector3(0, 2.1f, 0);
            head.transform.localScale = new Vector3(0.45f, 0.45f, 0.45f);
            Paint(head, new Color(0.82f, 0.70f, 0.58f));
            var hc = head.GetComponent<BoxCollider>(); if (hc != null) Object.Destroy(hc);

            for (int i = -1; i <= 1; i += 2)
            {
                var arm = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                arm.transform.SetParent(root.transform, false);
                arm.transform.localPosition = new Vector3(i * 0.45f, 1.2f, 0);
                arm.transform.localScale = new Vector3(0.14f, 0.5f, 0.14f);
                Paint(arm, new Color(0.30f, 0.45f, 0.65f));
                var ac = arm.GetComponent<CapsuleCollider>(); if (ac != null) Object.Destroy(ac);

                var leg = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                leg.transform.SetParent(root.transform, false);
                leg.transform.localPosition = new Vector3(i * 0.18f, 0.3f, 0);
                leg.transform.localScale = new Vector3(0.16f, 0.55f, 0.16f);
                Paint(leg, new Color(0.14f, 0.14f, 0.18f));
                var lc = leg.GetComponent<CapsuleCollider>(); if (lc != null) Object.Destroy(lc);
            }

            var cc = root.AddComponent<CharacterController>();
            cc.center = new Vector3(0, 1f, 0); cc.height = 1.9f; cc.radius = 0.32f;
            Player = root.AddComponent<PlayerController>();
            _spawned.Add(root);
        }

        private void BuildCamera()
        {
            var camGo = new GameObject("MainCamera");
            camGo.tag = "MainCamera";
            var cam = camGo.AddComponent<UnityEngine.Camera>();
            cam.clearFlags = CameraClearFlags.Skybox;
            cam.backgroundColor = new Color(0.05f, 0.08f, 0.13f);
            camGo.AddComponent<AudioListener>();

            CameraRig = camGo.AddComponent<ThirdPersonCameraRig>();
            CameraRig.Target = Player.transform;
            Player.CameraRig = camGo.transform;
            _spawned.Add(camGo);
        }

        private void BuildCar()
        {
            var root = new GameObject("Car");
            root.transform.position = new Vector3(10, 0.7f, 0);
            var body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body.transform.SetParent(root.transform, false);
            body.transform.localScale = new Vector3(2f, 0.9f, 4.4f);
            Paint(body, new Color(0.75f, 0.15f, 0.15f));
            var cabin = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cabin.transform.SetParent(root.transform, false);
            cabin.transform.localPosition = new Vector3(0, 0.75f, -0.2f);
            cabin.transform.localScale = new Vector3(1.8f, 0.8f, 2.4f);
            Paint(cabin, new Color(0.06f, 0.10f, 0.16f));
            var rb = root.AddComponent<Rigidbody>(); rb.mass = 1500f;
            Car = root.AddComponent<VehicleController>(); Car.SetKind(VehicleKind.Car);
            AttachWheel(root.transform, new Vector3(0.95f, -0.3f, 1.6f));
            AttachWheel(root.transform, new Vector3(-0.95f, -0.3f, 1.6f));
            AttachWheel(root.transform, new Vector3(0.95f, -0.3f, -1.6f));
            AttachWheel(root.transform, new Vector3(-0.95f, -0.3f, -1.6f));
            var interact = root.AddComponent<VehicleInteractable>();
            interact.Kind = VehicleKind.Car; interact.Vehicle = Car;
            _spawned.Add(root);
        }

        private void BuildBike()
        {
            var root = new GameObject("Bike");
            root.transform.position = new Vector3(-10, 0.6f, 0);
            var body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body.transform.SetParent(root.transform, false);
            body.transform.localScale = new Vector3(0.5f, 0.6f, 2.1f);
            Paint(body, new Color(0.10f, 0.10f, 0.14f));
            var seat = GameObject.CreatePrimitive(PrimitiveType.Cube);
            seat.transform.SetParent(root.transform, false);
            seat.transform.localPosition = new Vector3(0, 0.4f, -0.2f);
            seat.transform.localScale = new Vector3(0.5f, 0.15f, 0.6f);
            Paint(seat, new Color(0.55f, 0.35f, 0.20f));
            var rb = root.AddComponent<Rigidbody>(); rb.mass = 220f;
            rb.centerOfMass = new Vector3(0, -0.2f, 0);
            Bike = root.AddComponent<VehicleController>(); Bike.SetKind(VehicleKind.Bike);
            AttachWheel(root.transform, new Vector3(0, -0.35f, 0.9f));
            AttachWheel(root.transform, new Vector3(0, -0.35f, -0.9f));
            var interact = root.AddComponent<VehicleInteractable>();
            interact.Kind = VehicleKind.Bike; interact.Vehicle = Bike;
            _spawned.Add(root);
        }

        private void AttachWheel(Transform parent, Vector3 localPos)
        {
            var w = new GameObject("Wheel");
            w.transform.SetParent(parent, false);
            w.transform.localPosition = localPos;
            var wc = w.AddComponent<WheelCollider>();
            wc.radius = 0.35f; wc.suspensionDistance = 0.15f;
            var vis = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            var vc = vis.GetComponent<CapsuleCollider>(); if (vc != null) Object.Destroy(vc);
            vis.transform.SetParent(w.transform, false);
            vis.transform.localRotation = Quaternion.Euler(0, 0, 90);
            vis.transform.localScale = new Vector3(0.35f, 0.12f, 0.35f);
            Paint(vis, new Color(0.04f, 0.04f, 0.06f));
        }

        private void BuildMission()
        {
            if (!GameServices.Current.TryGet<MissionController>(out var missions)) return;
            var so = ScriptableObject.CreateInstance<MissionSO>();
            so.Id = "M01_FirstSignal";
            so.DisplayName = "First Signal";
            so.Briefing = "Reach the plaza, drive to the Port, return home.";
            so.Steps.Add(new MissionSO.Step
            {
                Id = "S1", Title = "Walk to the plaza waypoint",
                Description = "Head east on Main Ave.", ObjectiveWorldPos = new Vector3(60, 1, 0), Radius = 4f
            });
            so.Steps.Add(new MissionSO.Step
            {
                Id = "S2", Title = "Take the car and reach the Port",
                Description = "Enter the red car and drive to the Port Warehouse.",
                ObjectiveWorldPos = new Vector3(-260, 3, 200), Radius = 8f
            });
            so.Steps.Add(new MissionSO.Step
            {
                Id = "S3", Title = "Return home to the Safehouse",
                Description = "Drive back to the Safehouse to complete the mission.",
                ObjectiveWorldPos = new Vector3(40, 4, 40), Radius = 6f
            });
            so.RewardMoney = 1500; so.RewardXp = 250;
            missions.Register(so);
            missions.Start(so.Id);
            for (int i = 0; i < so.Steps.Count; i++)
            {
                var s = so.Steps[i];
                var t = new GameObject($"MissionTrigger_{s.Id}");
                t.transform.position = s.ObjectiveWorldPos + Vector3.up * 1.5f;
                var col = t.AddComponent<SphereCollider>();
                col.isTrigger = true; col.radius = s.Radius;
                var mt = t.AddComponent<MissionTrigger>();
                mt.MissionId = so.Id; mt.StepRequired = i;
                var beacon = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                beacon.transform.SetParent(t.transform, false);
                beacon.transform.localScale = new Vector3(1.5f, 6f, 1.5f);
                var bc = beacon.GetComponent<CapsuleCollider>(); if (bc != null) Object.Destroy(bc);
                Paint(beacon, new Color(0.35f, 0.95f, 1f));
                _spawned.Add(t);
            }
        }
    }
}
