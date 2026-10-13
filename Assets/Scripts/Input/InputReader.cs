using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static PlayerInputActions;

public interface IInputReader
{
    void EnablePlayerActions();
}

/// <summary>
/// The SO that reads and disperses input events to all scripts
/// Participating scripts should get a reference to the inputReader SO
/// and subscibe to the necessary delegates. Be sure to call `EnablePlayerActions()`
/// in Enable and `DisablePlayerActions()` in Disable.
/// </summary>
[CreateAssetMenu(fileName = "InputReader", menuName = "Input/Input Reader")]
public class InputReader : ScriptableObject, IInputReader, IPlayerActions
{
    public event UnityAction InteractPressed = delegate { };
    public event UnityAction InteractReleased = delegate { };

    public event UnityAction JumpPressed = delegate { };

    public event UnityAction<Vector2> MoveChanged = delegate { };
    public event UnityAction<Vector2> LookChanged = delegate { };

    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }

    public PlayerInputActions inputActions;

    public void EnablePlayerActions()
    {
        if (inputActions == null)
        {
            inputActions = new PlayerInputActions();
            inputActions.Player.SetCallbacks(this);
        }
        inputActions.Enable();
    }

    public void DisablePlayerActions()
    {
        if (inputActions != null)
        {
            inputActions.Player.Disable();
        }
    }


    // This one is for things like "press _ to jump" if we need
    public string GetInteractDisplayString()
    {
        if (inputActions == null)
        {
            return "";
        }

        foreach (InputBinding binding in inputActions.Player.Interact.bindings)
        {
            if (binding.groups != null && binding.groups.Contains("Keyboard"))
            {
                return InputControlPath.ToHumanReadableString(
                    binding.effectivePath,
                    InputControlPath.HumanReadableStringOptions.OmitDevice);
            }
        }

        return "";
    }

    #region Player Actions

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
        MoveChanged.Invoke(MoveInput);
    }
    public void OnAttack(InputAction.CallbackContext context) { }
    public void OnCrouch(InputAction.CallbackContext context) { }
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            InteractPressed.Invoke();
        }
        else if (context.canceled)
        {
            InteractReleased.Invoke();
        }
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            JumpPressed.Invoke();
        }
    }
    public void OnLook(InputAction.CallbackContext context)
    {
        LookInput = context.ReadValue<Vector2>();
        LookChanged.Invoke(LookInput);
    }
    public void OnNext(InputAction.CallbackContext context) { }
    public void OnPrevious(InputAction.CallbackContext context) { }
    public void OnSprint(InputAction.CallbackContext context) { }

    #endregion
}
