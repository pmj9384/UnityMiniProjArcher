using UnityEngine;

public class ScytheOutlineEffect : MonoBehaviour
{
  public Material outlineMaterial; // 🔥 새 Outline Material (Inspector에서 할당)

  private Renderer scytheRenderer;
  private Material[] originalMaterials;

  private void Start()
  {
    scytheRenderer = GetComponent<Renderer>();

    if ( scytheRenderer != null && outlineMaterial != null )
    {
      // 기존 머티리얼 유지하면서 새로운 Outline 머티리얼 추가
      originalMaterials = scytheRenderer.materials;
      Material[] newMaterials = new Material[originalMaterials.Length + 1];

      for ( int i = 0; i < originalMaterials.Length; i++ )
      {
        newMaterials[i] = originalMaterials[i];
      }

      newMaterials[originalMaterials.Length] = outlineMaterial; // 🔥 추가된 Outline Material
      scytheRenderer.materials = newMaterials;
    }
  }

  private void OnDestroy()
  {
    if ( scytheRenderer != null )
    {
      scytheRenderer.materials = originalMaterials; // 오브젝트 제거 시 원래대로 복구
    }
  }
}
