using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpecialInteraction
{
    [Serializable]
    public struct FireData
    {
        [SerializeField] float lifeTime;
        public float wantDuration;
        public float backBounce;
        public float upBounce;
        public Character detectedCharacter;
    }

    public class Fire : SpecialInteractionObject
    {
        [SerializeField] FireData fireData;

        private void OnTriggerEnter(Collider other)
        {
            //if (TryGetComponent(out PhysicsInteractionObject result))
            //{
            //    result.GetSpecialInteraction(fireData);
            //}
            if (other.GetComponent<Character>())
            {
                fireData.detectedCharacter = other.GetComponent<Character>();
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.TryGetComponent(out PhysicsInteractionObject result))
            {
                result.GetSpecialInteraction(fireData);
            }
            
        }

        private void OnTriggerExit(Collider other)
        {
            fireData.detectedCharacter = null;
        }
        protected override void RegisterFuncInInitialize()
        {
            
        }
        protected override void RemoveFuncInDestroy()
        {

        }
    }
}
