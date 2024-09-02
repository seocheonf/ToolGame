using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpecialInteraction
{

    [Serializable]
    public struct WaterData
    {
        //세기
        public float intensity;
        //양
        public float amount;

        //방향 오프셋
        [SerializeField]
        public Quaternion offset;

        //방향
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
            //방향 값 갱신
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

        //바람 방향 보여주는 디버그용 함수
        private void OnDrawGizmos()
        {

            Gizmos.color = Color.red;

            Gizmos.DrawRay(transform.position, transform.rotation * waterData.offset * Vector3.forward * 10f);

        }

        //그리기용
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
