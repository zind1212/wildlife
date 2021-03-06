﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jintori
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class Transition : IllogicGate.SingletonBehaviour<Transition>
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        public const float TransitionTime = 0.45f;

        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        [SerializeField]
        GameObject loading;

        [SerializeField]
        UnityEngine.UI.Image maskImage;

        // --- Properties -------------------------------------------------------------------------------
        public float maskValue
        {
            get { return _maskValue; }
            set { SetMaskValue(value); }
        }
        float _maskValue;

        Material material { get { return maskImage.material; } }

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }

        // -----------------------------------------------------------------------------------	
        void SetMaskValue(float value)
        {
            _maskValue = Mathf.Clamp01(value);
            material.SetFloat("_MaskValue", _maskValue);
        }

        // -----------------------------------------------------------------------------------	
        public IEnumerator Show(bool displayLoading = true, bool playSound = true)
        {
            gameObject.SetActive(true);
            loading.SetActive(displayLoading);
            yield return StartCoroutine(Run(0, 1, playSound));
        }

        // -----------------------------------------------------------------------------------	
        public IEnumerator Hide(bool playSound = true)
        {
            gameObject.SetActive(true);
            loading.SetActive(false);
            yield return StartCoroutine(Run(1, 0, playSound));
            gameObject.SetActive(false);
        }

        // -----------------------------------------------------------------------------------	
        IEnumerator Run(float from, float to, bool playSound)
        {
            if (playSound)
                SoundManager.instance.PlaySFX("ui_transition");

            float elapsed = 0;
            while (elapsed < TransitionTime)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / TransitionTime);
                maskValue = Mathf.Lerp(from, to, t);
                yield return null;
            }
            maskValue = to;
        }
    }
}