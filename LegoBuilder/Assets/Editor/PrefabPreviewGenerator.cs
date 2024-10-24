using UnityEngine;
using UnityEditor;
using System.IO;

namespace EditorTools
{
    public class PrefabPreviewGenerator : UnityEditor.EditorWindow
    {
        [UnityEditor.MenuItem("Tools/Generate Brick Prefab Previews")]
        public static void GeneratePrefabPreviews()
        {
            string previewFolder = "Assets/Resources/PrefabPreviews";
            if (!Directory.Exists(previewFolder))
            {
                Directory.CreateDirectory(previewFolder);
            }

            string[] prefabGuids = UnityEditor.AssetDatabase.FindAssets("t:Prefab");

            foreach (string guid in prefabGuids)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(path);

                if (prefab != null)
                {
                    Texture2D preview = UnityEditor.AssetPreview.GetAssetPreview(prefab);
                    if (preview != null)
                    {
                        byte[] bytes = preview.EncodeToPNG();
                        string previewPath = $"{previewFolder}/{prefab.name}_preview.png";
                        File.WriteAllBytes(previewPath, bytes);
                        UnityEditor.AssetDatabase.ImportAsset(previewPath);
                    }
                }
            }

            UnityEditor.AssetDatabase.Refresh();
        }
    }
}