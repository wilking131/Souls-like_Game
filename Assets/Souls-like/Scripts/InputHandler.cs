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

        PlayerControls inputActions;

        Vector2 movementInput;
        Vector2 cameraInput;

        public void OnEnable()
        {
            if (inputActions == null)
            {
                inputActions = new PlayerControls();

                //���Ӵ������������߼�
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
