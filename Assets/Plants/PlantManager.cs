using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlantManager : MonoBehaviour
{


  private Tilemap plantTilemap;
  private Dictionary<Vector3Int, IPlant> plants = new Dictionary<Vector3Int, IPlant>();

  void Awake()
  {
    plantTilemap = gameObject.GetComponent<Tilemap>();
  }

  public bool Plant(IPlant plant, Vector3Int pos)
  {
    if (HasPlant(pos))
      return false;
    plants.Add(pos, plant);
    plant.transform.SetParent(transform);
    StartCoroutine(StartGrowing(plant, pos));
    return true;
  }

  IEnumerator StartGrowing(IPlant plant, Vector3Int pos)
  {
    plantTilemap.SetTile(pos, plant.stages[0]);
    while (plant.currentStage < plant.stages.Length - 1)
    {
      var growTime = Random.Range(plant.minGrowTime, plant.maxGrowTime);
      yield return new WaitForSeconds(growTime);
      plant.currentStage += 1;
      plantTilemap.SetTile(pos, plant.stages[plant.currentStage]);
    }
  }

  IEnumerator StartFruiting(Bush bush, Vector3Int pos)
  {
    var fruitTime = Random.Range(bush.minFruitTime, bush.maxGrowTime);
    yield return new WaitForSeconds(fruitTime);
    bush.currentStage += 1;
    plantTilemap.SetTile(pos, bush.stages[bush.currentStage]);
  }

  public void Harvest(Vector3Int pos)
  {
    var plant = plants[pos];
    if (!plant)
      return;
    if (plant.currentStage != plant.stages.Length - 1)
      return;

    plant.Harvest(pos);

    if (plant is Bush)
    {
      plant.currentStage -= 1;
      plantTilemap.SetTile(pos, plant.stages[plant.currentStage]);
      StartCoroutine(StartFruiting(plant as Bush, pos));
    }
    else
    {
      plantTilemap.SetTile(pos, null);
      plants.Remove(pos);
    }
  }

  internal bool HasPlant(Vector3Int pos)
  {
    return plants.ContainsKey(pos);
  }
}
