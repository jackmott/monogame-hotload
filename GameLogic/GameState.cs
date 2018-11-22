using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace GameLogic
{
 

    public class GameState
    {
        public GraphicsDevice device;
        public Texture2D playerTex;
        public Dictionary<string, Effect> shaders;
        public Vector2 PlayerPos;
        public float jumpStart;

        public GameState()
        {
            shaders = new Dictionary<string, Effect>();
        }
    }

    public class GameLogic 
    {
        public GameState state;

        public GameLogic()
        {
            state = new GameState();            
            state.PlayerPos = new Vector2(0.5f, 1.0f);
            state.jumpStart = 0;
        }

        
        public GameState GetState()
        {
            return state;
        }

        private const float MOVE_SPEED = 0.25f;
        private const float JUMP_DURATION = 1000.0f;
        private const float JUMP_SPEED = 0.02f;

        
        public void Update(KeyboardState keyboard, GameTime gameTime)
        {
            var elapsed = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

           
            if (keyboard.IsKeyDown(Keys.Left))
            {
                state.PlayerPos.X -= MOVE_SPEED / elapsed;
            }
            else if (keyboard.IsKeyDown(Keys.Right))
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
                var jumpTime = ((float)gameTime.TotalGameTime.TotalMilliseconds - state.jumpStart) / 1000.0f;
                jumpTime -= (JUMP_DURATION / 1000.0f) / 2.0f;
                var jumpAmount = JUMP_SPEED * jumpTime;
                state.PlayerPos.Y += jumpAmount;

                if (state.PlayerPos.Y >= 1.0f)
                {
                    state.PlayerPos.Y = 1.0f;
                    state.jumpStart = 0.0f;
                }

            }            
        }

        public void Draw(SpriteBatch batch,  GameTime gameTime)
        {
            
            state.device.Clear(Color.Black);
            batch.Begin(effect: state.shaders["effect"]);
            batch.Draw(state.playerTex, new Vector2(state.PlayerPos.X * state.device.Viewport.Width, state.PlayerPos.Y * state.device.Viewport.Height - 100.0f), Color.White);
            batch.End();            
            
        }
    }
}

