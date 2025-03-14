using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class cameraMovement : MonoBehaviour
{
    [SerializeField] private float min = 1.0f;
    [SerializeField] private float max = 2.5f;
    [SerializeField] private float currentSize = 1.0f;
    [SerializeField] private float changeThreshold = 2.5f;
    [SerializeField] private float changeTresholdLeap = 0.2f;
    [SerializeField] private float moveSmoothness = 2.0f;
    [SerializeField] private float zoomSmoothness = 2.0f;
    [SerializeField] private float zoomOutSmoothness = 0.2f;

    [SerializeField] private GameObject player1;
    [SerializeField] private GameObject player2;
    [SerializeField] private float zoomOutTimeThreshold = 2.0f;
    [SerializeField] private float zoomOutFactor = 1.2f;
    [OptionalField] private bool zoomedOut = false;
    private float zoomOutTime = 0.0f;
    private float actualZoomOutFactor = 1.0f;
    
    private Camera cam;

    public float val;
    
    // Start is called before the first frame update
    void Start()
    {
        this.cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player2 != null)
        {
            this.UpdateZoomOut();

            float distance = Vector2.Distance(player1.transform.position, player2.transform.position);
            val = distance / currentSize;

            if (val > changeThreshold)
            {
                currentSize = Mathf.Lerp(currentSize, max, zoomSmoothness * Time.deltaTime);
            }
            else if (val < changeThreshold - changeTresholdLeap)
            {
                currentSize = Mathf.Lerp(currentSize, min, zoomSmoothness * Time.deltaTime);
            }

            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, currentSize * actualZoomOutFactor,
                zoomSmoothness * Time.deltaTime);
        }
        else
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, min, zoomSmoothness * Time.deltaTime);
        }

        // change camera position to the middle of the two player smoothl
        Vector3 middle;
        if (player2 != null)
        {
            middle = (player1.transform.position + player2.transform.position) / 2;
        }
        else
        {
            middle = player1.transform.position;
        }

        cam.transform.position = Vector3.Lerp(cam.transform.position, new Vector3(middle.x, middle.y, -10),
            moveSmoothness * Time.deltaTime);
    }
    public void TriggerZoomOut()
    {
        this.zoomedOut = true;
        this.zoomOutTime = Time.time;
    }
    private void UpdateZoomOut()
    {
        if(player1.GetComponent<MultiInputSystem>().IsMoving())
        {
            this.TriggerZoomOut();
        }
        if (Time.time - this.zoomOutTime > this.zoomOutTimeThreshold)
        {
            this.zoomedOut = false;
        }
        
        if (this.zoomedOut)
        {
            this.actualZoomOutFactor = Mathf.Lerp(this.actualZoomOutFactor, this.zoomOutFactor, this.zoomOutSmoothness * Time.deltaTime);
        }
        else
        {
            this.actualZoomOutFactor = Mathf.Lerp(this.actualZoomOutFactor, 1.0f, this.zoomOutSmoothness * Time.deltaTime);
        }
    }
}
