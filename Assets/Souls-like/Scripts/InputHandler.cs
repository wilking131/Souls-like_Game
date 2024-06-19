using UnityEngine;



namespace ZhouYu
{
    //处理输入逻辑
    public class InputHandler : MonoBehaviour
    {
        public float horizontal;    //AD
        public float vertical;      //WS
        public float moveAmount;
        public float mouseX;        
        public float mouseY;

        PlayerControls inputActions;
        CameraHandler cameraHandler;

        Vector2 movementInput;
        Vector2 cameraInput;

        private void Awake()
        {
            cameraHandler = CameraHandler.singleton;
        }

        private void FixedUpdate()
        {
            float delta = Time.fixedDeltaTime;

            if (cameraHandler != null)
            {
                cameraHandler.Followtarget(delta);
                cameraHandler.HandleCameraRotation(delta, mouseX, mouseY);
            }
        }



        public void OnEnable()
        {
            if (inputActions == null)
            {
                inputActions = new PlayerControls();

                //添加触发控制器的逻辑
                inputActions.PlayerMovement.Movement.performed += (context) =>
                {
                    movementInput = context.ReadValue<Vector2>();
                };

                inputActions.PlayerMovement.Camera.performed += (context) =>
                {
                    cameraInput = context.ReadValue<Vector2>();
                };
            }
            //启用输入检测
            inputActions.Enable();

        }

        private void OnDisable()
        {
            inputActions.Disable();
        }

        public void TickInput(float delta)
        {
            MoveInput(delta);
        }

        private void MoveInput(float delta)
        {
            horizontal = movementInput.x;
            vertical = movementInput.y;
            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
            mouseX = cameraInput.x;
            mouseY = cameraInput.y;
        }
    }
}

