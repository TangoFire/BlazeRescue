using UnityEngine;
using System.Collections;

public class VictimMovement : MonoBehaviour
{
    [Header("Pathfinding Setup")]
    public Transform[,] windowsGrid;  // 3x3 grid of windows
    public Transform[] stairPositions; // Stair locations for each floor
    public Transform exitDoor;  // Exit door at the ground floor

    [Header("Victim Status")]
    private int currentFloor = 2;  // Start at the top floor
    private int currentWindowIndex = 0;  // Horizontal position on the floor
    private bool movingToStairs = false;  // Flag for stair movement

    [Header("Movement & Behavior")]
    public float baseMoveSpeed = 2f;  // Base movement speed
    private float moveSpeed;  // Actual speed (affected by victim type)
    private bool isPanicking = false; // Panic mode activation
    private bool doorOpened = false; // Whether the player has opened the door

    [Header("Panic Mode")]
    public float panicDelayMin = 1f; // Minimum delay before moving
    public float panicDelayMax = 3f; // Maximum delay before moving
    public AudioClip screamSound; // Victim scream when panicking
    private AudioSource audioSource;

    public enum VictimType { Normal, Child, Elderly, FastResponder }
    public VictimType victimType;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Initialize windowsGrid array (assuming a 3x3 grid for this example)
        windowsGrid = new Transform[3, 3];

        // Set movement speed based on victim type
        switch (victimType)
        {
            case VictimType.Normal:
                moveSpeed = baseMoveSpeed;
                break;
            case VictimType.Child:
                moveSpeed = baseMoveSpeed * 1.2f; // Faster movement
                break;
            case VictimType.Elderly:
                moveSpeed = baseMoveSpeed * 0.8f; // Slower movement
                break;
            case VictimType.FastResponder:
                moveSpeed = baseMoveSpeed * 1.5f; // Quick reaction
                break;
        }

        StartCoroutine(VictimBehavior());
    }

    IEnumerator VictimBehavior()
    {
        while (currentFloor >= 0) // Continue moving down floors until the ground floor is reached
        {
            if (!movingToStairs)
            {
                Transform targetWindow = windowsGrid[currentFloor, currentWindowIndex];

                // Panic delay if the victim is scared
                if (isPanicking)
                {
                    float panicTime = Random.Range(panicDelayMin, panicDelayMax);
                    if (screamSound && audioSource) // Play scream sound
                        audioSource.PlayOneShot(screamSound);

                    yield return new WaitForSeconds(panicTime);
                }

                // Move to the next window
                yield return MoveTo(targetWindow.position);

                // Wait until fire is extinguished at this window
                while (!targetWindow.GetComponent<Window>().isOnFire    )
                {
                    yield return null;
                }

                // Move towards the stairs when reaching the last window on the floor
                if (currentWindowIndex == windowsGrid.GetLength(1) - 1)
                {
                    movingToStairs = true;
                    yield return MoveToStairs();
                }
                else
                {
                    currentWindowIndex++; // Move to the next window on the same floor
                }
            }
            else
            {
                yield return MoveToStairs();
            }
        }

        // Once on the ground floor, move to the exit door
        yield return MoveTo(exitDoor.position);

        // Wait until the player opens the door
        while (!doorOpened)
        {
            yield return null;
        }

        Debug.Log("Victim escaped!");
    }

    IEnumerator MoveToStairs()
    {
        Transform targetStair = stairPositions[currentFloor];

        // Move to the staircase
        yield return MoveTo(targetStair.position);

        // Wait for fire below to be extinguished
        Transform windowBelow = windowsGrid[currentFloor - 1, 0];
       while (windowBelow.GetComponent<Window>().isOnFire)
        {
            yield return null;
        }

        // Move down to the next floor
        currentFloor--;
        currentWindowIndex = 0;
        movingToStairs = false;
    }

    IEnumerator MoveTo(Vector3 target)
    {
        while (Vector3.Distance(transform.position, target) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    // Called when the player breaks open the exit door
    public void OpenDoor()
    {
        doorOpened = true;
    }
}
