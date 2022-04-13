using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Outline.Feature
{
    [RequireComponent(typeof(ObjectOutline))]
    public class HighlightObjectSelection : MonoBehaviour
    {
        private ObjectOutline objectOutline;

        private void Awake()
        {
            objectOutline = GetComponent<ObjectOutline>();
        }

    #if ENABLE_INPUT_SYSTEM
        public void OnSelect(InputValue value)
        {
            if (!value.isPressed)
            {
                return;
            }

            if (Mouse.current != null)
            {
                Vector2 mousePos = Mouse.current.position.ReadValue();
                if (Physics.Raycast(Camera.main.ScreenPointToRay(mousePos), out RaycastHit hit))
                {
                    List<GameObject> listOfHighlightedGameObject = new List<GameObject>();
                    
                    //Uncomment this if you want it to work with PiXYZ
                    /*if (hit.transform.TryGetComponent(out Pixyz.ImportSDK.Metadata pixyzMetaData))
                    {
                        if ((!listOfHighlightedGameObject.Contains(hit.transform.gameObject)))
                        {
                            listOfHighlightedGameObject.Add(hit.collider.gameObject);
                        }
                    }*/

                    //Uncomment this if you want it to work with Reflect
                    /*if (hit.transform.TryGetComponent(out UnityEngine.Reflect.Metadata reflectMetaData))
                    {
                        if ((!listOfHighlightedGameObject.Contains(hit.transform.gameObject)))
                        {
                            listOfHighlightedGameObject.Add(hit.collider.gameObject);
                        }
                    }*/
                    
                    //Uncomment this if you want it to work without the plugin above
                    /*if(!listOfHighlightedGameObject.Contains(hit.transform.gameObject))
                    {
                        listOfHighlightedGameObject.Add(hit.transform.gameObject);
                    }*/
                    
                    
                    if(listOfHighlightedGameObject.Count == 0) return;
                    objectOutline?.ChangePalette();
                    objectOutline?.Outline(listOfHighlightedGameObject);
                }
                else
                {
                    objectOutline?.ClearSelection();
                }
            }
        }
    #endif
    }
}

