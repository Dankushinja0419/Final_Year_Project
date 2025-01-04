using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Whiteboard : MonoBehaviour
{
    [SerializeField] private HiraganaChecker hiraganaChecker;
    public Texture2D texture;
    public Vector2 textureSize = new Vector2(2048, 8192);

    void Start()
    {
        var r = GetComponent<Renderer>();
        texture = new Texture2D((int)textureSize.x, (int)textureSize.y);

        // Set all pixels to white
        Color[] whitePixels = new Color[(int)(textureSize.x * textureSize.y)];
        for (int i = 0; i < whitePixels.Length; i++)
        {
            whitePixels[i] = Color.white;
        }

        texture.SetPixels(whitePixels);
        texture.Apply();

        r.material.mainTexture = texture;
    }

    public void ClearBoard()
    {
        // Set all pixels in the texture to white
        Color clearColor = Color.white;
        Color[] clearPixels = Enumerable.Repeat(clearColor, (int)(textureSize.x * textureSize.y)).ToArray();

        texture.SetPixels(clearPixels);
        texture.Apply();
        hiraganaChecker.read.text = "";
    }
}
