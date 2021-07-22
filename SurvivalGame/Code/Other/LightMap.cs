using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    class LightMap
    {
        RenderTarget2D renderTarget2D;
        Light shadow = new Light(Vector2.Zero, new Vector2(10000, 10000), new Color(0, 0, 0), false, "Rectangle");
        AlphaTestEffect _alphaTestEffect;
        BlendState blendstate;
        int dayLengthInSeconds = 30;
        float darkness = 0;
        public LightMap()
        {

            renderTarget2D = new RenderTarget2D(
                Globals.graphics.GraphicsDevice,
                Globals.graphics.PreferredBackBufferWidth, Globals.graphics.PreferredBackBufferHeight);
            blendstate = new BlendState()
            {
                ColorSourceBlend = Blend.One,
                AlphaSourceBlend = Blend.One,

                ColorDestinationBlend = Blend.One,
                AlphaDestinationBlend = Blend.One
            };
            blendstate.AlphaBlendFunction = BlendFunction.ReverseSubtract;


        }
        public void Update(SpriteBatch spriteBatch)
        {
            darkness = 0.8f * (1 - (Math.Abs((Globals.worldTime / dayLengthInSeconds) % 1f - 0.5f) / 0.5f));
            Globals.graphics.GraphicsDevice.SetRenderTarget(renderTarget2D);
            Globals.graphics.GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(blendState: blendstate);
            //shadow.Draw(spriteBatch);
            foreach (var entity in Globals.getActiveRoom.Entities)
            {
                foreach (var light in entity.Lights)
                {
                    if (light.relativePosition)
                        light.Draw(spriteBatch, entity);
                    else
                        light.Draw(spriteBatch);
                }
            }
            spriteBatch.End();
            Globals.graphics.GraphicsDevice.SetRenderTarget(null);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(renderTarget2D, Vector2.Zero, null, Color.White * darkness, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.2f);
        }
    }
}
