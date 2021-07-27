using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Game : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    // Start is called before the first frame update
   
    int level;
    int wires;
    int time;
    int score;
    public Camera camera;
    public int startWires;
    public int wiresIncrese;
    public int maxWires;
    public int startTime;
    public int timeDecrese;
    public Color[] wireColors;

    [Header("UI")]
    public Text levelUI;
    public Text scoreUI;
    public Text timeUI;

    [Header("Zones")]
    public RectTransform leftGameZone;
    public RectTransform rightGameZone;
    public Contact contactPrefab;
    public Wire wirePrefab;
    public DebugPanel dp;

    List<Wire> wireList= new List<Wire>();
    List<Contact> contactList = new List<Contact>();
    
    bool lastLevel;
    Wire tempWire;
    RectTransform tempWire_rt;

    public void Start()
    {
        level = 1;
        score = 0;
        wires = startWires;
        time = startTime;
        lastLevel = false;
        GenerateLevel();
        
    }

    // Update is called once per frame
    void Update()
    {

        
    }

    IEnumerator TimeCountr()
    {
        yield return new WaitForSeconds(1f);
        time--;
        UpdateUI();
        if (time > 0) {
            StartCoroutine(TimeCountr()); 
        } else {
            time = 0;
            EndGame(); 
        }
        
    }

    void UpdateUI()
    {
        levelUI.text = level.ToString();
        scoreUI.text = score.ToString();
        timeUI.text = time.ToString();
    }

    void GenerateLevel()
    {
        StopAllCoroutines();
        //Clear level
        ClearLevel();
        //Generate new 

        Spawn(true);        //generaet left
        Spawn(false);       // generate right
        UpdateUI();
        StartCoroutine(TimeCountr());
    }


    void Spawn(bool left)
    {
        List<int> ids = new List<int>();
        for (int i = 0; i < wires; i++)
        {
            ids.Add(i);
        }

        for (int i = 0; i < wires; i++)
        {
            Contact contact = Instantiate(contactPrefab);
            if (left)
            {
                contact.transform.SetParent(leftGameZone, false);
            }
            else
            {
                contact.transform.SetParent(rightGameZone, false);
            }
            int rnd = Random.Range(0, ids.Count);
            contact.Initialize(ids[rnd], wireColors[ids[rnd]], left, this);
            if (ids[rnd] == 0)
            {
                //dp.SetContactPosition(contact.gameObject.GetComponent<RectTransform>().position, left);
            }
            ids.RemoveAt(rnd);
            contactList.Add(contact);
        }
    }

    void ClearLevel()
    {
        foreach(Wire w in wireList)
        {
            Destroy(w.gameObject);
        }
        wireList.Clear();
        foreach(Contact c in contactList)
        {
            Destroy(c.gameObject);
        }
        contactList.Clear();
        if (tempWire)
        {
            Destroy(tempWire.gameObject);
            tempWire = null;
        }
    }

    void CheckLevel()
    {
        int connectedCount = 0;
        foreach(Contact c in contactList)
        {
            if (c.connected)
            {
                connectedCount++;
            }
        }
        if (connectedCount >= contactList.Count) {
            score += time * level;
            UpdateUI();
            NextLevel(); 
        }
        ClearGarbage();
    }

    void NextLevel()
    {
        //ClearLevel();
        LevelUp();
        GenerateLevel();
    }
    void LevelUp()
    {
        if (!lastLevel)
        {
            wires = level * wiresIncrese + startWires;
            time = level * timeDecrese + startTime;
            level++;
            if (wires >= maxWires)
            {
                lastLevel = true;
                if (wires > maxWires)
                {
                    wires = maxWires;
                }
            }
        }
        else
        {
            EndGame();
        }
    }

    public void InstTempWire(Vector3 pos, int id, Contact con)
    {
        tempWire = Instantiate(wirePrefab);
        
        tempWire.Initialize(id, wireColors[id],pos, con);
        tempWire.name = "Wire";
        tempWire.transform.SetParent(rightGameZone.parent, false);
        tempWire.transform.position = pos+ new Vector3(25f,0,0);
        tempWire_rt = tempWire.GetComponent<RectTransform>();
        tempWire_rt.sizeDelta = new Vector2(50f, 20f);
    }

    void ClearGarbage()
    {
        for(int i=0; i<this.gameObject.transform.GetChild(1).childCount; i++)
        {
            Transform child = this.gameObject.transform.GetChild(1).GetChild(i);
            if (child.name == "Wire")
            {

                child.GetComponent<Wire>().CheckConnection();
                
            }
        }
    }

    void EndGame()
    {
        ClearLevel();
        ClearGarbage();
        GameObject.Find("GameManager").GetComponent<GameManager>().FinishGame(score);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (tempWire != null)
        {
            Vector2 twpos = Vector2.Lerp(eventData.pointerCurrentRaycast.screenPosition, tempWire.startPosition, .5f);
            tempWire.transform.position = twpos;
            float coef = 720f / Screen.width;
            tempWire_rt.sizeDelta = new Vector2(Vector2.Distance(eventData.pointerCurrentRaycast.screenPosition, tempWire.startPosition)*coef+30f, 20f);
            tempWire_rt.eulerAngles = new Vector3(0f, 0f, -Vector2.SignedAngle(eventData.pointerCurrentRaycast.screenPosition - tempWire.startPosition, Vector2.right));
        }
        if (eventData.pointerEnter != null)
        {
            if (eventData.pointerEnter.name == "contact")
            {
                //eventData.pointerEnter.GetComponent<Image>().color = Color.green;
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
       //Debug.Log("You dragging! " + 600f/Screen.width);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.pointerEnter != null)
        {
            if (eventData.pointerEnter.name == "contact")
            {
                if (eventData.pointerEnter.GetComponent<Contact>().id == tempWire.id)
                {
                    if (tempWire != null)
                    {
                        Vector2 endpos = eventData.pointerEnter.GetComponent<RectTransform>().position;
                        eventData.pointerEnter.GetComponent<Contact>().connected = true;
                        tempWire.startContact.connected = true;
                        Vector2 twpos = Vector2.Lerp(endpos, tempWire.startPosition, .5f);
                        tempWire.transform.position = twpos;
                        float coef = 720f / Screen.width;
                        tempWire_rt.sizeDelta = new Vector2(Vector2.Distance(endpos, tempWire.startPosition)*coef + 30f, 20f);
                        tempWire_rt.eulerAngles = new Vector3(0f, 0f, -Vector2.SignedAngle(endpos - tempWire.startPosition, Vector2.right));
                        tempWire.ConnectWire();
                        wireList.Add(tempWire);
                        tempWire = null;
                        score += 2;
                        UpdateUI();
                        CheckLevel();
                    }
                }
                else
                {
                    if (tempWire != null)
                    {
                        Destroy(tempWire.gameObject);
                    }
                }
                if (tempWire != null)
                {
                    tempWire.CheckConnection();
                }
            }
            else
            {
                
                if (tempWire != null)
                {
                    Destroy(tempWire.gameObject);
                }
            }
        }
        else
        {
            if (tempWire != null)
            {
                Destroy(tempWire.gameObject);
            }
        }
    }
}
