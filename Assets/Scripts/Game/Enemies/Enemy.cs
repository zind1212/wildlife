﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jintori.Game
{
    /// <summary>
    /// Event raised when the minion dies
    /// </summary>
    /// <param name="enemy"> Enemy that was killed </param>
    /// <param name="wasKilledByPlayer"> True, if the player killed it. False if it died naturally </param>
    public delegate void EnemyKilledEvent(Enemy enemy, bool wasKilledByPlayer);

    // --- Class Declaration ------------------------------------------------------------------------
    [RequireComponent(typeof(Collider2D))]
    public abstract class Enemy : PlayAreaObject
    {
        // --- Events -----------------------------------------------------------------------------------
        /// <summary> Raised when the enemy dies </summary>
        public event EnemyKilledEvent killed = null;

        /// <summary> Raised when one of the minions spawned by this enemy dies </summary>
        public event EnemyKilledEvent minionKilled = null;

        // --- Constants --------------------------------------------------------------------------------

        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        [SerializeField]
        bool _isBoss = false;
        public bool isBoss { get { return _isBoss; } }

        // --- Properties -------------------------------------------------------------------------------
        /// <summary> True if the enemy is active on the play area </summary>
        protected bool isAlive;

        /// <summary> Collider for the enemy </summary>
        public new Collider2D collider
        {
            get
            {
                if (_collider == null)
                    _collider = GetComponent<Collider2D>();
                return _collider;
            }
        }
        private Collider2D _collider;

        /// <summary> List of minions that can be spawned by a boss</summary>
        List<Enemy> minions;

        /// <summary> List of active minions </summary>
        protected int minionCount { get { return minions.Count; } }

        /// <summary> Settings for the current difficulty level and round </summary>
        protected JSONObject settings { get; private set; }

        /// <summary> Score for killing this enemy, if available (0 otherwise)</summary>
        public int score { get { return settings.HasField("score") ? (int)settings["score"].i : 0; } }

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        /// <summary> 
        /// Does some basic initalization (changes per enemy type)
        /// </summary>
        protected virtual void Setup() { }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Main update routine. Steers the boss through the shadow
        /// (changes per enemy type)
        /// </summary>
        protected abstract void UpdatePosition();

        // -----------------------------------------------------------------------------------	
        /// <summary> 
        /// Do some ending for the enemy (animation, whatever)
        /// </summary>
        protected virtual void Finish() { }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Runs the enemy
        /// </summary>
        public void Run()
        {
            // get settings (if available)
            settings = Config.instance.GetEnemySettings(GetType().Name);
            if (settings != null)
                settings = settings[Controller.instance.round];


            gameObject.SetActive(true);
            minions = new List<Enemy>();
            StartCoroutine(RunCoroutine());
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Stops the enemy
        /// </summary>
        public void Kill(bool killedByPlayer)
        {
            if (killed != null)
                killed(this, killedByPlayer);

            isAlive = false;

            // kill all sub enemies
            // (use a copy, since kill tends to remove enemies from the sublist)
            foreach (Enemy enemy in minions.ToArray())
                enemy.Kill(killedByPlayer);
        }

        // -----------------------------------------------------------------------------------	
        IEnumerator RunCoroutine()
        {
            YieldInstruction wffu = new WaitForFixedUpdate();
            isAlive = true;
            playArea.mask.maskCleared += KillIfOutsideShadow;
            Setup();
            while (isAlive)
            {
                // don't move while frozen
                if (Skill.instance.isFreezeActive)
                {
                    animator.speed = 0;
                    while (Skill.instance.isFreezeActive)
                    {
                        CheckPlayerHit();
                        yield return null;
                    }
                    animator.speed = 1;
                }

                UpdatePosition();
                CheckPlayerHit();
                yield return wffu;
            }
            Finish();
            playArea.mask.maskCleared -= KillIfOutsideShadow;
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Returns a scale value based on the amount of mask remaining
        /// </summary>
        /// <returns></returns>
        protected float ScaleBasedOnMaskSize()
        {
            const float MinSize = 0.4f;
            const float MaxSize = 1.0f;
            const float MinRatio = 0.25f; // start getting small here
            const float MaxRatio = 0.75f; // stop getting small here

            float t = Mathf.Clamp(playArea.mask.clearedRatio, MinRatio, MaxRatio);
            t = (t - MinRatio) / (MaxRatio - MinRatio);
            return Mathf.Lerp(MaxSize, MinSize, t);
        }

        // -----------------------------------------------------------------------------------	
        void CheckPlayerHit()
        {
            Physics2D.Simulate(0.005f);
            if (collider.IsTouching(playArea.cutPath.collider))
                playArea.player.Hit(false);
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Checks if the enemy fell outside the shadow. This doesn't happen for
        /// bosses, but it does happen for non-boss enemies that get cut out.
        /// It "kills" the enemy, if that is the case
        /// </summary>
        void KillIfOutsideShadow()
        {
            if (playArea.mask[x, y] != PlayArea.Shadowed)
                Kill(true);
        }

        // -----------------------------------------------------------------------------------	
        public void AddMinion(Enemy enemy)
        {
            enemy.killed += OnMinionKilled;
            minions.Add(enemy);
        }

        // -----------------------------------------------------------------------------------	
        private void OnMinionKilled(Enemy enemy, bool killedByPlayer)
        {
            enemy.killed -= OnMinionKilled;
            minions.Remove(enemy);

            if (minionKilled != null)
                minionKilled(enemy, killedByPlayer);
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Sets the position of the boss at the start of the game
        /// </summary>
        /// <param name="playerInitalSquare">Needed to know where NOT to start</param>
        public void SetBossStartPosition(IntRect playerInitalSquare)
        {
            // object must be active for this to work...
            Debug.Assert(gameObject.activeInHierarchy);

            // object must be a boss
            Debug.Assert(isBoss);

            // create a random position within the play
            // area that 
            // a) is outside the initial square and
            // b) does not overlap borders or path
            Bounds bounds = collider.bounds;
            while (true)
            {
                Vector2 test = new Vector2();
                test.x = Random.Range(bounds.extents.x, PlayArea.imageWidth - bounds.extents.x);
                test.y = Random.Range(bounds.extents.y, PlayArea.imageHeight - bounds.extents.y);
                
                if (playerInitalSquare.Contains(test))
                    continue;

                x = (int)test.x;
                y = (int)test.y;

                Physics2D.Simulate(0.005f);
                if (collider.IsTouchingLayers(PlayArea.EdgesLayerMask))
                    continue;

                break;
            }
        }

        // -----------------------------------------------------------------------------------	
        RaycastHit2D[] hits = new RaycastHit2D[8];
        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Finds a random point on the shadow it can move to without being interrupted
        /// </summary>
        protected Vector2 FindValidTarget(Vector2 from, float bossRadius)
        {
            Vector2 target = Vector2.zero;
            int nHits = 1;
            int retries = 200;

            // search until a valid zone is found or it fails
            // TODO: when it fails, there is no recovery
            while (nHits > 0 && retries > 0)
            {
                // find a random position in the play area
                target = new Vector2()
                {
                    x = Random.Range(bossRadius, PlayArea.imageWidth - bossRadius),
                    y = Random.Range(bossRadius, PlayArea.imageHeight - bossRadius)
                };

                retries--;

                // target is not in the shadow (cannot move there)
                if (playArea.mask[(int)target.x, (int)target.y] != PlayArea.Shadowed)
                    continue;

                // cast a circle to see if it collides with something
                Vector2 direction = target - from;
                nHits = Physics2D.CircleCast(
                    transform.position, bossRadius, direction,
                    PlayArea.EdgeContactFilter, hits, direction.magnitude);
                //Debug.DrawRay(transform.position, direction, Color.red, 5);
            }

#if UNITY_EDITOR
            if (retries == 0)
            {
                print("failed");
                print(transform.position);
                print(string.Format("{0}, {1}", x, y));
                Debug.Break();
            }
#else
            ^^^ YOU SHOULD FIX THIS, YOU LAZY MOTHERFUCKER ^^^
#endif

            return target;
        }

    }
}