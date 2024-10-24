using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MTCG.BusinessLayer.Interface;
using MTCG.DataAccessLayer;

namespace MTCG
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
                return await tcs.Task; // Long-polling: waits for result
            }

            var opponent = _waitingPlayers[0];
            _waitingPlayers.RemoveAt(0);

            player.Deck.SetCards(await StackRepository.Instance.GetDeckByUser(player.Id) ?? []);
            opponent.Deck.SetCards(await StackRepository.Instance.GetDeckByUser(opponent.Id) ?? []);

            // Remove waiting opponent from pending results
            var opponentTcs = _pendingResults[opponent.GetName()];
            _pendingResults.Remove(opponent.GetName());


            // Start battle
            var battleController = new BattleController(player, opponent);
            battleController.StartBattle();
            var result = battleController.GetSerializedBattleLog();

            // Complete both tasks (for both players)
            opponentTcs.SetResult(result);
            return result; // Immediate result for player B
            
        }

    }
}
