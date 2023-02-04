//---------------------------------------------------------------------------//
// Name        - TooltipObject.cs
// Author      - Daniel
// Project     - Lemongrass
// Description - Mousing over an object with this behavior will generate a 
//  tooltip with the given string contents
//---------------------------------------------------------------------------//
using UnityEngine;
using UnityEngine.EventSystems;

namespace Lemongrass
{
  public class TooltipObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
  {
    ////////////////////////////////////Variables//////////////////////////////////
    //-public--------------------------------------------------------------------//
    
    [Tooltip("The type of prefab that should be used with this tooltip")]
    public Subsystem_Tooltips.TooltipType tooltipType = Subsystem_Tooltips.TooltipType.Light;
    
    //-private-------------------------------------------------------------------//
    [SerializeField, TextArea, Tooltip("This text will be displayed on the tooltip")]
    private string tooltip = "Not Set";

    ////////////////////////////////////Functions//////////////////////////////////
    //-public--------------------------------------------------------------------//
    
    /**
     * @brief Gets the text of this tooltip.
     * @return The tooltip text.
     */
    public string GetText()
    {
      return tooltip;
    }

    ///////////////////////////////Pointer Interface///////////////////////////////
    public void OnPointerEnter(PointerEventData eventData)
    {
      GameInstance.GetSubsystem<Subsystem_Tooltips>().AddTooltip(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
      GameInstance.GetSubsystem<Subsystem_Tooltips>().RemoveTooltip();
    }
    ///////////////////////////////////////////////////////////////////////////////
  }
}
