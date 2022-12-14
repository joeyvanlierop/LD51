using UnityEngine;
using UnityEngine.InputSystem;

public enum Direction
{
  UP,
  DOWN,
  LEFT,
  RIGHT
}

public class CharacterMovement : MonoBehaviour
{

  public Animator animator;
  public Vector2 velocity;
  public float moveX;
  public float moveY;
  public float moveSpeedX = 5f;
  public float moveSpeedY = 5f;
  public InputAction movementAction;
  public Direction direction = Direction.DOWN;

  private bool canMove = true;

  Rigidbody2D rb;
  public Vector2 moveDirection = Vector2.zero;
  public bool autoWalk = false;

  float oldMoveSpeedX = 0;
  float oldMoveSpeedY = 0;

  void Start()
  {
    rb = gameObject.GetComponent<Rigidbody2D>();
  }

  //Update is called once per frame
  void Update()
  {
    if (!canMove)
    {
      moveDirection = Vector2.zero;
      return;
    }

    if (!autoWalk)
      moveDirection = movementAction.ReadValue<Vector2>();

    var moving = true;

    if (moveDirection.x < 0)
    {
      direction = Direction.LEFT;
      animator.SetFloat("X", -1);
      animator.SetFloat("Y", 0);
    }
    else if (moveDirection.x > 0)
    {
      direction = Direction.RIGHT;
      animator.SetFloat("X", 1);
      animator.SetFloat("Y", 0);
    }
    else if (moveDirection.y < 0)
    {
      direction = Direction.DOWN;
      animator.SetFloat("Y", -1);
      animator.SetFloat("X", 0);
    }
    else if (moveDirection.y > 0)
    {

      direction = Direction.UP;
      animator.SetFloat("Y", 1);
      animator.SetFloat("X", 0);
    }
    else
    {
      moving = false;
    }

    animator.SetBool("moving", moving);

    UpdateSortingLayer();
  }

  private void FixedUpdate()
  {
    rb.velocity = new Vector2(moveDirection.x * moveSpeedX, moveDirection.y * moveSpeedY);
  }

  void SetCanMove(string canMove)
  {
    this.canMove = canMove == "true";
  }

  void UpdateSortingLayer()
  {

  }

  private void OnEnable()
  {
    movementAction.Enable();
  }

  private void OnDisable()
  {
    movementAction.Disable();
  }

  public void ResetSpeed() {
    moveSpeedX = oldMoveSpeedX;
    moveSpeedY = oldMoveSpeedY;
  }
  public void SetSpeed(float x, float y) {
    oldMoveSpeedX = moveSpeedX;
    oldMoveSpeedY = moveSpeedY;
    moveSpeedX = x;
    moveSpeedY = y;
  }
}
