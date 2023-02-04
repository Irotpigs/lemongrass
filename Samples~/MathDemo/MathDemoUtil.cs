//---------------------------------------------------------------------------//
// Name        - MathDemoUtil.cs
// Author      - Daniel Onstott
// Project     - Lemongrass
// Description - Provides some usage of each of the math functions so they can
//  be seen in action.
//---------------------------------------------------------------------------//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MathDemoUtil : MonoBehaviour
{
  ////////////////////////////////////Variables//////////////////////////////////
  //-public--------------------------------------------------------------------//
  //-private-------------------------------------------------------------------//
  private const int weightedRandomCount = 10;
  private const int probabilityCount = 5;

  [SerializeField, Range(0.1f, 1.5f)] private float updateInterval = 1.0f;

  [Header("Weighted Random Values")]
  [SerializeField] private GameObject weightedRandomParticlePrefab;

  [SerializeField] private Transform[] weightedRandomSpawnLocations = new Transform[weightedRandomCount];
  [SerializeField] private float[] weights = new float[weightedRandomCount] { 1, 2, 3, 4, 5, 5, 4, 3, 2, 1 };
  [SerializeField] private TextMeshPro[] displayObjects0 = new TextMeshPro[weightedRandomCount];

  [SerializeField] private Lemongrass.ValueProbabilityPair<Transform>[] weightedRandomPairs = new Lemongrass.ValueProbabilityPair<Transform>[weightedRandomCount];
  [SerializeField] private TextMeshPro[] displayObjects1 = new TextMeshPro[weightedRandomCount];

  [Header("Probabilities Values")]
  [SerializeField] private MeshRenderer[] probabilityRepresentations = new MeshRenderer[probabilityCount];
  [SerializeField, Range(0, 1)] private float[] successChances = new float[probabilityCount] {0.1f, 0.3f, 0.5f, 0.7f, 0.9f};
  [SerializeField] private TextMeshPro[] displayObjects2 = new TextMeshPro[probabilityCount];

  [Header("Random From Collection")]
  [SerializeField] List<GameObject> gameObjectCollection = new List<GameObject>();


  ////////////////////////////////////Functions//////////////////////////////////
  //-private-------------------------------------------------------------------//

  private void Start()
  {
    InitializeProbabilities();
    StartCoroutine(ProbabilityCoroutine());

    InitializeWeightedRandom();
    StartCoroutine(WeightedRandomCoroutine());

    InitializeRandomFromCollection();
    StartCoroutine(RandomFromCollectionCoroutine());
  }

  // sets all of the text mesh pro displays to be accurate to the values set in inspector
  private void InitializeWeightedRandom()
  {
    for (int i = 0; i < weightedRandomCount; ++i)
    {
      displayObjects0[i].text = weights[i].ToString("0.00");
      displayObjects1[i].text = weightedRandomPairs[i].P.ToString("0.00");
    }
  }

  // sets all of the text mesh pro displays to be accurate to the values set in inspector
  // AND sets the cubes to have no color
  private void InitializeProbabilities()
  {
    for (int i = 0; i < probabilityCount; ++i)
    {
      displayObjects2[i].text = successChances[i].ToString("0.00");
      probabilityRepresentations[i].material.color = Color.white;
    }
  }

  // Sets all the gameobjects in the list to a scale of 1
  private void InitializeRandomFromCollection()
  {
    foreach (GameObject obj in gameObjectCollection) obj.transform.localScale = Vector3.one;
  }

  private IEnumerator WeightedRandomCoroutine()
  {
    InitializeWeightedRandom();

    // create a cube using a list of weights and a list of values
    Transform spawnPosition = Lemongrass.LemonRandom.WeightedRandom(weights, weightedRandomSpawnLocations);
    Instantiate(weightedRandomParticlePrefab, spawnPosition.position, Random.rotation);

    // create a cube using our list of probability pairs
    spawnPosition = Lemongrass.LemonRandom.WeightedRandom(weightedRandomPairs);
    Instantiate(weightedRandomParticlePrefab, spawnPosition.position, Random.rotation);

    yield return new WaitForSeconds(updateInterval);
    StartCoroutine(WeightedRandomCoroutine());
  }
  private IEnumerator RandomFromCollectionCoroutine()
  {
    InitializeRandomFromCollection();

    // select a cube at random from the list
    // note that we check there are objects in the list first.
    if (gameObjectCollection.Count > 0)
    {
      GameObject selected = Lemongrass.LemonRandom.FromCollection(gameObjectCollection);
      selected.transform.localScale = Vector3.one * 0.5f;
    }

    yield return new WaitForSeconds(updateInterval);
    StartCoroutine(RandomFromCollectionCoroutine());
  }

  private IEnumerator ProbabilityCoroutine()
  {
    yield return new WaitForSeconds(updateInterval * 0.5f);

    // Test our various probabilities
    for (int i = 0; i < probabilityCount; ++i)
    {
      if (Lemongrass.LemonRandom.Probability(successChances[i]))
      {
        // we successfully did this event!
        probabilityRepresentations[i].material.color = Color.green;
      }
      else
      {
        // we failed at this event :(
        probabilityRepresentations[i].material.color = Color.red;
      }
    }
    
    yield return new WaitForSeconds(updateInterval * 0.5f);
    InitializeProbabilities();

    StartCoroutine(ProbabilityCoroutine());
  }
}
