using UnityEngine;
using UnityEngine.UI;

public class Wire : MonoBehaviour
{
    public int id;
    public Color color;
    public Vector2 startPosition;
    public Vector2 endPosition;
    public Contact startContact;
    public bool doubleConnect;

    public void Initialize(int id, Color color, Vector3 start, Contact con)
    {
        this.id = id;
        this.color = color;
        gameObject.GetComponent<Image>().color = color;
        this.startPosition = new Vector2(start.x, start.y);
        this.startContact = con;
        
    }

    public void ConnectWire()
    {
        doubleConnect = true;
    }
    public void CheckConnection()
    {
        if (!doubleConnect) 
        {
            Destroy(this.gameObject);
        }
    }

    public bool IsConnected()
    {
        return doubleConnect;
    }

}
