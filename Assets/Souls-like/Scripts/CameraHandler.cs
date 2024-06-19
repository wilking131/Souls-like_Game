using UnityEngine;

namespace ZhouYu
{
    public class CameraHandler : MonoBehaviour
    {
        public Transform targetTransform;           //Player
        public Transform cameraTransform;           //camera    
        public Transform cameraPivotTransform;      //cameraPivot   绕X轴旋转
        private Transform myTransform;              //cameraHolder  绕Y轴旋转
        private Vector3 cameraTransformPosition;    //camera伸缩时候的中间变量
        private LayerMask ignoreLayers;
        private Vector3 cameraFollowVelocity = Vector3.zero;  //当前移动的速度，从SmoothDamp函数中获取

        public static CameraHandler singleton;

        public float lookSpeed = 0.1f;              //视角左右旋转速度
        public float followSpeed = 0.1f;            //相机跟随角色速度
        public float pivotSpeed = 0.03f;            //视角俯仰速度

        private float targetPosition;
        private float defaultPosition;              //camera默认Z轴长度
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
            ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10);  //按位取反，所以忽略 8,9,10层
        }

        /// <summary>
        /// 相机靠近目标
        /// </summary>
        /// <param name="delta"></param>
        public void Followtarget(float delta)
        {
            //先快后慢，慢慢靠拢
            //Vector3 targetPosition = Vector3.Lerp(myTransform.position, targetTransform.position, delta / followSpeed);
            Vector3 targetPosition = Vector3.SmoothDamp(myTransform.position, targetTransform.position, ref cameraFollowVelocity, delta / followSpeed);
            myTransform.position = targetPosition;

            HandleCameraCollision(delta);
        }

        public void HandleCameraRotation(float delta, float mouseXInput, float mouseYInput)
        {
            //左右
            lookAngle += (mouseXInput * lookSpeed) / delta;
            //俯仰
            pivotAngle -= (mouseYInput * pivotSpeed) / delta;
            pivotAngle = Mathf.Clamp(pivotAngle, minimumPivot, maximumPivot);

            //cameraHoulder的Y轴旋转
            Vector3 rotation = Vector3.zero;
            rotation.y = lookAngle;
            Quaternion targetRotation = Quaternion.Euler(rotation);
            myTransform.rotation = targetRotation;

            //cameraPivot的x轴进行旋转
            rotation = Vector3.zero;
            rotation.x = pivotAngle;
            targetRotation = Quaternion.Euler(rotation);
            cameraPivotTransform.localRotation = targetRotation;
        }

        private void HandleCameraCollision(float delta)
        {
            print(cameraTransform.localPosition);
            //targetPosition是负数距离
            targetPosition = defaultPosition;
            RaycastHit hit;
            Vector3 direction = cameraTransform.position - cameraPivotTransform.position;
            direction.Normalize();
  
            //角色和相机之间进行检测
            if(Physics.SphereCast(
                cameraPivotTransform.position, cameraSphereRadius, direction, out hit, Mathf.Abs(targetPosition), ignoreLayers)
                )
            {
                float dis = Vector3.Distance(cameraPivotTransform.position, hit.point);
                targetPosition = -(dis - cameraCollisionOffset);
            }

            //摄像机最短距离
            if(Mathf.Abs(targetPosition) < minimumCollisionOffset)
            {
                targetPosition = -minimumCollisionOffset;
            }

            cameraTransformPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, delta / 0.2f);
            cameraTransform.localPosition = cameraTransformPosition;

        }

    }
}

