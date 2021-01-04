using Mirror;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TopDownShooter
{
    public class PlayerInput : NetworkBehaviour, iInput
    {
        public Vector2 lookDirection { get; set; }
        public Vector2 moveDirection { get; set; }
        public OnFire fireStart { get; set; }
        public OnFire altFireStart { get; set; }
        public OnFire fireEnded { get; set; }
        public OnFire altFireEnded { get; set; }

        public OnFire onExecution { get; set; }

        private PlayerControls Controls;

        private Camera mainCamera;

        private bool initialized;

        private bool Initialize()
        {
            if (!hasAuthority) return false;

            Debug.Log("Initialized");

            mainCamera = Camera.main;

            Controls = new PlayerControls();
            Controls.Enable();

            initialized = true;

            return true;
        }

        private void OnEnable()
        {
            if (initialized == false)
            {
                Initialize();

                Invoke(nameof(OnEnable), 0.1f);
                return;
            }

            Controls.Default.AimPosition.performed += AimPosition_performed;

            Controls.Default.Movement.performed += Movement_performed;
            Controls.Default.Movement.canceled += Movement_performed;

            Controls.Default.Fire.started += _ => fireStart?.Invoke();
            Controls.Default.AltFire.started += _ => altFireStart?.Invoke();

            Controls.Default.Fire.canceled += _ => fireEnded?.Invoke();
            Controls.Default.AltFire.canceled += _ => altFireEnded?.Invoke();
        }

        private void OnDisable()
        {
            if (!hasAuthority) return;

            if (Controls != null)
            {
                Controls.Default.AimPosition.performed -= AimPosition_performed;

                Controls.Default.Movement.performed -= Movement_performed;
                Controls.Default.Movement.canceled -= Movement_performed;

                Controls.Default.Fire.started -= _ => fireStart?.Invoke();
                Controls.Default.AltFire.started -= _ => altFireStart?.Invoke();

                Controls.Default.Fire.canceled -= _ => fireEnded?.Invoke();
                Controls.Default.AltFire.canceled -= _ => altFireEnded?.Invoke();
            }
        }

        private void Movement_performed(InputAction.CallbackContext obj)
        {
            moveDirection = obj.ReadValue<Vector2>();
        }

        private void AimPosition_performed(InputAction.CallbackContext obj)
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
                return;
            }

            Vector3 mouseScreenPosition = mainCamera.ScreenToWorldPoint(obj.ReadValue<Vector2>());

            lookDirection = mouseScreenPosition;
        }
    }
}