//---------------------------------------------------------------------------//
// Name        - ScriptableObjectDataSource.cs
// Author      - Daniel
// Project     - Lemongrass
// Description - Provides a generic container for ScriptableObject data types
//  to be loaded and accessible through a dictionary.
//---------------------------------------------------------------------------//
using System.Collections.Generic;
using UnityEngine;

namespace Lemongrass
{
  /**
   * @brief A generic container for ScriptableObject data that is used as a resource
   *  collection. For example, scriptable objects for card data where cards are defined by ID
   */
  public class ScriptableObjectDataSource<Resource, Key> where Resource : ScriptableObject
  {
    // This delegate defines a way to access the resource key when given a resource
    // for example, (resource) => resource.enumType
    public delegate Key KeyResource(Resource resource);

    ////////////////////////////////////Variables//////////////////////////////////
    //-public--------------------------------------------------------------------//

    /**
     * @brief Returns the resource associated with the indexing key.
     * @return The requested resource. Return null if the key was not present in the dictionary
     */
    public Resource this[Key key]
    {
      get 
      {
        if (!resourceByKey.ContainsKey(key)) return null;
        return resourceByKey[key]; 
      }
    }

    //-private-------------------------------------------------------------------//
    private Dictionary<Key, Resource> resourceByKey
    {
      get
      {
        if (resourceDataMap == null)
        {
          // populate a new resource data map.
          resourceDataMap = new Dictionary<Key, Resource>();
          foreach (Resource data in resourceData)
          {
            Key key = GetResourceKey(data);
            // flag duplicates with warnings. This data collection does not support duplicates
            if (resourceDataMap.ContainsKey(key))
            {
              throw new System.Exception($"Duplicate {data.GetType()} definition detected: {key}");
            }
            resourceDataMap[key] = data;
          }
        }
        return resourceDataMap;
      }
    }

    private Resource[] resourceData;
    private Dictionary<Key, Resource> resourceDataMap;

    private KeyResource GetResourceKey = null;

    ////////////////////////////////////Functions//////////////////////////////////
    //-public--------------------------------------------------------------------//

    /**
     * @brief Initializes the collection with the proper resources
     * @param resourcePath The path to the resource collection. Should be a
     *        folder full of scriptable objects.
     *        IMPORTANT: Expected to be relative to the Resources/ path in the
     *        project assets.
     * @param keyAccessDelegate A delegate function that will return the key used to
     *        organize the objects. Probably a function that returns an enum
     * @return The number of resources that this collection was initialized with
     */
    public bool Initialize(string resourcePath, KeyResource keyAccessDelegate)
    {
      Debug.Assert(keyAccessDelegate != null);

      GetResourceKey = keyAccessDelegate;

      // loads all of the resource data from the given path
      resourceData = Resources.LoadAll<Resource>(resourcePath);
      resourceDataMap = null;

      return resourceData.Length > 0;
    }
  }
}
