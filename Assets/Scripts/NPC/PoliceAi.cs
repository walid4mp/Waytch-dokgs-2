// =====================================================================
//  Neon Cipher — Police AI (the Civic Guard)
//  File:    PoliceAi.cs
//  Notes:   Patrol → Investigate (noise) → Pursue → Search → Arrest.
// =====================================================================
using UnityEngine;
using UnityEngine.AI;

namespace NeonCipher.NPC
{
    public enum GuardState { Patrol, Investigate, Chase, Search, Arrest }

    [RequireComponent(typeof(NavMeshAgent))]
    public sealed class CivicGuardAi : MonoBehaviour
    {
        [SerializeField] private Transform[] _patrolRoute;
        [SerializeField] private float       _sightRange = 30f;
        [SerializeField] private float       _catchRange = 1.6f;
        [SerializeField] private float       _searchDuration = 8f;
        [SerializeField] private string      _targetTag = "Player";

        private NavMeshAgent _agent;
        private int          _routeIndex;
        private GuardState   _state = GuardState.Patrol;
        private float        _searchEndTime;
        private Transform    _target;

        public GuardState State => _state;

        private void Awake() => _agent = GetComponent<NavMeshAgent>();

        private void Update()
        {
            switch (_state)
            {
                case GuardState.Patrol:      Patrol();     Detect(); break;
                case GuardState.Investigate: Investigate(); Detect(); break;
                case GuardState.Chase:       Chase();       break;
                case GuardState.Search:      Search();      break;
                case GuardState.Arrest:      Arrest();      break;
            }
        }

        private void Patrol()
        {
            if (_patrolRoute == null || _patrolRoute.Length == 0) return;
            var target = _patrolRoute[_routeIndex].position;
            _agent.SetDestination(target);
            if ((transform.position - target).sqrMagnitude < 2f)
                _routeIndex = (_routeIndex + 1) % _patrolRoute.Length;
        }

        private void Investigate()
        {
            // TODO: move to last noise source (raised by Trap/Hacking signals)
            _state = GuardState.Patrol;
        }

        private void Chase()
        {
            if (_target == null) { _state = GuardState.Search; _searchEndTime = Time.time + _searchDuration; return; }
            _agent.SetDestination(_target.position);
            if (Vector3.Distance(transform.position, _target.position) <= _catchRange)
                _state = GuardState.Arrest;
            if (Vector3.Distance(transform.position, _target.position) > _sightRange * 1.6f)
            { _state = GuardState.Search; _searchEndTime = Time.time + _searchDuration; }
        }

        private void Search()
        {
            _agent.SetDestination(transform.position + UnityEngine.Random.insideUnitSphere * 8f);
            if (Time.time > _searchEndTime) _state = GuardState.Patrol;
        }

        private void Arrest()
        {
            // Cutscene / mini-game trigger
            Debug.Log($"[Guard] {name} arrests the target.");
            _state = GuardState.Patrol;
        }

        private void Detect()
        {
            var player = GameObject.FindGameObjectWithTag(_targetTag);
            if (player == null) return;
            Vector3 eye = transform.position + Vector3.up * 1.7f;
            if (Vector3.Distance(eye, player.transform.position) > _sightRange) return;
            var dir = (player.transform.position - eye).normalized;
            if (Physics.Raycast(eye, dir, out var hit, _sightRange))
            {
                if (hit.transform.CompareTag(_targetTag))
                { _target = player.transform; _state = GuardState.Chase; }
            }
        }

        public void RaiseAlert(Vector3 noisePos) => _state = GuardState.Investigate;
    }
}
