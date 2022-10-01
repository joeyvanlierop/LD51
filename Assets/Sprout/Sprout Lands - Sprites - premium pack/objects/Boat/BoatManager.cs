using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatManager : MonoBehaviour
{
    List<GameObject> Boats = new List<GameObject>();
    public List<GameObject> PossibleItems = new List<GameObject>();
    public GameObject BoatPrefab;
    float betweenBoatTime = 10f;
    float startTime = 0;
    public Vector3 SpawnLocation = new Vector3(13.29f, -2.36f, 0.02834536f);
    // Start is called before the first frame update
    void Start()
    {
        foreach (var go in GameObject.FindGameObjectsWithTag("Wanted Item")) {
            PossibleItems.Add(go);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void FixedUpdate() {
        if (startTime > betweenBoatTime) {
            startTime = 0;
            GameObject newBoat = Instantiate(BoatPrefab, SpawnLocation, Quaternion.identity);
            foreach (GameObject PossibleItem in PossibleItems) {
                newBoat.GetComponent<boatSoliciting>().wantedItems.Add(Instantiate(PossibleItem), Random.Range(1, 9));
            }
            Boats.Add(newBoat);
        } else {
            startTime += Time.deltaTime;
        }
    }
}
