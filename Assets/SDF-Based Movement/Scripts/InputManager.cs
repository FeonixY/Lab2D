using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputManager : MonoBehaviour
{
    public static Vector2 Movement;

    private PlayerInput playerInput;
    private InputAction moveAction;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        moveAction = playerInput.actions["Move"];
    }

    private void Update()
    {
        Movement = moveAction.ReadValue<Vector2>();
    }
}
