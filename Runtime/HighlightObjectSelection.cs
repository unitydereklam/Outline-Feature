using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
#endif

namespace Outline.Feature
{
    [RequireComponent(typeof(ObjectOutline))]
    public class HighlightObjectSelection : MonoBehaviour
    {
        public UnityEvent<List<GameObject>> OnHighlightedEvent;

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

                if (EventSystem.current != null)
                {
                    var eventDataCurrentPosition = new PointerEventData(EventSystem.current)
                    {
                        position = mousePos
                    };
                    
                    List<RaycastResult> result = new List<RaycastResult>();
                    EventSystem.current.RaycastAll(eventDataCurrentPosition, result);

                    if (result.Count > 0)
                    {
                        return;
                    }
                }


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
                    OnHighlightedEvent?.Invoke(listOfHighlightedGameObject);
                }
                else
                {
                    OnHighlightedEvent?.Invoke(null);
                }
            }
        }
    #endif
    }
}

