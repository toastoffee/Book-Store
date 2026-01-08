using UnityEngine;

public class RotateObjectWithMouse : MonoBehaviour
{
    private Vector2 prevMousePosition;
    private bool isRotating = false;
    
    float rotationSpeed = 0.3f;

    void Update()
    {
        // 检测鼠标右键是否按下并保持
        if (Input.GetMouseButtonDown(2)) // 鼠标右键开始按下
        {
            isRotating = true;
            prevMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(2)) // 鼠标右键释放
        {
            isRotating = false;
        }

        if (isRotating)
        {
            // 获取当前帧鼠标的位置
            Vector2 currentMousePosition = Input.mousePosition;

            // 计算鼠标在屏幕上的位移
            float deltaX = currentMousePosition.x - prevMousePosition.x;

            // 根据鼠标的水平位移量计算旋转角度
            transform.Rotate(Vector3.up, -deltaX * rotationSpeed);

            // 更新prevMousePosition为当前帧的鼠标位置
            prevMousePosition = currentMousePosition;
        }
    }
}
