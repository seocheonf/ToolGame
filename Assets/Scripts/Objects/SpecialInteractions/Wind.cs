using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using ToolGame;

namespace SpecialInteraction
{
    [Serializable]
    public struct WindData
    {
        //세기
        public float intensity;
        
        //방향 오프셋
        [SerializeField]
        public Quaternion offset;
        
        //현재 방향
        private Vector3 direction;
        //현재 방향을 수정하고, 정규화
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
        public PhysicsInteractionObject origin;
    }

    public class Wind : SpecialInteractionObject
    {
        [SerializeField]
        WindData windData;
        [SerializeField]
        Collider windCollider;
        [SerializeField]
        bool initialOnOff;
        
        const float reactionRatio = 0.3f;


#if UNITY_EDITOR

        //바람 방향 보여주는 디버그용 함수
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            
            Gizmos.DrawRay(transform.position, transform.rotation * windData.offset * Vector3.forward);

        }

        //그리기용
        public void GetWindData(out WindData result)
        {
            result = windData;
        }
        public void SetWindData(ref Quaternion result)
        {
            windData.offset = result;
        }

#endif

        /*
        /// <summary>
        /// 게임상 로직 보다 이전에 초기화를 위한 함수. 일종의 기본 값 세팅 같은 것.
        /// </summary>
        private void Awake()
        {
            windCollider.isTrigger = true;
            windCollider.enabled = false;
        }
        */

        protected override void Initialize()
        {
            base.Initialize();

            if (initialOnOff)
                WindStart();
            else
                WindDestory();
        }

        protected override void MyDestroy()
        {
            base.MyDestroy();

            WindDestory();
        }


        private void CustomFixedUpdate(float fixedDeltaTime)
        {

            //방향 값 갱신
            windData.Direction = transform.rotation * windData.offset * Vector3.forward;

            //원천에게 반작용.
            windData.origin.AddForce(reactionRatio * windData.intensity * -windData.Direction, ForceType.VelocityForce);

        }

        //감지를 하자.
        private void OnTriggerStay(Collider target)
        {
            if(target.TryGetComponent(out PhysicsInteractionObject result))
            {
                result.GetSpecialInteraction(windData);
            }
        }




        [SerializeField]
        ParticleSystem[] windParticle;

        private bool windActive = false;
        public bool WindActive => windActive;
        public void WindSetActive(bool isActive)
        {
            if (isActive == this.windActive)
                return;

            if(isActive)
            {
                WindStart();
            }
            else
            {
                WindDestory();
            }
        }

        protected void WindStart()
        {
            windActive = true;
            GameManager.ObjectsFixedUpdate -= CustomFixedUpdate;
            GameManager.ObjectsFixedUpdate += CustomFixedUpdate;
            windCollider.enabled = true;
            foreach (ParticleSystem each in windParticle)
            {
                each.Play();
            }
        }

        protected void WindDestory()
        {
            windActive = false;
            GameManager.ObjectsFixedUpdate -= CustomFixedUpdate;
            windCollider.enabled = false;
            foreach (ParticleSystem each in windParticle)
            {
                each.Stop();
            }
        }

    }


}

