﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jintori.Game
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class SkillRechargeItem : BonusItem
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------

        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        // --- Properties -------------------------------------------------------------------------------
        public override int maxPerGame { get { return 8; } }
        public override int maxPerRound { get { return 2; } }

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        public override float SpawnChance(float clearedRatio, int round, int totalRounds)
        {
            switch (Config.instance.difficulty)
            {
                case Config.Difficulty.Easy:
                    return 0.70f * clearedRatio;
                case Config.Difficulty.Normal:
                    return 0.65f * clearedRatio;
                case Config.Difficulty.Hard:
                    return 0.60f * clearedRatio;
            }
            return 0;
        }

        // -----------------------------------------------------------------------------------	
        protected override void Award()
        {
            Vector3 pos = playArea.MaskPositionToWorld(x, y);
            UI.instance.PlayBonusItemEffect(BonusEffect.Type.SkillRecharge, pos);
            Destroy(gameObject);
        }
    }
}