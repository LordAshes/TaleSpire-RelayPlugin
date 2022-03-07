using BepInEx;
using BepInEx.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;


namespace LordAshes
{
    [BepInPlugin(Guid, Name, Version)]
    [BepInDependency(LordAshes.StatMessaging.Guid)]
    public partial class LightsOutPlugin : BaseUnityPlugin
    {
        // Plugin info
        public const string Name = "Replay Plug-In";
        public const string Guid = "org.lordashes.plugins.lightsout";
        public const string Version = "1.0.0.0";

        public List<System.Guid> subscriptionIds = new List<System.Guid>();
        public Dictionary<string, string> relays = new Dictionary<string, string>();

        public string prefix = "Lever";
        public int assetCount = 9;
        public string victoryState = "2";
        public string victorySet = "";

        /// <summary>
        /// Function for initializing plugin
        /// This function is called once by TaleSpire
        /// </summary>
        void Awake()
        {
            UnityEngine.Debug.Log("Lights Out Plugin: Active.");

            prefix = Config.Bind("Setting", "Asset Prefix", "Lever").Value;
            assetCount = Config.Bind("Setting", "Asset Count", 9).Value;
            victoryState = Config.Bind("Setting", "Victory State", "2").Value;
            victorySet = Config.Bind("Setting", "Victory Set", "org.lordashes.plugins.extraassetsregistration.Anim:Door01:2").Value;

            StatMessaging.Subscribe("org.lordashes.plugins.extraassetsregistration.Anim", GameStateChange);
        }

        private void GameStateChange(StatMessaging.Change[] obj)
        {
            int victory = 0;
            Debug.Log("Lights Out Plugin: Game State Change. Looking at " + prefix + "01 to " + prefix + assetCount.ToString("d2"));
            foreach (CreatureBoardAsset asset in CreaturePresenter.AllCreatureAssets)
            {
                Debug.Log("Lights Out Plugin: Found " + StatMessaging.GetCreatureName(asset)+" ("+asset.Creature.CreatureId+")");
                if (StatMessaging.GetCreatureName(asset).StartsWith(prefix))
                {
                    Debug.Log("Lights Out Plugin: Prefix Match");
                    int assetIndex = int.Parse(StatMessaging.GetCreatureName(asset).Substring(prefix.Length));
                    if (assetIndex >= 1 && assetIndex <= assetCount)
                    {
                        Debug.Log("Lights Out Plugin: Index In Range");
                        string state = StatMessaging.ReadInfo(asset.Creature.CreatureId, "org.lordashes.plugins.extraassetsregistration.Anim");
                        if (state == victoryState) { victory++; }
                        Debug.Log("Lights Out Plugin: " + prefix+assetIndex.ToString("d2") + " is in state "+state+". Looking for "+victoryState+". Success = "+(state == victoryState));
                    }
                }
            }
            Debug.Log("Lights Out Plugin: " + victory + " of " + assetCount + " set correctly");
            if (victory >= assetCount)
            {
                Debug.Log("Lights Out Plugin: Victory! Executing " + victorySet);
                foreach (CreatureBoardAsset asset in CreaturePresenter.AllCreatureAssets)
                {
                    if (StatMessaging.GetCreatureName(asset) == victorySet.Split(':')[1])
                    {
                        StatMessaging.SetInfo(asset.Creature.CreatureId, victorySet.Split(':')[0], victorySet.Split(':')[2]);
                    }
                }
            }
        }

        /// <summary>
        /// Function for determining if view mode has been toggled and, if so, activating or deactivating Character View mode.
        /// This function is called periodically by TaleSpire.
        /// </summary>
        void Update()
        {
        }
    }
}
