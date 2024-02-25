using UnityEngine;
using System.Collections;

public static class ColorModificationExtensions
{
    private static Vector3[][] TRANSFORMS = new Vector3[][] {
      new Vector3[] { new Vector3(1f,    0f,    0f),     new Vector3(0f,    1f,    0f),     new Vector3(0f,    0f,    1f)     },
      new Vector3[] { new Vector3(0.567f,0.433f,0f),     new Vector3(0.558f,0.442f,0f),     new Vector3(0f,    0.242f,0.758f) },
      new Vector3[] { new Vector3(0.817f,0.183f,0f),     new Vector3(0.333f,0.667f,0f),     new Vector3(0f,    0.125f,0.875f) },
      new Vector3[] { new Vector3(0.625f,0.375f,0f),     new Vector3(0.7f,  0.3f,  0f),     new Vector3(0f,    0.3f,  0.7f)   },
      new Vector3[] { new Vector3(0.8f,  0.2f,  0f),     new Vector3(0.258f,0.742f,0f),     new Vector3(0f,    0.142f,0.858f) },
      new Vector3[] { new Vector3(0.95f, 0.05f, 0f),     new Vector3(0f,    0.433f,0.567f), new Vector3(0f,    0.475f,0.525f) },
      new Vector3[] { new Vector3(0.967f,0.033f,0f),     new Vector3(0f,    0.733f,0.267f), new Vector3(0f,    0.183f,0.817f) },
      new Vector3[] { new Vector3(0.299f,0.587f,0.114f), new Vector3(0.299f,0.587f,0.114f), new Vector3(0.299f,0.587f,0.114f) },
      new Vector3[] { new Vector3(0.618f,0.320f,0.062f), new Vector3(0.163f,0.775f,0.062f), new Vector3(0.163f,0.320f,0.516f) },
    };
    public static Vector3 RedCoefficients(this ColorModification mode) { return TRANSFORMS[(int)mode][0]; }
    public static Vector3 BlueCoefficients(this ColorModification mode) { return TRANSFORMS[(int)mode][1]; }
    public static Vector3 GreenCoefficients(this ColorModification mode) { return TRANSFORMS[(int)mode][2]; }
}
public enum ColorModification
{
    Normal = 0,
    Protanopia = 1,
    Protanomaly = 2,
    Deuteranopia = 3,
    Deuteranomaly = 4,
    Tritanopia = 5,
    Tritanomaly = 6,
    Achromatopsia = 7,
    Achromatomaly = 8,
};

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class ColorBlindnessEffect : MonoBehaviour
{
    public ColorModification mode = ColorModification.Normal;
    public Shader colorAlterationShader;
    private Material mat = null;

    public void OnEnable() { }

    public IEnumerator OnPostRender()
    {
        if (colorAlterationShader == null) yield break;
        if (mat && colorAlterationShader != mat.shader)
        {
            Destroy(mat);
            mat = null;
        }

        // Wait until after everything else has happened, *including* rendering the
        // GUI!
        yield return new WaitForEndOfFrame();

        if (!mat)
        {
            mat = new Material(colorAlterationShader);
            mat.hideFlags = HideFlags.HideAndDontSave;
        }

        mat.SetVector("_RedTx", mode.RedCoefficients());
        mat.SetVector("_GreenTx", mode.BlueCoefficients());
        mat.SetVector("_BlueTx", mode.GreenCoefficients());

        Graphics.Blit(null, mat);
    }
}
