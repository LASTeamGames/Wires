
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Contact : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    // Start is called before the first frame update
    public int id;
    public Color color;
    public bool connected;
    public bool busy;
    public bool left;
    Game game;

    public void Initialize(int id, Color color, bool left, Game g)
    {
        this.id = id;
        this.color = color;
        this.left = left;
        this.game = g;
        
        gameObject.GetComponent<Image>().color = color;
        
        gameObject.name = "contact";
        
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.pointerEnter != null)
        {
            if (!connected)
            {
                //gameObject.GetComponent<Image>().color = Color.green;
                game.InstTempWire(eventData.pointerEnter.GetComponent<RectTransform>().position, id, this);
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        gameObject.GetComponent<Image>().color = color;

    }
    public void OnPointerExit(PointerEventData eventData)
    {
        gameObject.GetComponent<Image>().color = color;
    }
}
