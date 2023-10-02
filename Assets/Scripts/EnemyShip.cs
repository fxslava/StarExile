using UnityEngine;


[RequireComponent(typeof(AircraftPhysicalControlSystem))]
public class EnemyShip : MonoBehaviour
{
    [Header("Guidance system")]
    [SerializeField] public Targetable Target = null;
    [SerializeField] public float AttackRadius = 5.0f;

    [Header("Weapon system")]
    [SerializeField] public Transform RocketSlot1 = null;
    [SerializeField] public Transform RocketSlot2 = null;

    [Header("AI")]
    [SerializeField] public float RocketAttackPeriod = 20.0f;
    [SerializeField] public GameObject Rocket;

    private AircraftPhysicalControlSystem _aircraftControlSystem;

    private float _rocketAttackTime;
    private Rigidbody _rigidbody;


    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _aircraftControlSystem = GetComponent<AircraftPhysicalControlSystem>();
        _rocketAttackTime = 0f;
    }


    private void FixedUpdate()
    {
        _rocketAttackTime += Time.fixedDeltaTime;

        if (_aircraftControlSystem && Target && Target.IsExist())
        {
            var currentPosition = transform.position;
            var targetPosition = Target.GetPosition();
            var toTargetVector = targetPosition - currentPosition;
            _aircraftControlSystem.SetTarget(toTargetVector, AttackRadius);
        }

        if (_rocketAttackTime > RocketAttackPeriod)
        {
            _rocketAttackTime = 0f;
            RocketAttack(RocketSlot1);
            RocketAttack(RocketSlot2);
        }
    }


    private void RocketAttack(Transform RocketSlot)
    {
        if (Target && Target.IsExist())
        {
            var rocket = Instantiate(Rocket, RocketSlot.position, RocketSlot.rotation);

            rocket.GetComponent<RocketScript>().Target = Target;
            rocket.GetComponent<Rigidbody>().velocity = _rigidbody.velocity;
        }
    }
}
