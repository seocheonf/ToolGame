using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace SpecialInteraction
{
    [Serializable]
    public struct WindData
    {
        //����
        public float intensity;
        
        //���� ������
        [SerializeField]
        public Quaternion offset;
        
        //���� ����
        private Vector3 direction;
        //���� ������ �����ϰ�, ����ȭ
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
        
        const float reactionRatio = 0.3f;


#if UNITY_EDITOR

        //�ٶ� ���� �����ִ� ����׿� �Լ�
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            
            Gizmos.DrawRay(transform.position, transform.rotation * windData.offset * Vector3.forward);

        }

        //�׸����
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
        /// ���ӻ� ���� ���� ������ �ʱ�ȭ�� ���� �Լ�. ������ �⺻ �� ���� ���� ��.
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

            WindStart();
        }

        protected override void MyDestroy()
        {
            base.MyDestroy();

            WindDestory();
        }


        private void CustomFixedUpdate(float fixedDeltaTime)
        {

            //���� �� ����
            windData.Direction = transform.rotation * windData.offset * Vector3.forward;

            //��õ���� ���ۿ�.
            windData.origin.AddForce(reactionRatio * windData.intensity * -windData.Direction, ForceType.VelocityForce);

        }

        //������ ����.
        private void OnTriggerStay(Collider target)
        {
            if(target.TryGetComponent(out PhysicsInteractionObject result))
            {
                result.GetSpecialInteraction(windData);
            }
        }




        [SerializeField]
        ParticleSystem windParticle;

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
            windParticle.Play();
        }

        protected void WindDestory()
        {
            windActive = false;
            GameManager.ObjectsFixedUpdate -= CustomFixedUpdate;
            windCollider.enabled = false;
            windParticle.Stop();
        }

    }


}

