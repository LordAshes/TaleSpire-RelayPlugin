using BepInEx;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LordAshes
{
    public partial class ReplayPlugin : BaseUnityPlugin
    {
        /// <summary>
        /// Handler for Stat Messaging subscribed messages.
        /// </summary>
        /// <param name="changes"></param>
        public void RelayHandler(AssetDataPlugin.DatumChange change)
        {
            Debug.Log("Relay Plugin: Received " + change.source + "->" + change.key + "->" + change.previous+"->"+change.value+" ("+change.action+")");
            
            CreatureBoardAsset asset;
            CreaturePresenter.TryGetAsset(new CreatureGuid(change.source), out asset);
            if (asset != null)
            {
                Debug.Log("Relay Plugin: Translated to " + change.key + "->" + AssetDataPlugin.Legacy.GetCreatureName(asset.Creature) + "->" + change.value);
                foreach (KeyValuePair<string, string> relay in relays)
                {
                    Debug.Log("Relay Plugin: Seeking Action. Creature Change: '" + change.key + ":" + AssetDataPlugin.Legacy.GetCreatureName(asset.Creature) + ":" + change.value + "' vs Relay: '" + relay.Key + "'");
                    if ((change.key + ":" + AssetDataPlugin.Legacy.GetCreatureName(asset.Creature)) + ":" + change.value == relay.Key)
                    {
                        Debug.Log("Relay Plugin: Found Relay. Actions: '" + relay.Value + "'");
                        string[] actions = relay.Value.Split(':');
                        while (actions.Length > 0)
                        {
                            float delay = float.Parse(actions[0]);
                            string[] parts = new string[] { actions[1], actions[2], actions[3] };
                            actions = actions.Skip(4).ToArray();
                            Debug.Log("Relay Plugin: Key = " + parts[0]);
                            Debug.Log("Relay Plugin: Creature = " + parts[1]);
                            Debug.Log("Relay Plugin: State = " + parts[2]);

                            CreatureBoardAsset target = FindCreature(parts[1]);
                            if(target!=null)
                            {
                                Debug.Log("Relay Plugin: Delaying Request " + target.Creature.CreatureId + "->" + parts[1] + "->" + parts[2]);
                                if (parts[2].StartsWith("~"))
                                {
                                    string strState = AssetDataPlugin.ReadInfo(target.Creature.CreatureId.ToString(), parts[0]);
                                    int state = 1;
                                    if (!int.TryParse(strState, out state)) { state = 1; }
                                    Debug.Log("Relay Plugin: Toggle State From " + strState);
                                    state++;
                                    if (state > int.Parse(parts[2].Substring(1))) { state = 1; }
                                    parts[2] = state.ToString();
                                    Debug.Log("Relay Plugin: Toggle State To " + parts[2]);
                                }
                                if (processRelays) { this.StartCoroutine("DelayRequest", new object[] { delay, target.Creature.CreatureId.ToString(), parts[0], parts[2] }); }
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (KeyValuePair<string, string> relay in relays)
                {
                    Debug.Log("Relay Plugin: Seeking Action. Asset Change: '" + change.key + ":" + change.source + ":" + change.value + "' vs Relay: '" + relay.Key + "'");
                    if ((change.key + ":" + change.source + ":" + change.value) == relay.Key)
                    {
                        Debug.Log("Relay Plugin: Found Relay. Actions: '" + relay.Value + "'");
                        string[] actions = relay.Value.Split(':');
                        while (actions.Length > 0)
                        {
                            float delay = float.Parse(actions[0]);
                            string[] parts = new string[] { actions[1], actions[2], actions[3] };
                            actions = actions.Skip(4).ToArray();
                            Debug.Log("Relay Plugin: Key = " + parts[0]);
                            Debug.Log("Relay Plugin: Asset = " + parts[1]);
                            Debug.Log("Relay Plugin: State = " + parts[2]);

                            Debug.Log("Relay Plugin: Delaying Request " + parts[1] + "->" + parts[0] + "->" + parts[2]);
                            if (parts[2].StartsWith("~"))
                            {
                                string strState = AssetDataPlugin.ReadInfo(change.source, parts[0]);
                                int state = 1;
                                if (!int.TryParse(strState, out state)) { state = 1; }
                                Debug.Log("Relay Plugin: Toggle State From " + strState);
                                state++;
                                if (state > int.Parse(parts[2].Substring(1))) { state = 1; }
                                parts[2] = state.ToString();
                                Debug.Log("Relay Plugin: Toggle State To " + parts[2]);
                            }
                            if (processRelays) { this.StartCoroutine("DelayRequest", new object[] { delay, parts[1], parts[0], parts[2] }); }
                        }
                    }
                }
            }
        }

        public CreatureBoardAsset FindCreature(string creatureName)
        {
            foreach (CreatureBoardAsset asset in CreaturePresenter.AllCreatureAssets)
            {
                if (AssetDataPlugin.Legacy.GetCreatureName(asset).ToUpper() == creatureName.ToUpper()) { return asset; }
            }
            return null;
        }

        public IEnumerator DelayRequest(object[] inputs)
        {
            yield return new WaitForSeconds((float)inputs[0]);
            Debug.Log("Relay Plugin: Processing Request " + inputs[1] + "->" + inputs[2] + "->" + inputs[3]);
            bool legacy = inputs[2].ToString().StartsWith("*");
            if (legacy) { inputs[2] = inputs[2].ToString().Substring(1); }
            if (((string)inputs[3]).Trim() != "")
            {
                AssetDataPlugin.SetInfo((string)inputs[1], (string)inputs[2], (string)inputs[3], legacy);
            }
            else
            {
                AssetDataPlugin.ClearInfo((string)inputs[1], (string)inputs[2], legacy);
            }
            this.StartCoroutine("SuspendRelays");
        }

        public IEnumerator SuspendRelays()
        {
            processRelays = false;
            Debug.Log("Relay Plugin: Relays Suspended");
            yield return new WaitForSeconds(suspensionInterval);
            Debug.Log("Relay Plugin: Relays Activated");
            processRelays = true;
        }        
    }
}