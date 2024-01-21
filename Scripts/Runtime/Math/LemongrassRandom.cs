//---------------------------------------------------------------------------//
// Name        - LemongrassRandom.cs
// Author      - Daniel Onstott
// Project     - Lemongrass
// Description - A collection of utility functions that can be used for
//  randomness! Serves mostly as a wrapper around Unity's randomness for 
//  common functionality.
//---------------------------------------------------------------------------//
using System.Collections.Generic;
using UnityEngine;

namespace Lemongrass
{
  /**
   * @brief Combines a value T with the probability that the value will be chosen
   *        from a collection with other ValueProbabilityPairs.
   */
  [System.Serializable]
  public struct ValueProbabilityPair<T>
  {
    public T Value;
    public float P; // (robability)
  }
  
  /**
   * @brief A collection of utility functions that are used for randomness.
   */
  public static class LemonRandom
  {
    /**
     * @brief Get the outcome of an event with probability p.
     * @param p the probability of a successful outcome. This value should be between 0-1.
     * @return True when the event succeeds. False when it does not.
     */
    public static bool Probability(float p)
    {
      Debug.Assert(p >= 0 && p <= 1, "Probability out of range 0-1");
      return Random.Range(0.0f, 1.0f) <= p;
    }

    /**
     * @brief Given an array of ValueProbabilityPair, return a value at random with the 
     *        probability of the corresponding weight. The probability in each pair
     *        can be any -positive- value (even exceeding 1).
     * @param valueProbabilityPairs An array of ValueProbabilityPairs to choose from.
     * @return The value that was chosen randomly.
     */
    static public T WeightedRandom<T>(ValueProbabilityPair<T>[] valueProbabilityPairs)
    {
      Debug.Assert(valueProbabilityPairs.Length >= 1);

      float total = 0.0f;
      foreach (ValueProbabilityPair<T> pair in valueProbabilityPairs)
      {
        Debug.Assert(pair.P >= 0, "WeightedRandom encountered a negative value.");
        total += pair.P;
      }

      float randomRoll = Random.Range(0.0f, total);

      // Find the interval randomRoll lies in
      float totalChance = valueProbabilityPairs[0].P;
      for (int i = 0; i < valueProbabilityPairs.Length - 1; ++i)
      {
        if (randomRoll <= totalChance) return valueProbabilityPairs[i].Value;
        totalChance += valueProbabilityPairs[i + 1].P;
      }

      return valueProbabilityPairs[valueProbabilityPairs.Length - 1].Value;
    }

    /**
     * @brief Given an array of weights and values, return a value at random with the 
     *        probability of the corresponding weight. The probability in each pair
     *        can be any -positive- value (even exceeding 1).
     * @param weights An array of weights representing the chance for each event. 
     *                Must be the same length as values.
     * @param values An array of values that can be chosen from.
     *               Must be the same length as weights.
     * @return The value that was chosen randomly.
     */
    static public T WeightedRandom<T>(float[] weights, T[] values)
    {
      Debug.Assert(weights.Length == values.Length, "WeightedRandom encountered unequal counts of weights and values");

      float total = 0.0f;
      foreach (float f in weights)
      {
        Debug.Assert(f >= 0, "WeightedRandom encountered a negative value.");
        total += f;
      }

      float randomRoll = Random.Range(0.0f, total);

      // Find the interval randomRoll lies in
      float totalChance = weights[0];
      for (int i = 0; i < weights.Length - 1; ++i)
      {
        if (randomRoll <= totalChance) return values[i];
        totalChance += weights[i + 1];
      }

      return values[values.Length - 1];
    }

    /**
     * @brief Returns a random item from the given list between index 0 and List.Count.
     * @param list The list to choose an item from.
     * @return An item from the given list. Returns default(T) if the list is empty.
     *         For that reason it is recommended to use this on a nullable type collection
     */
    static public T FromNullableCollection<T>(List<T> list) 
    {
      if (list.Count <= 0) return default(T);
      return list[Random.Range(0, list.Count)];
    }

    /**
     * @brief Returns a random item from the given list between index 0 and List.Count.
     * @param list The list to choose an item from.
     * @return An item from the given list. Returns default(T) if the list is empty.
     *         For that reason it is recommended to use this on a nullable type collection
     */
    static public T FromNullableCollection<T>(T[] array)
    {
      if (array.Length <= 0) return default(T);
      return array[Random.Range(0, array.Length)];
    }

    /**
     * @brief Returns a random item from the given list between index 0 and List.Count.
     *        Will assert when the collection is empty. THE COLLECTION MUST NOT BE EMPTY
     * @param list The list to choose an item from.
     * @return An item from the given list.
     */
    static public T FromCollection<T>(List<T> list)
    {
      Debug.Assert(list.Count > 0, "Cannot get a random from an empty collection");
      return list[Random.Range(0, list.Count)];
    }

    /**
     * @brief Returns a random item from the given list between index 0 and List.Count.
     *        Will assert when the collection is empty. THE COLLECTION MUST NOT BE EMPTY
     * @param list The list to choose an item from.
     * @return An item from the given list.
     */
    static public T FromCollection<T>(T[] array)
    {
      Debug.Assert(array.Length > 0, "Cannot get a random from an empty collection");
      return array[Random.Range(0, array.Length)];
    }
    
    /**
     * @brief Shuffles the given collection ased on fisher-yates shuffle
     * @param list The list to shuffle
     */
    static public void Shuffle<T>(this IList<T> list)
    {
      int n = list.Count;
      while (n-- > 1)
      {
        int k = Random.Range(0, list.Count);
        T value = list[k];
        list[k] = list[n];
        list[n] = value;
      }
    }
  }
}
