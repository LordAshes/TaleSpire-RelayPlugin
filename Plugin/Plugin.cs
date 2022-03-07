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
    [BepInDependency(LordAshes.FileAccessPlugin.Guid)]
    [BepInDependency(LordAshes.AssetDataPlugin.Guid)]
    public partial class ReplayPlugin : BaseUnityPlugin
    {
        // Plugin info
        public const string Name = "Replay Plug-In";             
        public const string Guid = "org.lordashes.plugins.relay";
        public const string Version = "1.2.0.0";

        public List<System.Guid> subscriptionIds = new List<System.Guid>();
        public Dictionary<string, string> relays = new Dictionary<string, string>();

        public KeyboardShortcut toggle;
        public float suspensionInterval = 2.0f;
        public bool processRelays = true;

        /// <summary>
        /// Function for initializing plugin
        /// This function is called once by TaleSpire
        /// </summary>
        void Awake()
        {
            UnityEngine.Debug.Log("Relay Plugin: Active.");

            toggle = Config.Bind("Setting", "Toggle Logic", new KeyboardShortcut(KeyCode.X, KeyCode.RightControl)).Value;

            suspensionInterval = Config.Bind("Setting", "Relay Suspension Interval", 2.0f).Value;

            List<string> unique = new List<string>();
            foreach (string subscriptions in FileAccessPlugin.File.Catalog().Where(f => System.IO.Path.GetExtension(f).ToUpper() == ".SUB").ToArray())
            {
                Debug.Log("Relay Plugin: Reading Relays From '"+ subscriptions + "'...");

                foreach (string subscription in FileAccessPlugin.File.ReadAllLines(subscriptions.ToString()))
                {
                    if (subscription.Trim() != "")
                    {
                        string[] parts = subscription.Split('|')[0].Split(':');
                        Debug.Log("Relay Plugin: Adding Relay " + parts[0] + " for " + parts[1] + " state " + parts[2]);
                        if (!unique.Contains(parts[0]))
                        {
                            Debug.Log("Relay Plugin: Adding subscription to " + parts[0]);
                            subscriptionIds.Add(AssetDataPlugin.Subscribe(parts[0], RelayHandler));
                            unique.Add(parts[0]);
                        }
                        relays.Add(parts[0] + ":" + parts[1] + ":" + parts[2], subscription.Split('|')[1]);
                    }
                }
            }

            Utility.PostOnMainPage(this.GetType());
        }

        /// <summary>
        /// Function for determining if view mode has been toggled and, if so, activating or deactivating Character View mode.
        /// This function is called periodically by TaleSpire.
        /// </summary>
        void Update()
        {
            if(Utility.StrictKeyCheck(toggle))
            {
                processRelays = !processRelays;
                SystemMessage.DisplayInfoText("Processing of Relays Logic is " + (processRelays ? "On" : "Off"));
            }
        }
    }
}
