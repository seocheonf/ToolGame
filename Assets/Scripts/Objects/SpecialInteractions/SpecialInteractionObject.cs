using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//������ �ܾ� ������� ���� namespace ����
namespace SpecialInteraction
{
    [RequireComponent(typeof(Collider), typeof(Rigidbody))]
    public class SpecialInteractionObject : PhysicsInteractionObject
    {
        //Ư����ȣ�ۿ��� Ư����ȣ�ۿ��� �޾��� �� �ϴ� ���� ����.
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
