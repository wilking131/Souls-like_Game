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
        [SerializeField]   //��inspector������ʾ
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

            //������ķ�����ǰ�����ó��˶�����
            moveDirection = cameraObject.forward * inputHandler.vertical;
            moveDirection += cameraObject.right * inputHandler.horizontal;
            moveDirection.Normalize();

            float speed = movementSpeed;
            moveDirection *= speed;

            //�˶������ƽ��ͶӰ
            Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
            //�ı�������ٶ�
            rigidbody.linearVelocity = projectedVelocity;

            animatorHandler.UpdateAnimatorValues(inputHandler.moveAmount, 0);

            if (animatorHandler.canRotate)
            {
                HandleRotation(delta);
            }
        }

        #region Movement
        Vector3 normalVector = new Vector3(0, 1, 0);
        Vector3 targetPosition;

        /// <summary>
        /// ��ת����
        /// ͨ�����������ȷ������ķ���Ȼ��ͨ��Slerpȥ�ı�����ĳ���
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

