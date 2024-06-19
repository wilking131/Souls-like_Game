using UnityEngine;

namespace ZhouYu
{
    public class PlayerLocomotion : MonoBehaviour
    {
        Transform cameraObject;
        InputHandler inputHandler;
        Vector3 moveDirection;

        [HideInInspector]
        public Transform myTransform;
        [HideInInspector]
        public AnimatorHandler animatorHandler;


        public new Rigidbody rigidbody;
        public GameObject normaCamera;

        [Header("Stats")]
        [SerializeField]   //在inspector窗口显示
        float movementSpeed = 5;

        [SerializeField]
        float rotationSpeed = 10;


        void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
            inputHandler = GetComponent<InputHandler>();
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
            cameraObject = Camera.main.transform;
            myTransform = this.transform;
            animatorHandler.Initialize();
        }

        public void Update()
        {
            float delta = Time.deltaTime;
            inputHandler.TickInput(delta);

            //摄像机的方向是前方，得出运动方向
            moveDirection = cameraObject.forward * inputHandler.vertical;
            moveDirection += cameraObject.right * inputHandler.horizontal;
            moveDirection.Normalize();
            moveDirection.y = 0;

            float speed = movementSpeed;
            moveDirection *= speed;

            //运动方向对平面投影,但是我觉得这行代码没有什么用
            Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);

            //改变物体的速度
            rigidbody.linearVelocity = projectedVelocity;

            animatorHandler.UpdateAnimatorValues(inputHandler.moveAmount, 0);

            if (animatorHandler.canRotate)
            {
                HandleRotation(delta);
            }
        }

        #region Movement
        Vector3 normalVector;
        Vector3 targetPosition;

        /// <summary>
        /// 旋转物体
        /// 通过摄像机方向确定输入的方向，然后通过Slerp去改变物体的朝向
        /// </summary>
        /// <param name="delta"></param>
        private void HandleRotation(float delta)
        {
            Vector3 targetDir = Vector3.zero;
            float moveOverride = inputHandler.moveAmount;

            //
            targetDir = cameraObject.forward * inputHandler.vertical;
            targetDir += cameraObject.right * inputHandler.horizontal;

            targetDir.Normalize();
            targetDir.y = 0;

            if(targetDir == Vector3.zero)
                targetDir = myTransform.forward;

            float rs = rotationSpeed;

            Quaternion tr = Quaternion.LookRotation(targetDir);
            Quaternion targetRotation = Quaternion.Slerp(myTransform.rotation, tr, rs * delta);

            myTransform.rotation = targetRotation;

        }



        #endregion
    }
}

