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
            RocketAttack();
        }
    }


    private void RocketAttack()
    {
        if (Target && Target.IsExist())
        {
            var Rocket1 = Instantiate(Rocket, RocketSlot1.position, RocketSlot1.rotation);
            var Rocket2 = Instantiate(Rocket, RocketSlot2.position, RocketSlot2.rotation);

            Rocket1.GetComponent<RocketScript>().Target = Target;
            Rocket2.GetComponent<RocketScript>().Target = Target;
            Rocket1.GetComponent<Rigidbody>().velocity = _rigidbody.velocity;
            Rocket2.GetComponent<Rigidbody>().velocity = _rigidbody.velocity;
        }
    }
}
