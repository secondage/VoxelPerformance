using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragRotate : MonoBehaviour {
    public float speed = 0.1f;

    private bool _mouseDown = false;
    private bool _mouseRDown = false;
    public Text debugText;

    public float Position = 0.0f;

    public GameObject cutObj;

    void Update()
    {
        if (EventSystem.current == null)
            return;
        //if point on ui controller, skip it
#if UNITY_ANDROID
        PointerEventData ped = new PointerEventData(EventSystem.current);
        ped.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> result = new List<RaycastResult>();
        EventSystem.current.RaycastAll(ped, result);
        if (result.Count > 0)
            return;
#else
        if (EventSystem.current.IsPointerOverGameObject())
            return;
#endif
        if (Input.GetMouseButtonDown(0))
            _mouseDown = true;
        else if (Input.GetMouseButtonUp(0))
            _mouseDown = false;

        if (Input.GetMouseButtonDown(1))
            _mouseRDown = true;
        else if (Input.GetMouseButtonUp(1))
            _mouseRDown = false;

        

        if ((Application.platform == RuntimePlatform.WindowsEditor || Input.touchCount == 1) && _mouseDown)
        {
#if UNITY_ANDROID
            float fMouseX = Input.GetAxis("Mouse X");
            float fMouseY = Input.GetAxis("Mouse Y");
            if (Application.platform != RuntimePlatform.WindowsEditor)
            {
                Touch touch = Input.GetTouch(0);

                fMouseX = touch.deltaPosition.x * 0.2f;
                fMouseY = touch.deltaPosition.y * 0.2f;
            }
#else
            float fMouseX = Input.GetAxis("Mouse X");
            float fMouseY = Input.GetAxis("Mouse Y");
#endif
            /*transform.Rotate(new Vector3(transform.rotation.eulerAngles.x,
                transform.rotation.eulerAngles.y + (mouseXLast - fMouseX) * speed,
                transform.rotation.eulerAngles.z));*/

            Vector3 axis = (transform.forward + transform.right).normalized;
            if (VoxelCut.rotateHot == 1)
            {
                transform.Rotate(new Vector3(-1.0f, 0.0f, 1.0f).normalized, fMouseY * speed, Space.World);
                transform.Rotate(Vector3.up, -fMouseX * speed, Space.World);
            }
            else if (VoxelCut.rotateHot == 2)
            {
                cutObj.transform.Rotate(new Vector3(-1.0f, 0.0f, 1.0f).normalized, fMouseY * speed, Space.World);
                cutObj.transform.Rotate(Vector3.up, -fMouseX * speed, Space.World);
            
            }
            debugText.text = "mousex is " + fMouseX + " mousey is " + fMouseY;


        }

        if ((Application.platform == RuntimePlatform.WindowsEditor || Input.touchCount == 2) && _mouseRDown)
        //    if (_mouseRDown && !_mouseDown)
        {
#if UNITY_ANDROID
            float fMouseX = Input.GetAxis("Mouse X");
            float fMouseY = Input.GetAxis("Mouse Y");
            if (Application.platform != RuntimePlatform.WindowsEditor)
            {
                Touch touch = Input.GetTouch(1);

                fMouseX = touch.deltaPosition.x * 0.2f;
                fMouseY = touch.deltaPosition.y * 0.2f;
            }
#else
            float fMouseX = Input.GetAxis("Mouse X");
            float fMouseY = Input.GetAxis("Mouse Y");
#endif

            Position += (cutObj.transform.up.y > 0 ? fMouseY : -fMouseY) * speed;
            VoxelCut vc = GetComponent<VoxelCut>();
            Position = Mathf.Clamp(Position, -vc.sizeMax.y * 0.6f, vc.sizeMax.y * 0.6f);
            
            //cutObj.transform.Rotate(Vector3.up, -fMouseX * speed, Space.World);
        }
        cutObj.transform.position = cutObj.transform.up *  Position;

    }
}
