using UnityEngine;
using UnityEngine.UI;

public class FPS : MonoBehaviour
{
    float elapsed = 0f;
    Text txt;
    // Start is called before the first frame update
    void Awake()
    {
        txt = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        elapsed += Time.deltaTime;
        if (elapsed >= 1f)
        {
            elapsed %= 1f;
            int avgFrameRate = (int)(1f / Time.unscaledDeltaTime);

            txt.text = avgFrameRate.ToString();
        }
    }
}
