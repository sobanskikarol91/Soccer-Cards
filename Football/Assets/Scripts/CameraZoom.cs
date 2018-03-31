using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraZoom : MonoBehaviour
{
    Camera camera;
    float maxCameraSize;
    float MinCameraSize = 8f;
    const float zoomSpeed = 0.2f;
    const float pcZoomSpeed = 20f;
    float orthoZoomSpeed = 0.05f;

    private void Start()
    {
        camera = GetComponent<Camera>();


        if (Application.isMobilePlatform)
            StartCoroutine(MobileZoom());
        else
            StartCoroutine(PCZoom());        
    }

    IEnumerator PCZoom()
    {
        while (true)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0f && CheckCameraSize())
                ZoomIn();
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f && CheckCameraSize())
                ZoomOut();

            CheckBoundry();
            yield return null;
        }
    }

    void ZoomIn()
    {
        camera.orthographicSize -= Time.deltaTime * pcZoomSpeed;
    }

    void ZoomOut()
    {
        camera.orthographicSize += Time.deltaTime * pcZoomSpeed;
    }

    IEnumerator MobileZoom()
    {
        while (true)
        {
            //if (Input.touchCount > 0)
            CheckBoundry();

            if (Input.touchCount == 2)
            {
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float currTouchDeltaMag = (touchZero.position - touchOne.position).magnitude;

                float deltaMagnitudeDiff = prevTouchDeltaMag - currTouchDeltaMag;


                camera.orthographicSize += deltaMagnitudeDiff * orthoZoomSpeed * Time.deltaTime;
                // to make sure that size won't be < 0.1f
                camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, MinCameraSize, maxCameraSize);

            }
            yield return null;
        }
    }

    bool CheckCameraSize()
    {
        if (CheckWidth() && CheckHeight()) return true;

        ZoomIn();
        return false;
    }

    bool CheckWidth()
    {
        return 2f * camera.orthographicSize * camera.aspect < Boundry.width;

    }

    bool CheckHeight()
    {
        return 2f * camera.orthographicSize < Boundry.height;

    }

    void CheckBoundry()
    {
        CheckCameraSize();

        float height = 2f * camera.orthographicSize;
        float width = height * camera.aspect;

            
        float xMin = -Boundry.x + width / 2;
        float xMax = Boundry.x - width / 2;

        float yMin = -Boundry.y + height / 2;
        float yMax = Boundry.y - height / 2;

        float x = transform.position.x;
        float y = transform.position.y;

        x = Mathf.Clamp(x, xMin, xMax);
        y = Mathf.Clamp(y, yMin, yMax);

        transform.position = new Vector3(x, y, transform.position.z);
    }
}
