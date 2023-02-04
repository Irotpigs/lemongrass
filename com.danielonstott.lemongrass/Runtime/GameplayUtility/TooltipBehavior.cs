//---------------------------------------------------------------------------//
// Name        - TooltipBehavior.cs
// Author      - Daniel
// Project     - Lemongrass
// Description - This behavior would be attached to a tooltip object. Auto
//  resizes and positions the tooltip so that it stays on the screen
//---------------------------------------------------------------------------//
using UnityEngine;
using TMPro;

namespace Lemongrass
{
  public class TooltipBehavior : MonoBehaviour
  {
    ////////////////////////////////////Variables//////////////////////////////////
    //-public--------------------------------------------------------------------//
    
    [Tooltip("The type classification of this tooltip. One prefab should exist per type")]
    public Subsystem_Tooltips.TooltipType tooltipType = Subsystem_Tooltips.TooltipType.Light;

    //-private-------------------------------------------------------------------//

    private Canvas myCanvas = null;
    private RectTransform canvasRect = null;

    [SerializeField]
    [Tooltip("the background of the tooltip to be scaled")]
    private RectTransform backgroundTransform = null;
    private RectTransform ourRect = null;

    [SerializeField]
    [Tooltip("the text component")]
    private TextMeshProUGUI text = null;

    // the spacing between the tooltip and things that are in its way. In unity units
    private float spacing = 15.0f;

    ////////////////////////////////////Functions//////////////////////////////////
    //-public--------------------------------------------------------------------//

    /**
     * @brief Sets this tooltips data with relevant matching data from the given object.
     * @param tooltipData The data to pull tooltip information from;
     */
    public void Initialize(TooltipObject tooltipData)
    {
      text.text = tooltipData.GetText();
      SetToMousePosition();
    }

    /**
     * @brief This must be called for the tooltip to position correctly.
     *        Sets the canvas that the tooltip will fit to.
     * @param parent The canvas that this tooltip is displayed on.
     */
    public void SetCanvas(Canvas parent)
    {
      myCanvas = parent;
      canvasRect = myCanvas?.GetComponent<RectTransform>();
    }

    //-private-------------------------------------------------------------------//
    protected virtual void Start()
    {
      // size the tooltip correctly
      Vector2 backgroundSize = new Vector2(text.preferredWidth, text.preferredHeight);
      backgroundTransform.sizeDelta = backgroundSize;

      // collect our rect transform so we can modify the anchor
      ourRect = GetComponent<RectTransform>();
    }

    protected virtual void Update()
    {
      if (!myCanvas || !canvasRect) return;

      SetToMousePosition();

      // now determine if the tooltip would go off screen
      Vector2 anchorPos = ourRect.anchoredPosition;
      if ((anchorPos.y + backgroundTransform.rect.height) > canvasRect.rect.height)
      {
        anchorPos.y = canvasRect.rect.height - (backgroundTransform.rect.height + spacing);
        // also add a little bit to x because we want to avoid being in the mouse
        anchorPos.x += spacing;
      }
      if ((anchorPos.x + backgroundTransform.rect.width + spacing) > canvasRect.rect.width)
      {
        anchorPos.x = canvasRect.rect.width - (backgroundTransform.rect.width + spacing);
      }

      ourRect.anchoredPosition = anchorPos;
    }

    private void SetToMousePosition()
    {
      // determine the position of the mouse
      Vector2 pos;
      RectTransformUtility.ScreenPointToLocalPointInRectangle(myCanvas.transform as RectTransform, Input.mousePosition, myCanvas.worldCamera, out pos);
      transform.position = myCanvas.transform.TransformPoint(pos);
    }
  }
}
