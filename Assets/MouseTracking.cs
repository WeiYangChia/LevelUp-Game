using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseTracking : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //Mousetracking
    public mouseHeatMap HMap;
    public float mouseTimer;
    public string location;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        mouseTimer += Time.deltaTime;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseTimer = 0f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HMap.RecordMouse(location, mouseTimer);
    }
}
