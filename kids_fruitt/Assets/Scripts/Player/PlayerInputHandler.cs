using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public delegate void MovementInputHandler(Vector2 horizontalInput);
    public delegate void JumpInputHandler();

    public event MovementInputHandler OnMovementInput;
    public event JumpInputHandler OnJumpInput;

    private InputSystem_Actions inputActions;
    private bool canTakeInput = true;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();

        inputActions.Player.Move.performed += HandleMove;
        inputActions.Player.Move.canceled += HandleMove;
        inputActions.Player.Jump.performed += HandleJump;
    }

    private IEnumerator Start()
    {
        DieEvent.Instance.onPlayerDie += SetCanTakeInputToFalse;
        yield return null;
        WinEvent.Instance.onPlayerWin += SetCanTakeInputToFalse;
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void HandleMove(InputAction.CallbackContext context)
    {
        if (!canTakeInput)
        {
            OnMovementInput?.Invoke(Vector2.zero);
            return;
        }

        Vector2 horizontalInput = context.ReadValue<Vector2>();
        OnMovementInput?.Invoke(horizontalInput);
    }

    private void HandleJump(InputAction.CallbackContext context)
    {
        if (!canTakeInput) return;

        OnJumpInput?.Invoke();
    }

    private void SetCanTakeInputToFalse()
    {
        canTakeInput = false;
    }
}
