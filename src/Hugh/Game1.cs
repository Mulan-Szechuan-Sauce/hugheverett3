using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Hugh {
    class Controller {
        private static bool IsKeyDown(Keys key) {
            return Keyboard.GetState().IsKeyDown(key);
        }

        public static bool isLeftPressed() {
            return IsKeyDown(Keys.Left) || IsKeyDown(Keys.A);
        }

        public static bool isRightPressed() {
            return IsKeyDown(Keys.Right) || IsKeyDown(Keys.E);
        }

        public static bool isUpPressed() {
            return IsKeyDown(Keys.Up) || IsKeyDown(Keys.OemComma);
        }

        public static bool isDownPressed() {
            return IsKeyDown(Keys.Down) || IsKeyDown(Keys.O);
        }
    }

    class Player {
        Game1 game;

        Texture2D texture;

        Vector2 position;
        Vector2 size;
        Vector2 velocity;

        public Player(Game1 game, Vector2 position) {
            this.game = game;
            this.position = position;
            this.velocity = new Vector2(100, 100);
        }

        public void Update(float dt) {
            position.X += velocity.X * dt;
            position.Y += velocity.Y * dt;

            float viewWidth  = game.GraphicsDevice.Viewport.Bounds.Width;
            float viewHeight = game.GraphicsDevice.Viewport.Bounds.Height;

            if (position.X < 0 || position.X + size.X > viewWidth) {
                velocity.X = - velocity.X;
            }

            if (position.Y < 0 || position.Y + size.Y > viewHeight) {
                velocity.Y = - velocity.Y;
            }

            if (Controller.isRightPressed()) {
                velocity.X += 100 * dt;
            }
            if (Controller.isLeftPressed()) {
                velocity.X -= 100 * dt;
            }
            if (Controller.isUpPressed()) {
                velocity.Y -= 100 * dt;
            }
            if (Controller.isDownPressed()) {
                velocity.Y += 100 * dt;
            }
        }

        public void Draw() {
            game.spriteBatch.Begin();
            
            SpriteEffects effects = velocity.X > 0
                ? SpriteEffects.None
                : SpriteEffects.FlipHorizontally;

            game.spriteBatch.Draw(texture, position, null, Color.White,
                                  0, Vector2.Zero, new Vector2(1, 1), effects, 1f);

            game.spriteBatch.End();
        }

        public void LoadContent() {
            texture = game.Content.Load<Texture2D>("idle_0");
            size = new Vector2(texture.Width, texture.Height);
        }
    }

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game {
        GraphicsDeviceManager graphics;

        public SpriteBatch spriteBatch;

        Player player;
        
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
            player = new Player(this, new Vector2(0, 0));
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            player.LoadContent();
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
            player.Update(dt);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.LightGreen);
            player.Draw();
            base.Draw(gameTime);
        }
    }
}
