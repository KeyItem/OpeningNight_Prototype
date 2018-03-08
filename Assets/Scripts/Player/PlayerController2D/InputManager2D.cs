using System.Collections;
using UnityEngine;
using Rewired;

[RequireComponent(typeof(PlayerController2D))]
public class InputManager2D : InputManager
{
    private Player playerInput;

    private PlayerController2D playerController2D;

    [Header("Input Manager Attributes")]
    public float xInputAxis;
    public float yInputAxis;

    [Space(10)]
    public Vector2 inputVector;

    [Header("Registered Input")]
    public RegisteredInput registeredInput = new RegisteredInput();

    private void Start()
    {
        SetupInputManager();
    }

    private void Update()
    {
        GetInput();
    }

    private void SetupInputManager()
    {
        playerInput = ReInput.players.GetPlayer(0);

        playerController2D = GetComponent<PlayerController2D>();
    }

    private void GetInput()
    {
        xInputAxis = playerInput.GetAxis("HorizontalAxis");
        yInputAxis = playerInput.GetAxis("VerticalAxis");

        inputVector.Set(xInputAxis, yInputAxis);

        if (playerInput.GetButtonDown("Jump"))
        {
            registeredInput.hasPressedJump = true;
        }

        if (playerInput.GetButtonUp("Jump"))
        {
            registeredInput.hasReleasedJump = true;
        }

        playerController2D.ReceiveInput(inputVector, registeredInput);

        registeredInput.ResetRegisteredInput();
    }
}

[System.Serializable]
public class RegisteredInput
{
    [Header("Registered Input")]
    public bool hasPressedJump = false;
    public bool hasReleasedJump = false;

    public void ResetRegisteredInput()
    {
        hasPressedJump = false;
        hasReleasedJump = false;
    }
}
