using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//보편적 단어 사용으로 인한 namespace 묶기
namespace SpecialInteraction
{
    [RequireComponent(typeof(Collider), typeof(Rigidbody))]
    public class SpecialInteractionObject : PhysicsInteractionObject
    {
        //특수상호작용은 특수상호작용을 받았을 때 하는 일이 없음.
        public override void GetSpecialInteraction(WindData source) { }
        public override void GetSpecialInteraction(WaterData source) { }
        public override void GetSpecialInteraction(FireData source) { }

        protected override void RegisterFuncInInitialize()
        {

        }
        protected override void RemoveFuncInDestroy()
        {

        }
    }
}
