//---------------------------------------------------------------------------//
// Name        - SubsystemConfigEditor.cs
// Author      - Daniel
// Project     - Lemongrass
// Description - Provides an editor interface for changing ini config values
//  in editor. Changing values with this interface is equivalent to changing
//  the raw text in the ini file.
//---------------------------------------------------------------------------//
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

using IniDictionary = System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, string>>;

namespace Lemongrass
{
  /**
   * @brief This editor window will display all values in config files that are stored in
   *        Assets/Config/. Allows editing the config values in text without editing the files directly
   */
  public class SubsystemConfigEditor : EditorWindow
  {
    /**
     * @brief Associates the section headers with their keys and values
     */
    private class IniFileSectionInformation
    {
      public string ConfigFilePath;
      public string SectionTitle;

      public IniFileSectionInformation(string filePath, string sectionTitle)
      {
        ConfigFilePath = filePath;
        SectionTitle = sectionTitle;
      }

      public void UpdateConfigValue(string Key, string Value)
      {
        Debug.Log($"Updating config value {Key} to {Value}");
        INIParser parser = new INIParser();
        parser.Open(ConfigFilePath);
        parser.WriteValue(SectionTitle, Key, Value);
        parser.Close();
      }
    }

    [SerializeField] private int selectedLeftPaneIndex = 0;
    private VisualElement rightPane;
    private List<IniFileSectionInformation> currentConfigSections;

    [MenuItem("Lemongrass/Window/Subsystem Config Editor", false, -1)]
    public static void ShowMyEditor()
    {
      // This method is called when the user selects the menu item in the Editor
      EditorWindow wnd = GetWindow<SubsystemConfigEditor>();
      wnd.titleContent = new GUIContent("Subsystem Config Editor");

      // Limit size of the window
      wnd.minSize = new Vector2(450, 200);
      wnd.maxSize = new Vector2(1200, 500);
    }

    public void CreateGUI()
    {
      string configFilePath = Application.dataPath + "/Config/";

      currentConfigSections = new List<IniFileSectionInformation>();
      List<string> filePaths = new List<string>();

      if (Directory.Exists(configFilePath))
      {
        filePaths = new List<string>(Directory.GetFiles(configFilePath, "*.ini"));
      }
      // load all the resources 
      if (filePaths.Count <= 0)
      {
        // there was no config file path yet, let the user know
        rootVisualElement.Add(new Label($"No config files found!\nAdd your config files to {configFilePath}.\nThey must have the .ini extension"));
        return;
      }

      // Create a two-pane view with the left pane being fixed with
      TwoPaneSplitView splitView = new TwoPaneSplitView(0, 250, TwoPaneSplitViewOrientation.Horizontal);

      // Add the view to the visual tree by adding it as a child to the root element
      rootVisualElement.Add(splitView);

      // A TwoPaneSplitView always needs exactly two child elements
      // Initialize the list view with all sprites' names
      ListView leftPane = new ListView();
      splitView.Add(leftPane);
      leftPane.makeItem = () => new Label();
      leftPane.bindItem =
        (item, index) =>
        {
          string configName = filePaths[index].Substring(filePaths[index].LastIndexOf('/') + 1);
          configName = configName.Substring(0, configName.LastIndexOf('.'));
          (item as Label).text = SplitLineOnCamelCase(configName);
        };

      // support for hot reloading
      leftPane.selectedIndex = selectedLeftPaneIndex;
      leftPane.onSelectionChange += (items) => { selectedLeftPaneIndex = leftPane.selectedIndex; };

      leftPane.itemsSource = filePaths;
      leftPane.onSelectionChange += LeftPaneOnSelectionChange;

      rightPane = new VisualElement();
      splitView.Add(rightPane);
    }

    private void LeftPaneOnSelectionChange(IEnumerable<object> obj)
    {
      rightPane.Clear();
      currentConfigSections.Clear();

      string configPath = obj.First() as string;

      // open the ini and get all the plain text information
      INIParser parser = new INIParser();
      parser.Open(configPath);
      IniDictionary sectionInformation = parser.GetCachedSections();
      parser.Close();

      foreach (KeyValuePair<string, Dictionary<string, string>> section in sectionInformation)
      {
        IniFileSectionInformation sectionElement = new IniFileSectionInformation(configPath, section.Key);
        rightPane.Add(new Label("<b>[" + section.Key + "]"));

        foreach (KeyValuePair<string, string> variable in section.Value)
        {
          TextField iniField = new TextField(SplitLineOnCamelCase(variable.Key));
          iniField.isDelayed = true;
          iniField.value = variable.Value;
          iniField.RegisterValueChangedCallback(OnIniValueChanged);
          // I guess I am capturing a lambda with this function.
          // This literally seems like the worst code I have ever written
          // also I don't understand the memory implications of this
          void OnIniValueChanged(ChangeEvent<string> e)
          {
            sectionElement.UpdateConfigValue(variable.Key, e.newValue);
          }

          //sectionElement.AddStringValue(variable.Key, variable.Value);
          rightPane.Add(iniField);
        }
        currentConfigSections.Add(sectionElement);
      }
    }

    // Modifies a string to split on camel case and be more human readable
    private string SplitLineOnCamelCase(string line)
    {
      Regex r = new Regex(@"
                (?<=[A-Z])(?=[A-Z][a-z]) |
                 (?<=[^A-Z])(?=[A-Z]) |
                 (?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.IgnorePatternWhitespace);

      return r.Replace(line, " ");
    }
  }
}
