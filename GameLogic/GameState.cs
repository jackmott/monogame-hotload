using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

using GameInterface;


namespace GameLogic
{
    
    public class GameLogic : IGameInterface
    {
        public GameState state;
        public void SetState(GameState state)
        {
            this.state = state;
        }

        private const float MOVE_SPEED = 0.1f;
        private const float JUMP_DURATION = 1000.0f;
        private const float JUMP_SPEED = 0.015f;

        public GameState Update(KeyboardState keyboard, GameTime gameTime)
        {
            var elapsed = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            
            if (state == null)
            {
                state = new GameState();
                state.PlayerPos = new Vector2(0.5f, 1.0f);
                state.jumpStart = 0;
            }
            
            if (keyboard.IsKeyDown(Keys.Left))
            {
                state.PlayerPos.X -= MOVE_SPEED / elapsed;
            }
            if (keyboard.IsKeyDown(Keys.Right))
            {
                state.PlayerPos.X += MOVE_SPEED / elapsed;
            }
            if (keyboard.IsKeyDown(Keys.Space))
            {
                if (state.jumpStart == 0.0f)
                {
                    state.jumpStart = (float)gameTime.TotalGameTime.TotalMilliseconds;
                }                
            }

            if (state.jumpStart != 0)
            {
                var jumpTime = ((float)gameTime.TotalGameTime.TotalMilliseconds - state.jumpStart)/1000.0f;
                jumpTime -= (JUMP_DURATION/1000.0f) / 2.0f;
                var jumpAmount = JUMP_SPEED * jumpTime;
                state.PlayerPos.Y += jumpAmount;

                if (state.PlayerPos.Y >= 1.0f)
                {
                    state.PlayerPos.Y = 1.0f;
                    state.jumpStart = 0.0f;
                }

            }
            return state;
        }
    }
}

