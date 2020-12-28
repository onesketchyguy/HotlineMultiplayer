using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace TopDownShooter
{
    [RequireComponent(typeof(Tilemap))]
    [RequireComponent(typeof(CompositeShadowCaster2D))]
    public class TilemapShadowCaster : MonoBehaviour
    {
        public bool useRendererSilhouette = true;
        public bool selfShadows = false;
        public bool castsShadows = true;

        public bool useComposite = true;

        public Tilemap tilemap;
        public ShadowCaster2D[] shadowCasters;
        private CompositeShadowCaster2D compositeShadowCaster2D;

        public Transform shadowParentObject;

        private int GetTileLength()
        {
            int i = 0;

            foreach (var position in tilemap.cellBounds.allPositionsWithin)
            {
                if (!tilemap.HasTile(position)) continue;
                i++;
            }

            return i;
        }

        private void OnValidate()
        {
            // Get the tilemap
            if (tilemap == null) tilemap = GetComponent<Tilemap>();

            // Lets assume that we found that component since it's required

            // Setup shadow objects
            if (shadowCasters == null || shadowCasters.Length != GetTileLength()) SetupShadows();

            // Configure shadow objects
            for (int i = 0; i < shadowCasters.Length; i++)
            {
                var caster = shadowCasters[i];

                caster.useRendererSilhouette = useRendererSilhouette;
                caster.selfShadows = selfShadows;
                caster.castsShadows = castsShadows;
            }

            if (compositeShadowCaster2D == null)
                compositeShadowCaster2D = GetComponent<CompositeShadowCaster2D>();

            compositeShadowCaster2D.enabled = useComposite;
        }

        internal void SetupShadows()
        {
            var shadowList = new List<ShadowCaster2D>();

            if (shadowParentObject == null)
            {
                shadowParentObject = new GameObject("Shadows").transform;
                shadowParentObject.transform.SetParent(transform);
                shadowParentObject.transform.position = transform.position;
            }
            else
            {
                // Destroy all the children
                for (int i = shadowParentObject.childCount - 1; i >= 0; i--)
                    DestroyImmediate(shadowParentObject.GetChild(i).gameObject);
            }

            foreach (var position in tilemap.cellBounds.allPositionsWithin)
            {
                if (!tilemap.HasTile(position)) continue;

                // Tile is not empty; do stuff
                var shadow = new GameObject($"shadow_{position}");
                shadow.transform.SetParent(shadowParentObject.transform);
                shadow.transform.position = position + tilemap.tileAnchor;
                shadow.transform.localScale = tilemap.cellSize;

                var caster = shadow.AddComponent<ShadowCaster2D>();

                caster.useRendererSilhouette = useRendererSilhouette;
                caster.selfShadows = selfShadows;
                caster.castsShadows = castsShadows;

                shadowList.Add(caster);
            }

            shadowCasters = shadowList.ToArray();
        }
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(TilemapShadowCaster))]
    public class TilemapShadowCasterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var t = (TilemapShadowCaster)target;

            if (GUILayout.Button("Recalculate lighting"))
                t.SetupShadows();
        }
    }

#endif
}