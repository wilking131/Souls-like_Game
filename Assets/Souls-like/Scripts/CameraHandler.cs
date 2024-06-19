using UnityEngine;

namespace ZhouYu
{
    public class CameraHandler : MonoBehaviour
    {
        public Transform targetTransform;           //Player
        public Transform cameraTransform;           //camera    
        public Transform cameraPivotTransform;      //cameraPivot   ��X����ת
        private Transform myTransform;              //cameraHolder  ��Y����ת
        private Vector3 cameraTransformPosition;    //camera����ʱ����м����
        private LayerMask ignoreLayers;
        private Vector3 cameraFollowVelocity = Vector3.zero;  //��ǰ�ƶ����ٶȣ���SmoothDamp�����л�ȡ

        public static CameraHandler singleton;

        public float lookSpeed = 0.1f;              //�ӽ�������ת�ٶ�
        public float followSpeed = 0.1f;            //��������ɫ�ٶ�
        public float pivotSpeed = 0.03f;            //�ӽǸ����ٶ�

        private float targetPosition;
        private float defaultPosition;              //cameraĬ��Z�᳤��
        private float lookAngle;
        private float pivotAngle;
        public float minimumPivot = -35;
        public float maximumPivot = 35;

        public float cameraSphereRadius = 0.2f;
        public float cameraCollisionOffset = 0.2f;
        public float minimumCollisionOffset = 0.2f;


        private void Awake()
        {
            singleton = this;
            myTransform = transform;
            defaultPosition = cameraTransform.position.z;
            ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10);  //��λȡ�������Ժ��� 8,9,10��
        }

        /// <summary>
        /// �������Ŀ��
        /// </summary>
        /// <param name="delta"></param>
        public void Followtarget(float delta)
        {
            //�ȿ������������£
            //Vector3 targetPosition = Vector3.Lerp(myTransform.position, targetTransform.position, delta / followSpeed);
            Vector3 targetPosition = Vector3.SmoothDamp(myTransform.position, targetTransform.position, ref cameraFollowVelocity, delta / followSpeed);
            myTransform.position = targetPosition;

            HandleCameraCollision(delta);
        }

        public void HandleCameraRotation(float delta, float mouseXInput, float mouseYInput)
        {
            //����
            lookAngle += (mouseXInput * lookSpeed) / delta;
            //����
            pivotAngle -= (mouseYInput * pivotSpeed) / delta;
            pivotAngle = Mathf.Clamp(pivotAngle, minimumPivot, maximumPivot);

            //cameraHoulder��Y����ת
            Vector3 rotation = Vector3.zero;
            rotation.y = lookAngle;
            Quaternion targetRotation = Quaternion.Euler(rotation);
            myTransform.rotation = targetRotation;

            //cameraPivot��x�������ת
            rotation = Vector3.zero;
            rotation.x = pivotAngle;
            targetRotation = Quaternion.Euler(rotation);
            cameraPivotTransform.localRotation = targetRotation;
        }

        private void HandleCameraCollision(float delta)
        {
            print(cameraTransform.localPosition);
            //targetPosition�Ǹ�������
            targetPosition = defaultPosition;
            RaycastHit hit;
            Vector3 direction = cameraTransform.position - cameraPivotTransform.position;
            direction.Normalize();
  
            //��ɫ�����֮����м��
            if(Physics.SphereCast(
                cameraPivotTransform.position, cameraSphereRadius, direction, out hit, Mathf.Abs(targetPosition), ignoreLayers)
                )
            {
                float dis = Vector3.Distance(cameraPivotTransform.position, hit.point);
                targetPosition = -(dis - cameraCollisionOffset);
            }

            //�������̾���
            if(Mathf.Abs(targetPosition) < minimumCollisionOffset)
            {
                targetPosition = -minimumCollisionOffset;
            }

            cameraTransformPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, delta / 0.2f);
            cameraTransform.localPosition = cameraTransformPosition;

        }

    }
}

