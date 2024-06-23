using UnityEngine;



namespace ZhouYu
{
    //���������߼�
    public class InputHandler : MonoBehaviour
    {
        public float horizontal;    //AD
        public float vertical;      //WS
        public float moveAmount;
        public float mouseX;        
        public float mouseY;

        public bool b_Input;

        public bool rollFlag;
        public bool sprintFlag;
        public float rollInputTimer;
        public bool isInteracting;

        PlayerControls inputActions;        //�����ļ��ű�
        CameraHandler cameraHandler;        //   

        Vector2 movementInput;
        Vector2 cameraInput;

        private void Awake()
        {
            cameraHandler = CameraHandler.singleton;
        }

        public void OnEnable()
        {
            if (inputActions == null)
            {
                inputActions = new PlayerControls();

                //��Ӵ������������߼�
                inputActions.PlayerMovement.Movement.performed += (context) =>
                {
                    movementInput = context.ReadValue<Vector2>();
                };

                inputActions.PlayerMovement.Camera.performed += (context) =>
                {
                    cameraInput = context.ReadValue<Vector2>();
                };
            }
            //����������
            inputActions.Enable();

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

        public void TickInput(float delta)
        {
            MoveInput(delta);
            HandleRollInput(delta);
        }


        private void OnDisable()
        {
            inputActions.Disable();
        }



        private void MoveInput(float delta)
        {
            horizontal = movementInput.x;
            vertical = movementInput.y;
            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
            mouseX = cameraInput.x;
            mouseY = cameraInput.y;
        }

        private void HandleRollInput(float delta)
        {
            b_Input = inputActions.PlayerActions.Roll.phase == UnityEngine.InputSystem.InputActionPhase.Performed;

            //Ĭ���ǳ�̣�Ȼ����ݰ�����ʱ���ж��Ƿ��Ƿ���
            if (b_Input)
            {
                rollInputTimer += delta;
                sprintFlag = true;
            }
            else
            {
                if(rollInputTimer > 0 && rollInputTimer < 0.5f)
                {
                    sprintFlag = false;
                    rollFlag = true;
                }

                rollInputTimer = 0;
            }
        }
    }
}

