using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Reflection;
using System.Linq;

namespace Outline.Feature
{
    public class ObjectOutline : MonoBehaviour
    {
        [SerializeField] private Color selectionColor = Color.green;
        private ScriptableRendererData[] m_RendererDatas;
        private HighlightObjectSelection myHighlightObjectSelection;
        
        private void Awake()
        {
            myHighlightObjectSelection = GetComponent<HighlightObjectSelection>();
            myHighlightObjectSelection.OnHighlightedEvent.AddListener(OnObjectHighlighted);
            UpdateRendererDatas((UniversalRenderPipelineAsset)GraphicsSettings.currentRenderPipeline);
        }

        void UpdateRendererDatas(UniversalRenderPipelineAsset pipeline)
        {
            FieldInfo propertyInfo = pipeline.GetType().GetField( "m_RendererDataList", BindingFlags.Instance | BindingFlags.NonPublic );
            m_RendererDatas = ((ScriptableRendererData[])propertyInfo?.GetValue(pipeline));
        }

        private void OnObjectHighlighted(List<GameObject> highlightedObjects)
        {
            if (highlightedObjects == null)
            {
                ClearSelection();
                return;
            }
            ChangePalette();
            Outline(highlightedObjects);
        }
        
        private void ChangePalette()
        {
            var texture = new Texture2D(1, 1, TextureFormat.RGB24, false);
            texture.filterMode = FilterMode.Point;
            texture.SetPixel(0, 0, selectionColor);
            texture.Apply();
            
            if (m_RendererDatas == null)
                return;

            foreach (var rendererData in m_RendererDatas)
            {
                var multiSelectionFeature = rendererData.rendererFeatures.OfType<MultiSelectionOutlineFeature>().FirstOrDefault();
                if (multiSelectionFeature != null)
                {
                    multiSelectionFeature.ChangePalette(texture);
                }
            }
        }

        private void ClearSelection()
        {
            foreach (var rendererData in m_RendererDatas)
            {
                var multiSelectionFeature = rendererData.rendererFeatures.OfType<MultiSelectionOutlineFeature>().FirstOrDefault();
                if (multiSelectionFeature != null)
                {
                    multiSelectionFeature.datas.Clear();
                }
            }
        }
        
        private void Outline(List<GameObject> listOfHighlightedObjects)
        {
            if (m_RendererDatas == null)
            {
                return;
            }
                

            foreach (var rendererData in m_RendererDatas)
            {
                var multiSelectionFeature = rendererData.rendererFeatures.OfType<MultiSelectionOutlineFeature>().FirstOrDefault();
                if (multiSelectionFeature == null) continue;
                multiSelectionFeature.datas.Clear();

                foreach (var data in listOfHighlightedObjects)
                {
                    if (data == null) continue;
                    var renderers = new List<Renderer>();
                    var renderer = data.GetComponent<Renderer>();
                    if (renderer == null)
                    {
                        renderers = data.GetComponentsInChildren<Renderer>().ToList();
                    }
                    else
                    {
                        renderers.Add(renderer);
                    }
                    multiSelectionFeature.datas.Add(new MultiSelectionOutlineFeature.SelectionOutlineData()
                    {
                        colorId = 0,
                        renderers = renderers
                    });
                }
            }
        }

        private void OnDestroy()
        {
            myHighlightObjectSelection.OnHighlightedEvent.RemoveListener(OnObjectHighlighted);
        }
    }
}