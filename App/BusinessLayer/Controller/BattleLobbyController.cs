﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MTCG.BusinessLayer.Interface;
using MTCG.BusinessLayer.Model.User;
using MTCG.DataAccessLayer;

namespace MTCG.BusinessLayer.Controller
{
    internal class BattleLobbyController
    {

        private static BattleLobbyController? _instance;

        public static BattleLobbyController Instance => _instance ??= new BattleLobbyController();

        private BattleLobbyController() { }

        private List<User> _waitingPlayers = [];
        private Dictionary<string, TaskCompletionSource<string>> _pendingResults = new Dictionary<string, TaskCompletionSource<string>>();


        public async Task<string> EnterLobby(User player)
        {
            if (_waitingPlayers.Count == 0)
            {
                // Player waits for opponent
                var tcs = new TaskCompletionSource<string>();
                _pendingResults[player.GetName()] = tcs;
                _waitingPlayers.Add(player);
                return await tcs.Task;
            }

            var opponent = _waitingPlayers[0];
            _waitingPlayers.RemoveAt(0);

            player.Deck.SetCards(await CardRepository.Instance.GetDeckByUser(player.Id) ?? []);
            opponent.Deck.SetCards(await CardRepository.Instance.GetDeckByUser(opponent.Id) ?? []);

            var opponentTcs = _pendingResults[opponent.GetName()];
            _pendingResults.Remove(opponent.GetName());


            var battleController = new BattleController(player, opponent);
            var result = await battleController.StartBattle();

            if (!result)
            {
                opponentTcs.SetResult("An Unexcpected Error Occured");
                return "An Unexcpected Error Occured";
            }

            opponentTcs.SetResult(battleController.GetSerializedBattleLogForPlayer(2));
            return battleController.GetSerializedBattleLogForPlayer(1);
            
        }

    }
}
