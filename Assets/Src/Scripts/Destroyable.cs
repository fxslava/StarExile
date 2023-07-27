using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyable : MonoBehaviour
{
    [SerializeField] public float Health = 100;
    [SerializeField] public ParticleSystem ExplosionPS = null;
    [SerializeField] public GameObject FragmentsObject = null;
    [SerializeField] public Transform ExplosionEpicenter = null;
    [SerializeField] public float ExplosionForce = 100.0f;
    [SerializeField] public float ExplosionRadius = 2.0f;


    public void ApplyDamage(float value)
    {
        Health -= value;
        if (Health <= 0)
        {
            Explode();
        }
    }

    public bool IsAlive()
    {
        return Health > 0;
    }


    private void Explode()
    {
        var fragmentsObjest = Instantiate(FragmentsObject, transform.position, transform.rotation);

        var fragments = fragmentsObjest.GetComponentsInChildren<Rigidbody>();
        if (fragments.Length > 0)
        {
            foreach(var rb in fragments) {
                rb.AddExplosionForce(ExplosionForce, ExplosionEpicenter.position, ExplosionRadius);
            }
        }

        if (ExplosionPS)
        {
            var ps = Instantiate(ExplosionPS, transform.position, transform.rotation);
            Destroy(ps.gameObject, ps.main.duration);
        }

        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
