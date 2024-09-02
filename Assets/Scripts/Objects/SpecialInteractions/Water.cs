using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpecialInteraction
{

    [Serializable]
    public struct WaterData
    {
        //����
        public float intensity;
        //��
        public float amount;

        //���� ������
        [SerializeField]
        public Quaternion offset;

        //����
        private Vector3 direction;
        public Vector3 Direction
        {
            get
            {
                return direction.normalized;
            }
            set
            {
                direction = value;
            }
        }
    }

    public class Water : SpecialInteractionObject
    {
        [SerializeField]
        WaterData waterData;

        protected override void MainFixedUpdate(float fixedDeltaTime)
        {
            //���� �� ����
            waterData.Direction = transform.rotation * waterData.offset * Vector3.forward;

            base.MainFixedUpdate(fixedDeltaTime);
        }

        private void OnTriggerStay(Collider target)
        {
            if(target.TryGetComponent(out PhysicsInteractionObject result))
            {
                result.GetSpecialInteraction(waterData);
            }
        }

#if UNITY_EDITOR

        //�ٶ� ���� �����ִ� ����׿� �Լ�
        private void OnDrawGizmos()
        {

            Gizmos.color = Color.red;

            Gizmos.DrawRay(transform.position, transform.rotation * waterData.offset * Vector3.forward * 10f);

        }

        //�׸����
        public void GetWaterData(out WaterData result)
        {
            result = waterData;
        }
        public void SetWaterData(ref Quaternion result)
        {
            waterData.offset = result;
        }

#endif

    }
}
