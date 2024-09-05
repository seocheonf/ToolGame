using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpecialInteraction
{
    public struct FireData
    {
        float lifeTime;
    }

    public class Fire : SpecialInteractionObject
    {
        [SerializeField] float wantDuration;
        [SerializeField] float backBounce;
        [SerializeField] float upBounce;
        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Character>())
            {
                other.GetComponent<Character>().SetCrowdControl(CrowdControlState.Stun, 0.5f);
                other.GetComponent<Character>().AddForce((-other.transform.forward * backBounce + Vector3.up * upBounce), ForceType.VelocityForce);
            }
        }

        protected override void RegisterFuncInInitialize()
        {
            
        }
        protected override void RemoveFuncInDestroy()
        {

        }
    }
}
