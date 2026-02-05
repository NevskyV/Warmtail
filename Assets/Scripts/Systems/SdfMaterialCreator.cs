using Entities.UI.SDF;
using TriInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Systems
{
    public class SdfMaterialCreator : MonoBehaviour
    {
        [SerializeField] private Image _mainImage;
        [Button]
        public void CreateMaterials()
        {
            var groups = GetComponentsInChildren<SdfGroup>();
            foreach (SdfGroup group in groups)
            {
                group.BaseMaterial = group.InstanceMaterial;
                group.InstanceMaterial = null;
                group.CreateMaterial(gameObject.name);
            }
            _mainImage.material = groups[0].InstanceMaterial;
            Debug.Log($"Created {groups.Length} materials");
        }
    }
}