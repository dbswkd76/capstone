using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowFPS : MonoBehaviour
{
    [Range(10, 150)]
    public int fontSize = 30;
    public Color color = new Color(.0f, .0f, .0f, 1.0f);
    public float width, height;
    bool isShowFPS = false;

    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            isShowFPS = !isShowFPS;
            Debug.Log("FPS 표시/비표시 전환");
        }
    }
    
    void OnGUI()
    {
        if (isShowFPS)
        {
            Rect position = new Rect(width, height, Screen.width, Screen.height);

            float fps = 1.0f / Time.deltaTime;
            float ms = Time.deltaTime * 1000.0f;
            string text = string.Format("{0:N1} FPS ({1:N1}ms)", fps, ms);

            GUIStyle style = new GUIStyle();

            style.fontSize = fontSize;
            style.normal.textColor = color;

            GUI.Label(position, text, style);
        }
    }
}
