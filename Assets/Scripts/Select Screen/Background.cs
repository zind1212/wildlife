﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Jintori.SelectScreen
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class Background : MonoBehaviour
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        static readonly Color defaultColor = new Color32(0, 0, 0, 150);

        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        // --- Properties -------------------------------------------------------------------------------
        Image image;

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        public void Show(Color color)
        {
            if (image == null)
                image = GetComponent<Image>();

            gameObject.SetActive(true);
            image.color = color;
        }

        // -----------------------------------------------------------------------------------	
        public void Show()
        {
            Show(defaultColor);
        }

        // -----------------------------------------------------------------------------------	
        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}