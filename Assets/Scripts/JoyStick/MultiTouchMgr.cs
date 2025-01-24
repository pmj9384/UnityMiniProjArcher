using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiTouchMgr : Singleton<MultiTouchMgr>
{
    public bool IsTap { get; private set; }
    public bool IsDoubleTap { get; private set; }
    public bool IsLongPress { get; private set; }

    public bool IsSwipeUp { get; private set; }
    public bool IsSwipeDown { get; private set; }
    public bool IsSwipeLeft { get; private set; }
    public bool IsSwipeRight { get; private set; }

    public float PinchZoomValue { get; private set; }

    public bool IsRotate { get; private set;}
    private float previousAngle = 0f;
    private float SwipeX{get; set;}
    private float SwipeY{get; set;}
    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;
    private Vector2[] startTouches = new Vector2[2];  
    private Vector2[] currentTouches = new Vector2[2];

    private const float swipeThreshold = 50f;


private float tapTime; 
    private float longPressStartTime;
    private const float doubleTapThreshold = 0.3f;
    private const float longPressThreshold = 0.8f;


    private void Update()
    {
        DetectTapDoubleTapLongPress();
        DetectSwipe();
        DetectPinchZoom();
        DetectRotate();
    }

  
    private void DetectTapDoubleTapLongPress()
    {
        IsTap = false;
        IsDoubleTap = false;
        IsLongPress = false;

        if (Input.touchCount == 1) 
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                IsTap = true;

                if (Time.time - tapTime <= doubleTapThreshold)
                {
                    IsDoubleTap = true;
                    Debug.Log("Double Tap");
                }

                tapTime = Time.time; 
                longPressStartTime = Time.time;
                Debug.Log("Tap !");

            }

            if (touch.phase == TouchPhase.Stationary && Time.time - longPressStartTime >= longPressThreshold)
            {
                IsLongPress = true;
                Debug.Log("Long Press!");
            }
        }
    }
    private System.Collections.IEnumerator HandleDoubleTap()
    {
        yield return new WaitForSeconds(0.3f);
        if (Input.touchCount == 1 && IsTap)
        {
            IsDoubleTap = true;
        }
    }
    private void DetectSwipe()
    {
 
        IsSwipeUp = false;
        IsSwipeDown = false;
        IsSwipeLeft = false;
        IsSwipeRight = false;

        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
        
                startTouchPosition = touch.position;
            }

            if (touch.phase == TouchPhase.Ended)
            {
             
                endTouchPosition = touch.position;


                Vector2 delta = endTouchPosition - startTouchPosition;

                
                if (delta.magnitude >= swipeThreshold)
                {
                    if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                    {
              
                        if (delta.x > 0)
                        {
                            IsSwipeRight = true;
                            Debug.Log("Swipe Right");
                        }
                        else
                        {
                            IsSwipeLeft = true;
                            Debug.Log("Swipe Left");
                        }
                    }
                    else
                    {
                        if (delta.y > 0)
                        {
                            IsSwipeUp = true;
                            Debug.Log("Swipe Up");
                        }
                        else
                        {
                            IsSwipeDown = true;
                            Debug.Log("Swipe Down");
                        }
                    }
                }
            }
        }
    }
    private void DetectPinchZoom()
    {
        PinchZoomValue = 0;

        if (Input.touchCount == 2)
        {
            currentTouches[0] = Input.GetTouch(0).position;
            currentTouches[1] = Input.GetTouch(1).position;

            if (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved)
            {
                float previousDistance = Vector2.Distance(startTouches[0], startTouches[1]);
                float currentDistance = Vector2.Distance(currentTouches[0], currentTouches[1]);
                PinchZoomValue = currentDistance - previousDistance;
                if (PinchZoomValue > 0)
                {
                    Debug.Log("Zoom");
                }
                else if (PinchZoomValue < 0)
                {
                    Debug.Log("Pinch");
                }
            }

            startTouches[0] = currentTouches[0];
            startTouches[1] = currentTouches[1];
        }
    }
private void DetectRotate()
{
    IsRotate = false;

    if (Input.touchCount == 2)
    {
        Vector2 touch0Prev = Input.GetTouch(0).position - Input.GetTouch(0).deltaPosition;
        Vector2 touch1Prev = Input.GetTouch(1).position - Input.GetTouch(1).deltaPosition;
        
        Vector2 touch0Current = Input.GetTouch(0).position;
        Vector2 touch1Current = Input.GetTouch(1).position;

        float anglePrev = Vector2.Angle(touch0Prev - touch1Prev, Vector2.right);
        float angleCurrent = Vector2.Angle(touch0Current - touch1Current, Vector2.right);

        if (Mathf.Abs(angleCurrent - anglePrev) > 1f)
        {
            IsRotate = true;
            float rotationDelta = angleCurrent - anglePrev;

                Debug.Log("Rotate!");
            
            Camera.main.transform.Rotate(Vector3.forward, rotationDelta, Space.World);

            previousAngle = angleCurrent;
        }
    }
}
}
