using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Hugh {
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D playerTexture;
        
        Vector2 playerSize;
        Vector2 playerPosition;
        Vector2 playerVelocity;
        
        public Game1() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize() {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            playerTexture = this.Content.Load<Texture2D>("idle_0");

            playerSize = new Vector2(playerTexture.Width, playerTexture.Height);
            playerPosition = new Vector2(0, 0);
            playerVelocity = new Vector2(100, 100);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent() {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            playerPosition.X += playerVelocity.X * dt;
            playerPosition.Y += playerVelocity.Y * dt;

            float viewWidth  = (float)GraphicsDevice.Viewport.Bounds.Width;
            float viewHeight = (float)GraphicsDevice.Viewport.Bounds.Height;

            if (playerPosition.X < 0 || playerPosition.X + playerSize.X > viewWidth) {
                playerVelocity.X = - playerVelocity.X;
            }

            if (playerPosition.Y < 0 || playerPosition.Y + playerSize.Y > viewHeight) {
                playerVelocity.Y = - playerVelocity.Y;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.LightGreen);

            spriteBatch.Begin();

            SpriteEffects effects = playerVelocity.X > 0
                ? SpriteEffects.None
                : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(playerTexture, playerPosition, null, Color.White,
                             0, Vector2.Zero, new Vector2(1, 1), effects, 1f);

            spriteBatch.End();
            
            base.Draw(gameTime);
        }
    }
}
