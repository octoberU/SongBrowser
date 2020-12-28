using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace AudicaModding
{
    internal static class ButtonUtils
    {
        public enum ButtonLocation
        {
            Menu,
            Failed,
            Results
        }

        public static void InitButton(GameObject button, string label, Action listener, Vector3 localPosition, 
                                      Vector3 rotation)
        {
            GameObject.Destroy(button.GetComponentInChildren<Localizer>());

            TextMeshPro buttonText = button.GetComponentInChildren<TextMeshPro>();
            buttonText.text        = label;

            GunButton gb       = button.GetComponentInChildren<GunButton>();
            gb.destroyOnShot   = false;
            gb.doMeshExplosion = false;
            gb.doParticles     = false;
            gb.onHitEvent      = new UnityEvent();
            gb.onHitEvent.AddListener(listener);

            button.transform.localPosition = localPosition;
            button.transform.Rotate(rotation);
        }
    }
}
