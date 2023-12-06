// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections.Generic;
using Fusion;

namespace com.meta.xr.colocation.fusion
{
    /// <summary>
    ///     A Photon Fusion concrete implementation of INetworkData
    ///     Used to manage a Player and Anchor list that all players that colocated are in
    /// </summary>
    public class FusionNetworkData : NetworkBehaviour, INetworkData
    {
        [Networked] private uint ColocationGroupCount { get; set; }
        [Networked, Capacity(10)] private NetworkLinkedList<FusionAnchor> AnchorList { get; }

        [Networked, Capacity(10)] private NetworkLinkedList<FusionPlayer> PlayerList { get; }

        public void AddPlayer(Player player)
        {
            AddFusionPlayer(new FusionPlayer(player));
        }

        public void RemovePlayer(Player player)
        {
            RemoveFusionPlayer(new FusionPlayer(player));
        }

        public Player? GetPlayerWithPlayerId(ulong playerId)
        {
            foreach (var fusionPlayer in PlayerList)
            {
                if (fusionPlayer.GetPlayer().playerId == playerId)
                {
                    return fusionPlayer.GetPlayer();
                }
            }

            return null;
        }

        public Player? GetPlayerWithOculusId(ulong oculusId)
        {
            foreach (var fusionPlayer in PlayerList)
            {
                if (fusionPlayer.GetPlayer().oculusId == oculusId)
                {
                    return fusionPlayer.GetPlayer();
                }
            }

            return null;
        }

        public List<Player> GetAllPlayers()
        {
            var allPlayers = new List<Player>();
            foreach (var fusionPlayer in PlayerList)
            {
                allPlayers.Add(fusionPlayer.GetPlayer());
            }

            return allPlayers;
        }

        public void AddAnchor(Anchor anchor)
        {
            AnchorList.Add(new FusionAnchor(anchor));
        }

        public void RemoveAnchor(Anchor anchor)
        {
            AnchorList.Remove(new FusionAnchor(anchor));
        }

        public Anchor? GetAnchor(ulong ownerOculusId)
        {
            foreach (var fusionAnchor in AnchorList)
            {
                if (fusionAnchor.GetAnchor().ownerOculusId == ownerOculusId)
                {
                    return fusionAnchor.GetAnchor();
                }
            }

            return null;
        }

        public List<Anchor> GetAllAnchors()
        {
            var anchors = new List<Anchor>();
            foreach (var fusionAnchor in AnchorList)
            {
                anchors.Add(fusionAnchor.GetAnchor());
            }

            return anchors;
        }

        public uint GetColocationGroupCount()
        {
            return ColocationGroupCount;
        }

        public void IncrementColocationGroupCount()
        {
            if (HasStateAuthority)
            {
                ColocationGroupCount++;
            }
            else
            {
                IncrementColocationGroupCountRpc();
            }
        }

        private void AddFusionPlayer(FusionPlayer player)
        {
            if (HasStateAuthority)
            {
                PlayerList.Add(player);
            }
            else
            {
                AddPlayerRpc(player);
            }
        }

        private void RemoveFusionPlayer(FusionPlayer player)
        {
            if (HasStateAuthority)
            {
                _ = PlayerList.Remove(player);
            }
            else
            {
                RemovePlayerRpc(player);
            }
        }

        private void AddFusionAnchor(FusionAnchor anchor)
        {
            if (HasStateAuthority)
            {
                AnchorList.Add(anchor);
            }
            else
            {
                AddAnchorRpc(anchor);
            }
        }

        private void RemoveFusionAnchor(FusionAnchor anchor)
        {
            if (HasStateAuthority)
            {
                _ = AnchorList.Remove(anchor);
            }
            else
            {
                RemoveAnchorRpc(anchor);
            }
        }

        #region Rpcs

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        private void AddPlayerRpc(FusionPlayer player)
        {
            AddFusionPlayer(player);
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        private void RemovePlayerRpc(FusionPlayer player)
        {
            RemoveFusionPlayer(player);
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        private void AddAnchorRpc(FusionAnchor anchor)
        {
            AddFusionAnchor(anchor);
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        private void RemoveAnchorRpc(FusionAnchor anchor)
        {
            RemoveFusionAnchor(anchor);
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        private void IncrementColocationGroupCountRpc()
        {
            IncrementColocationGroupCount();
        }

        #endregion
    }
}
