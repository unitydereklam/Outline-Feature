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
        ScriptableRendererData[] m_RendererDatas;
        [SerializeField] private Color selectionColor = Color.green;
        
        protected void Awake()
        {
            UpdateRendererDatas((UniversalRenderPipelineAsset)GraphicsSettings.currentRenderPipeline);
        }
        
        void UpdateRendererDatas(UniversalRenderPipelineAsset pipeline)
        {
            FieldInfo propertyInfo = pipeline.GetType().GetField( "m_RendererDataList", BindingFlags.Instance | BindingFlags.NonPublic );
            m_RendererDatas = ((ScriptableRendererData[])propertyInfo?.GetValue(pipeline));
        }
        
        public void ChangePalette()
        {
            var texture = new Texture2D(1, 1, TextureFormat.RGB24, false);
            texture.filterMode = FilterMode.Point;
            texture.SetPixel(0, 0, selectionColor);
            texture.Apply();
            
            if (m_RendererDatas == null)
                return;

            foreach (var rendererData in m_RendererDatas)
            {
                var multiselectionFeature = rendererData.rendererFeatures.OfType<MultiSelectionOutlineFeature>().FirstOrDefault();
                if (multiselectionFeature != null)
                {
                    multiselectionFeature.ChangePalette(texture);
                }
            }
        }

        public void ClearSelection()
        {
            foreach (var rendererData in m_RendererDatas)
            {
                var multiselectionFeature = rendererData.rendererFeatures.OfType<MultiSelectionOutlineFeature>().FirstOrDefault();
                if (multiselectionFeature != null)
                {
                    multiselectionFeature.datas.Clear();
                }
            }
        }
        
        public void Outline(List<GameObject> listOfHighlightedObjects)
        {
            if (m_RendererDatas == null)
            {
                return;
            }
                

            foreach (var rendererData in m_RendererDatas)
            {
                var multiselectionFeature = rendererData.rendererFeatures.OfType<MultiSelectionOutlineFeature>().FirstOrDefault();
                if (multiselectionFeature != null)
                {
                    multiselectionFeature.datas.Clear();

                    foreach (var data in listOfHighlightedObjects)
                    {
                        if (data != null)
                        {
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
                            multiselectionFeature.datas.Add(new MultiSelectionOutlineFeature.SelectionOutlineData()
                            {
                                colorId = 0,
                                renderers = renderers
                            });
                        }
                    }
                }
            }
        }
    }
}