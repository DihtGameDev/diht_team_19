using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Animals
{
    public abstract class Ability
    {
        public abstract string GetName();
        public abstract bool Available(PredatorController caster, PredatorController other);
        public abstract void Apply(PredatorController caster, PredatorController other);
    }

    public class Roar : Ability
    {
        private float power_;
        private float lowerBound_;
        private float range_;
        private bool ready = true;

        private IEnumerator Reload()
        {
            yield return new WaitForSeconds(15);
            ready = true;
        }
        
        public Roar(float power, float lowerBound, float range)
        {
            power_ = power;
            lowerBound_ = lowerBound;
            range_ = range;
        }

        public override string GetName()
        {
            return "Roar";
        }

        public override bool Available(PredatorController caster, PredatorController other)
        {
            return ready && (caster.transform.position - other.transform.position).magnitude < range_;
        }

        public override void Apply(PredatorController caster, PredatorController other)
        {
            if (caster == null || other == null)
            {
                Debug.Log("Got Null caster or target");
                return;
            }
            Debug.Log("Applying roar!");
            if (Random.Range(0, 100) < power_)
            {
                other.Scare(caster);
            }
            power_ = Math.Max(lowerBound_, power_ - 10);
            ready = false;
            GameController.Get().StartCoroutine("Reload");
        }
    }
}