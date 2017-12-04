using UnityEngine;
using System.Collections.Generic;

public static class MaterialExtensions
{
    public static void AddResource(this ICollection<Material> materials, string resource)
    {
        Material material = Resources.Load(resource, typeof(Material)) as Material;
        if (material)
        {
            materials.Add(material);
        }
        else
            Debug.LogWarning("Material Resource '" + resource + "' could not be loaded.");
    }
}
