﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Jintori;
using Jintori.SelectScreen;

// --- Class Declaration ------------------------------------------------------------------------
public class Test : MonoBehaviour 
{
    // --- Events -----------------------------------------------------------------------------------
    // --- Constants --------------------------------------------------------------------------------
    // --- Static Properties ------------------------------------------------------------------------
    // --- Static Methods ---------------------------------------------------------------------------
    // -----------------------------------------------------------------------------------
    // --- Inspector --------------------------------------------------------------------------------

    // --- Properties -------------------------------------------------------------------------------
    [SerializeField]
    CharacterGrid test;

    // --- MonoBehaviour ----------------------------------------------------------------------------
    // -----------------------------------------------------------------------------------	

	// --- Methods ----------------------------------------------------------------------------------
	// -----------------------------------------------------------------------------------	
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            for (int i = 0; i < 30; i++)
                test.AddCharacter();
            test.Paginate();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Overlay.instance.messagePopup.Show("HELLO WORLD", "POPUP TITLE");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Overlay.instance.skillSelectPopup.Show(Jintori.Game.Skill.Type.Freeze);
        }
    }
}
