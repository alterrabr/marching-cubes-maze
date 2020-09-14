using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    public float Speed = 30f;
    public float JumpForce = 2f;

    private bool grounded;
    private CharacterController characterController;
    private Vector3 playerVelocity;
    private float gravityValue = 0;
    private bool mazeSolved = false;

    public delegate void MazeSolves();
    public static event MazeSolves OnMazeSolved;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();

        Maze.OnMazeGenerated += AddGravity;

        UIController.OnMazeRestarted += RestartMaze;
    }

    void Update()
    {
        grounded = characterController.isGrounded;

        if (grounded && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        if (Input.GetButtonDown("Jump") && grounded)
        {
            playerVelocity.y += Mathf.Sqrt(JumpForce * -3.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        characterController.Move(playerVelocity * Time.deltaTime);

        float horizontal = Input.GetAxis("Horizontal") * Speed * Time.deltaTime;
        float veritcal = Input.GetAxis("Vertical") * Speed * Time.deltaTime;

        Vector3 gravity = new Vector3(0, gravityValue, 0);
        Quaternion rotation = Camera.main.transform.rotation;
        rotation.x = 0;
        rotation.z = 0;
        transform.rotation = rotation;

        Vector3 move = (Camera.main.transform.right * horizontal) + (Camera.main.transform.forward * veritcal) + gravity * Time.deltaTime;
        characterController.Move(move);

        if ((this.transform.position.y < -3f && !mazeSolved))
        {
            RestartMaze();
        }
    }

    private void AddGravity()
    {
        transform.position = new Vector3(0, 30, 0);
        gravityValue = -9.81f;

        mazeSolved = false;
    }

    private void RestartMaze()
    {
        OnMazeSolved();

        transform.position = new Vector3(0, 30, 0);

        mazeSolved = true;
    }
}
