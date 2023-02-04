//---------------------------------------------------------------------------//
// Name        - Subsystem_Tooltips.cs
// Author      - Daniel
// Project     - Lemongrass
// Description - This subsystem creates and manages tooltip objects. 
// expects to be paired with TooltipObject and TooltipBehavior
//---------------------------------------------------------------------------//
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Lemongrass
{
  /**
   * @brief This subsystem creates and manages tooltips. Tooltips will be created
   *        upon mousing over colliders with the TooltipObject  component on them.
   *        TooltipBehavior should be used for tooltip prefabs to change the style
   *        and appearance of tooltips
   */
  public class Subsystem_Tooltips : GameSubsystem
  {
    /**
     * @brief This enum can be expanded to include different types of tooltip.
     *        These will be loaded by the subsystem and used to instantiate the
     *        correct prefab.
     */
    public enum TooltipType
    {
      Light,
      Dark
    }

    ////////////////////////////////////Variables//////////////////////////////////
    //-private-------------------------------------------------------------------//

    // This is where the tooltip prefabs are loaded and organized
    private Dictionary<TooltipType, GameObject> tooltipPrefabData = new Dictionary<TooltipType, GameObject>();

    private GameObject currentTooltipInstance;

    // This canvas will be created to display the tooltips on top of
    private Canvas tooltipCanvas;

    // GameSubsystem implementation //////////////////////////////////////////////////
    protected internal override void Initialize()
    {
      // parse default values
      INIParser parser = new INIParser();
      parser.OpenConfig("SubsystemTooltips.ini");

      string tooltipPrefabPath = parser.ReadValue("SubsystemDefaults", "TooltipPrefabPath", "TooltipPrefabs");

      parser.Close();

      // listen for scene changes
      SceneManager.activeSceneChanged += OnActiveSceneChanged;

      // listen for game instance initializing
      GameInstance.Event_GameInstanceInitialized += OnGameInstanceInitialized;

      // organize all the tooltips into the dictionary
      GameObject[] tooltipPrefabs = Resources.LoadAll<GameObject>(tooltipPrefabPath);
      foreach(GameObject obj in tooltipPrefabs)
      {
        TooltipBehavior tooltip = obj.GetComponent<TooltipBehavior>();
        if (!tooltip)
        {
          Debug.LogError($"Tooltip prefab {obj.name} missing tooltip behavior!");
          continue;
        }

        if (tooltipPrefabData.ContainsKey(tooltip.tooltipType)) Debug.LogError($"Overwriting tooltip prefab for type {tooltip.tooltipType}");
        tooltipPrefabData[tooltip.tooltipType] = tooltip.gameObject;
      }
    }

    protected internal override void Update()
    {
    }

    protected internal override void Shutdown()
    {
    }

    //////////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////Functions//////////////////////////////////
    //-public--------------------------------------------------------------------//

    /**
     * @brief Creates a tooltip to match the data of the given TooltipObject.
     * @param tooltipData The data that will drive what this tooltip looks like.
     */
    public GameObject AddTooltip(TooltipObject tooltipData)
    {
      if (!tooltipPrefabData.ContainsKey(tooltipData.tooltipType))
      {
        Debug.LogError($"Cannot create a tooltip of type {tooltipData.tooltipType}. No available prefab");
        return null;
      }

      if (currentTooltipInstance != null) RemoveTooltip();
      currentTooltipInstance = GameObject.Instantiate(tooltipPrefabData[tooltipData.tooltipType], tooltipCanvas.transform);

      TooltipBehavior tooltip = currentTooltipInstance.GetComponent<TooltipBehavior>();
      tooltip.SetCanvas(tooltipCanvas);
      tooltip.Initialize(tooltipData);

      return currentTooltipInstance;
    }

    /**
     * @brief Removes the current tooltip in the scene from the tooltip canvas
     */
    public void RemoveTooltip()
    {
      if (currentTooltipInstance) GameObject.Destroy(currentTooltipInstance);
    }

    //-private-------------------------------------------------------------------//

    /**
     * @brief By default the tooltip will be removed when the scene changes to 
     *        prevent errant tooltips from existing. If you plan on loading 
     *        scenes additively and then changing the active scene frequently 
     *        then you will want to do something about this
     */
    private void OnActiveSceneChanged(Scene prev, Scene next)
    {
      RemoveTooltip();
    }

    private void OnGameInstanceInitialized()
    {
      // parse canvas values
      INIParser parser = new INIParser();
      parser.OpenConfig("SubsystemTooltips.ini");

      int canvasOrder = parser.ReadValue("Canvas", "CanvasOrder", 1);
      float scaleFactor = (float)parser.ReadValue("Canvas", "ScaleFactor", 1.0f);
      float referencePixelsPerUnit = (float)parser.ReadValue("Canvas", "ReferencePixelsPerUnit", 100.0f);

      parser.Close();

      // create the canvas that will display tooltips
      GameObject canvasObject = new GameObject("TooltipCanvas");
      canvasObject.transform.parent = GameInstance.Instance.transform.parent;

      tooltipCanvas = canvasObject.AddComponent<Canvas>();
      tooltipCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
      tooltipCanvas.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1;
      tooltipCanvas.sortingOrder = canvasOrder;

      CanvasScaler scalar = canvasObject.AddComponent<CanvasScaler>();
      scalar.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
      scalar.scaleFactor = scaleFactor;
      scalar.referencePixelsPerUnit = referencePixelsPerUnit;
    }
  }
}
