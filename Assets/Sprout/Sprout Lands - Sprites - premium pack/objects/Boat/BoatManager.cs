using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BoatManager : MonoBehaviour
{
  List<GameObject> Boats = new List<GameObject>();
  public List<GameObject> PossibleSolicitingItems = new List<GameObject>();
  public GameObject BoatPrefab;
  public GameObject DeliveryBoatPrefab;

  public GameObject GameStateManager;

  public List<GameObject> ChoicesPrefab = new List<GameObject>();

  public List<Item> ChoicesItemsPrefab = new List<Item>();

  public List<int> WeightedItemsPrefab = new List<int>();
  public List<int> CurrentWeightedItemsPrefab = new List<int>();

  Dictionary<GameObject, Item> CurrentPossibleChoices = new Dictionary<GameObject, Item>();

  List<GameObject> CurrentPossibleSolicitingItems = new List<GameObject>();

  float betweenBoatTime = 10f;

  public int MaxBoats = 1;

  public float MinBoatX = -18f;

  bool boatTimerStarted = false;
  public GameObject BetweenBoatTimer;
  GameObject TimerRef;
  float startTime = 0f;

  int rounds = 0;

  int maxItems = 0;

  public Event myTrigger;


  public Vector3 SpawnLocation = new Vector3(13.29f, -2.36f, 0.02834536f);
  // Start is called before the first frame update
  void Start()
  {
    for (int i = 0; i < ChoicesPrefab.Count; i++)
    {
      CurrentPossibleChoices.Add(ChoicesPrefab[i], ChoicesItemsPrefab[i]);
    }


  }

  void Awake()
  {
    myTrigger = new Event();
    Event.Instance.EatCallbacks.Add(ResetTimer);
  }

  // Update is called once per frame
  void Update()
  {

  }

  void SpawnBoat()
  {
    if (GameStateManager.GetComponent<GameStateManager>().gameState == GameState.MAIN_MENU)
    {
      SpawnNormalBoat();

      return;
    }

    if (rounds % 3 == 0)
    {
      if (CurrentPossibleChoices.Count == 0) {
        SpawnNormalBoat();
        rounds += 5;
        return;
      }
      SpawnDeliveryBoat();
    }
    else
    {
      SpawnNormalBoat();
    }
    rounds++;
  }

  void SpawnDeliveryBoat()
  {
    GameObject newBoat = Instantiate(DeliveryBoatPrefab, SpawnLocation, Quaternion.identity);
    if (CurrentPossibleSolicitingItems.Count == 0)
    {
      Debug.Log("TEST");
      newBoat.GetComponent<boatDelivery>().choicesPrefab.Add(ChoicesPrefab[0]);
      newBoat.GetComponent<boatDelivery>().Items.Add(ChoicesItemsPrefab[0]);
      newBoat.GetComponent<boatDelivery>().choicesPrefab.Add(ChoicesPrefab[1]);
      newBoat.GetComponent<boatDelivery>().Items.Add(ChoicesItemsPrefab[1]);
      newBoat.GetComponent<boatDelivery>().ChoiceCallback = GetChoice;
      Boats.Add(newBoat);
      return;
    }

    if (CurrentPossibleChoices.Count == 1) {
      newBoat.GetComponent<boatDelivery>().choicesPrefab.Add(ChoicesPrefab[0]);
      newBoat.GetComponent<boatDelivery>().Items.Add(ChoicesItemsPrefab[0]);
      newBoat.GetComponent<boatDelivery>().ChoiceCallback = GetChoice;
      Boats.Add(newBoat);
      return;
    }
    var addedChoice = 0;
    while (addedChoice != 2)
    {
      foreach (var pair in CurrentPossibleChoices)
      {
        bool toAdd = Random.Range(0, 2) == 1;
        if (toAdd)
        {
          newBoat.GetComponent<boatDelivery>().choicesPrefab.Add(pair.Key);
          newBoat.GetComponent<boatDelivery>().Items.Add(pair.Value);
          addedChoice++;
          if (addedChoice == 2)
          {
            break;
          }
        }
      }
    }
    newBoat.GetComponent<boatDelivery>().ChoiceCallback = GetChoice;
    Boats.Add(newBoat);
  }

  void SpawnNormalBoat()
  {
    if (rounds < 3)
    {
      maxItems = rounds;
    }
    else if (rounds % 10 == 0)
    {
      maxItems++;
    }
    GameObject newBoat = Instantiate(BoatPrefab, SpawnLocation, Quaternion.identity);
    var wantedItems = newBoat.GetComponent<boatSoliciting>().wantedItems;
    for (int i = 0; i < maxItems; i++)
    {
      var itemIndex = GetRandomWeightedIndex(CurrentWeightedItemsPrefab);
      if (wantedItems.ContainsKey(CurrentPossibleSolicitingItems[itemIndex]))
      {
        wantedItems[CurrentPossibleSolicitingItems[itemIndex]] += 1;
      }
      else
      {
        wantedItems.Add(Instantiate(CurrentPossibleSolicitingItems[itemIndex]), 1);
      }
    }
    // foreach (GameObject PossibleItem in CurrentPossibleSolicitingItems) {
    //     var thisChoice = Random.Range(1, maxItems - choseItems);
    //     choseItems += thisChoice;
    //     newBoat.GetComponent<boatSoliciting>().wantedItems.Add(Instantiate(PossibleItem), choseItems);
    // }
    newBoat.GetComponent<boatSoliciting>().EndSolicitingCallback = EndSolicitingCallback;
    Boats.Add(newBoat);
  }

  void EndSolicitingCallback(boatSoliciting.SolicitingState state)
  {
    if (state != boatSoliciting.SolicitingState.SUCCESS)
    {
      if (GameStateManager.GetComponent<GameStateManager>().gameState == GameState.PLAYING)
      {
        GameStateManager.GetComponent<GameStateManager>().EndGame();
      }

    }
  }

  void RemoveBoat(GameObject boat)
  {
    if (boat.GetComponent<boatSoliciting>() == null)
    {
      boat.GetComponent<boatDelivery>().DeleteAll();
    }
    else
    {
      boat.GetComponent<boatSoliciting>().DeleteAll();
    }

    Boats.RemoveAt(0);
    Destroy(boat);
  }

  void GetChoice(GameObject choice)
  {
    var choiceIndex = 0;
    bool found = false;
    for (int i = 0; i < ChoicesPrefab.Count; i++)
    {
      if (choice.name.Split()[0] == ChoicesPrefab[i].name.Split()[0])
      {
        choiceIndex = i;
        found = true;
      }

    }

    if (!found)
    {
      Debug.Log("Panic!");
      Debug.Log(choiceIndex);
    }
    else
    {
      CurrentWeightedItemsPrefab.Add(WeightedItemsPrefab[choiceIndex]);
      CurrentPossibleSolicitingItems.Add(PossibleSolicitingItems[choiceIndex]);
      //   CurrentPossibleChoices.Remove(choice);
      foreach (var pair in CurrentPossibleChoices)
      {
        if (pair.Key.name.Split()[0] == choice.name.Split()[0])
        {
          CurrentPossibleChoices.Remove(pair.Key);
          break;
        }
      }
    }
  }

  void FixedUpdate()
  {
    if (Boats.Count >= 1)
    {
      if (Boats[0].transform.position.x < MinBoatX)
      {
        RemoveBoat(Boats[0]);
      }
      return;
    }

    if (startTime > betweenBoatTime)
    {
      startTime = 0;
      SpawnBoat();
      if (TimerRef != null)
      {
        Destroy(TimerRef);
        boatTimerStarted = false;
      }
    }
    else
    {
      if (!boatTimerStarted)
      {
        TimerRef = Instantiate(BetweenBoatTimer, new Vector2(8, -2.5f), Quaternion.identity);
        boatTimerStarted = true;
      }
      startTime += Time.deltaTime;
    }
  }

  void ResetTimer()
  {
    Debug.Log("Works");
    if (TimerRef != null)
    {
      TimerRef.GetComponent<Animator>().Play("time", -1, 0f);
    }
  }


  public int GetRandomWeightedIndex(List<int> weights)
  {
    if (weights == null || weights.Count == 0) return -1;

    float w;
    float t = 0;
    int i;
    for (i = 0; i < weights.Count; i++)
    {
      w = weights[i];

      if (float.IsPositiveInfinity(w))
      {
        return i;
      }
      else if (w >= 0f && !float.IsNaN(w))
      {
        t += weights[i];
      }
    }

    float r = Random.value;
    float s = 0f;

    for (i = 0; i < weights.Count; i++)
    {
      w = weights[i];
      if (float.IsNaN(w) || w <= 0f) continue;

      s += w / t;
      if (s >= r) return i;
    }

    return -1;
  }
}
