//---------------------------------------------------------------------------//
// Name        - GameInstance.cs
// Author      - Daniel
// Project     - Lemongrass
// Description - This object would persist for the life of the program. It
//  manages and updates any systems and subsystems that should also persist
//  for the life of the program.
//---------------------------------------------------------------------------//
using System.Collections.Generic;
using UnityEngine;

namespace Lemongrass
{
  /**
   * @brief GameSubsystems can be managed and updated through the global GameInstance.
   *  Inherit this class and then register the subsystem in GameInstance::RegisterSubsystem
   */
  public abstract class GameSubsystem
  {
    public bool IsInitialized { internal set; get; } = false;

    /**
     * @brief Function called when the subsystem is registered.
     */
    protected internal abstract void Initialize();

    /**
     * @brief Function called when the owning GameInstance object 
     *        receives the Update event
     */
    protected internal abstract void Update();

    /**
     * @brief Function called when the owning GameInstance object
     *        is destroyed. This is intended to happen when the program
     *        is terminated
     */
    protected internal abstract void Shutdown();

    /**
     * @brief Function called when the owning GameInstance object
     *        receives the OnGui event. Useful for debugging subsystems
     */
    protected internal virtual void OnGui() { }
  }

  /**
   * @brief Helper class for providing singletons of instantiated subsystems
   */
  internal static class SubsystemCache<T> where T : GameSubsystem, new()
  {
    public static readonly GameSubsystem Value = new T();
  }

  /**
   * @brief This object would persist for the life of the program.
   *        It manages and updates any systems and subsystems that should also persist
   *        for the life of the program. Subsystems should be registered in 
   *        GameInstance::RegisterSubsystem and can be configured through ini files.
   *        See Editor/SubsystemConfigEditor.cs
   */
  public class GameInstance : MonoBehaviour
  {
    public delegate void VoidVoid();

    /** 
     * @brief Returns the singleton instance of the GameInstance object. There should only
     *        ever be one GameInstance throughout the life of the program
     */
    public static GameInstance Instance
    {
      get
      {
        if (_instance == null || !_instance.IsInitialized)
        {
          Debug.LogError("Tried to access the GameInstance before it was initialized. " +
          "The GameInstance wont be available until after Awake or something. " +
          "Use Event_GameInstanceInitialized to know exactly when the game instance is ready."); // + ratio
          return null;
        }
        return _instance;
      }
    }
    private static GameInstance _instance;

    /**
     * @brief Can be used to check if the GameInstance singleton is ready to be accessed
     */
    public static bool IsGameInstanceInitialized
    {
      get
      {
        return (_instance != null && _instance.IsInitialized);
      }
    }

    /**
     * @brief Returns the singleton instance of the templated subsystem.
     *        It is perfectly cromulent to access subsystems directly, the GameInstance
     *        merely handles the updating and lifetime of a subsystem.
     *        NOTE: The subsystem can be retrieved even if it wasn't registered
     *        All subsystems should be registered in GameInstance::Initialize
     * @return A reference to the game subsystem singleton.
     */
    public static T GetSubsystem<T>() where T : GameSubsystem, new()
    {
      if (!IsGameInstanceInitialized) return null; // the subsystem won't be ready until the GameInstance is ready
      return SubsystemCache<T>.Value as T;
    }

    /**
     * @brief Can be used to listen for when the GameInstance gets initialized. This should happen
     *        sometime around the first ever Awake
     */
    public static VoidVoid Event_GameInstanceInitialized;

    // Runtime Initialization ////////////////////////////////////////////////////////////////

    /**
     * @brief Creates the GameInstance object and puts it in the "don't destroy on load" scene
     *        The GameInstance will not be created if UseGameInstance=false GameInstance.ini
     */
    [RuntimeInitializeOnLoadMethod]
    static void InitializeGameInstanceObject()
    {
      if (IsGameInstanceInitialized) return;

      INIParser parser = new INIParser();
      parser.OpenConfig("GameInstance.ini");
      bool useGameInstance = parser.ReadValue("GameInstance", "UseGameInstance", false);
      parser.Close();

      if (!useGameInstance) return;

      GameObject gameInstanceObject = new GameObject("GameInstanceSingleton");
      _instance = gameInstanceObject.AddComponent<GameInstance>();

      _instance.Initialize();
    }
    ///////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////Variables//////////////////////////////////
    //-public--------------------------------------------------------------------//
    public bool IsInitialized = false;

    //-private-------------------------------------------------------------------//
    // contains the list of subsystems for updating
    private List<GameSubsystem> activeSubsystems = new List<GameSubsystem>();

    ////////////////////////////////////Functions//////////////////////////////////
    //-private-------------------------------------------------------------------//

    // Call this for each subsystem that you want to have uzed in your game instance.
    // See GameInstance::Initialize
    private void RegisterSubsystem<T>() where T : GameSubsystem, new()
    {
      if (activeSubsystems.Find((item) => item.GetType() == typeof(T)) != null)
      {
        // This is not necessarily an error, if I have the stones to add dependencies
        Debug.Log($"System {typeof(T)} was already registered. Skipping it");
        return;
      }

      // The system was not found so we should create and initialize it
      GameSubsystem system = SubsystemCache<T>.Value;
      if (!system.IsInitialized)
      {
        Debug.Log($"Now initializing {system.GetType()}");
        system.Initialize();
        system.IsInitialized = true;
      }
      activeSubsystems.Add(system);
    }

    // determine which subsystems you want to have loaded here.
    private void Initialize()
    {
      DontDestroyOnLoad(gameObject);

      // These systems are not needed in release builds of the game.
      // Could bake this into the subsystem but I don't think it is necessary
#if DEVELOPMENT_BUILD || UNITY_EDITOR
      // Provides additional logging features like log to screen
      RegisterSubsystem<Subsystem_Logging>();
#endif

      // Supports tooltips through TooltipObject and TooltipBehaviour
      RegisterSubsystem<Subsystem_Tooltips>();

      FinishInitialization();
    }
    private void FinishInitialization()
    {
      IsInitialized = true;
      Event_GameInstanceInitialized?.Invoke();
    }

    // Unity update (this is a game object)
    private void Update()
    {
      foreach (GameSubsystem system in activeSubsystems) system.Update();
    }

    private void ShutdownAllSubsystems()
    {
      foreach (GameSubsystem system in activeSubsystems) system.Shutdown();
      activeSubsystems.Clear();
    }

    private void OnDestroy()
    {
      ShutdownAllSubsystems();
    }

    private void OnGUI()
    {
      foreach (GameSubsystem system in activeSubsystems) system.OnGui();
    }
  }
}
