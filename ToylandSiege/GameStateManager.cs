using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ToylandSiege.GameState;

namespace ToylandSiege
{
    public class GameStateManager
    {
        private GameState.GameState _state;
        private GameState.GameState _previousState;

        public Dictionary<string, GameState.GameState> AvailableGameStates = new Dictionary<string, GameState.GameState>();

        public GameStateManager()
        {
            GameState.GameState._GameStateManager = this;
        }

        public void AddGameState(string name, GameState.GameState state)
        {
            AvailableGameStates.Add(name, state);
            if (_state == null)
                _state = state;
        }

        public GameState.GameState GetCurrentGameState()
        {
            return _state;
        }

        public GameState.GameState GetPreviousGameState()
        {
            return _previousState;
        }

        public void SetNewGameState(GameState.GameState state)
        {
            if (_state != state)
            {
                _previousState = _state;
                _state = state;
                _state.GameStateChanged();
            }
        }


        public void Update(GameTime gameTime)
        {
            _state.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            _state.Draw(gameTime);
        }
    }
}
