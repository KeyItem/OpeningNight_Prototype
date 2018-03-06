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

    private bool hasJumped = false;

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
            if (playerController2D.playerCollisionData.isCollisionBelow)
            {
                hasJumped = true;
            }
        }

        playerController2D.ReceiveInput(inputVector, hasJumped);

        hasJumped = false;
    }
}
