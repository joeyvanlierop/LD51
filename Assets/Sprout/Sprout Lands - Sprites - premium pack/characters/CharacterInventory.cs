using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterInventory : MonoBehaviour
{
  public Item heldItem;
  public GameObject heldItemBubble;
  public InputAction inventoryAction;
  public float pickupRadius = 0.5f;
  public float throwSpeed = 1f;

  private CharacterMovement characterMovement;

  private int oldSortingOrder;

  void Awake()
  {
    inventoryAction.performed += _ => PerformAction();
    characterMovement = gameObject.GetComponent<CharacterMovement>();
  }

  void PerformAction()
  {
    if (!heldItem)
      PickupItem();
    else
      ThrowItem();
  }


  private void PickupItem()
  {
    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, pickupRadius);
    foreach (Collider2D collider in colliders)
    {
      if (collider.tag == "Item")
      {
        heldItem = collider.GetComponent<Item>();
        AttachItem(heldItem);
        break;
      }
    }
  }

  public Item AttachItem(Item heldItem) {
    heldItem.transform.SetParent(heldItemBubble.transform);
    oldSortingOrder = heldItem.GetComponent<SpriteRenderer>().sortingOrder;
    heldItem.GetComponent<SpriteRenderer>().sortingOrder = heldItemBubble.GetComponent<SpriteRenderer>().sortingOrder + 1;
    heldItem.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
    heldItem.GetComponent<BoxCollider2D>().enabled = false;
    heldItem.transform.localPosition = new Vector2(0, 0);
    this.heldItem = heldItem;
    return heldItem;
  }

  private void ThrowItem()
  {
    heldItem.transform.SetParent(transform);
    heldItem.GetComponent<SpriteRenderer>().sortingOrder = oldSortingOrder;
    heldItem.GetComponent<BoxCollider2D>().enabled = true;
    heldItem.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
    Vector2 velocity = Vector2.zero;
    switch (characterMovement.direction)
    {
      case Direction.UP:
        velocity = new Vector2(0, throwSpeed);
        heldItem.transform.localPosition = new Vector2(0, 1);
        break;
      case Direction.DOWN:
        velocity = new Vector2(0, -throwSpeed);
        heldItem.transform.localPosition = new Vector2(0, -1);
        break;
      case Direction.LEFT:
        velocity = new Vector2(-throwSpeed, 0);
        heldItem.transform.localPosition = new Vector2(-1, 0);
        break;
      case Direction.RIGHT:
        velocity = new Vector2(throwSpeed, 0);
        heldItem.transform.localPosition = new Vector2(1, 0);
        break;
    }
    heldItem.transform.SetParent(null);
    heldItem.Throw(velocity);
    heldItem = null;
  }

  void Update()
  {
    // heldItemBubble.GetComponent<SpriteRenderer>().enabled = heldItem == true;
    heldItemBubble.GetComponent<SpriteRenderer>().enabled = false;
    if (heldItem)
    {
      heldItem.transform.localPosition = Vector2.up * Mathf.Cos(Time.time * 4) / 3;
    }
  }

  private void OnEnable()
  {
    inventoryAction.Enable();
  }

  private void OnDisable()
  {
    inventoryAction.Disable();
  }
}
