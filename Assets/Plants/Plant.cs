using UnityEngine;
using UnityEngine.Tilemaps;

public class Plant : MonoBehaviour
{
  public int currentStage = 0;
  public Tile[] stages;
  public GameObject harvestItem;
  public string harvestSeedName;
  private GameObject harvestSeed;
  public float maxSeedsDropped = 3;

  void Awake()
  {
    harvestSeed = (GameObject)Resources.Load(harvestSeedName);
    Debug.Log(harvestSeed);
  }

  public void Harvest(Vector3 pos)
  {
    Burst(harvestItem, pos);
    for (int i = 0; i < Random.Range(1, maxSeedsDropped + 1); i++)
    {
      Debug.Log(i);
      Burst(harvestSeed, pos);
    }
  }

  void Burst(GameObject obj, Vector3 pos)
  {
    var newObj = Instantiate(obj, pos, Quaternion.identity);
    var rb = newObj.GetComponent<Rigidbody2D>();
    var force = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
    force *= 10;
    rb.AddForce(force, ForceMode2D.Impulse);
  }
}
