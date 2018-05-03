using HughFor.Concrete;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using TiledSharp;

namespace HughFor
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class HughFor : Game
    {
        public GraphicsDeviceManager Graphics;
        public SpriteBatch SpriteBatch;
        public Viewport ViewportMain;
        public TilesetManager TilesetManager;
        public Multiverse Multiverse;
        public SpriteFont GameFont;
        
        public HughFor()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            TilesetManager = new TilesetManager(this);

            Graphics.PreferredBackBufferHeight = 720;
            Graphics.PreferredBackBufferWidth = 1280;
            Graphics.ApplyChanges();

            ViewportMain = GraphicsDevice.Viewport;

            GameFont = Content.Load<SpriteFont>("GameFont");

            LoadLevel("level1");
        }

        private void LoadLevel(string levelName)
        {
            TmxMap map = new TmxMap("Content/" + levelName + ".tmx");
            TilesetManager.LoadMap(map);
            Multiverse = new Multiverse(this, map);
            Multiverse.SetViewport(ViewportMain);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Multiverse.Update(dt);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            Multiverse.Draw();

            if (Multiverse.HasWon)
            {
                SpriteBatch.Begin();
                SpriteBatch.DrawString(GameFont, "VERY NICE. GREAT SUCCESS!",
                                       new Vector2(10, 10), Color.Black);
                SpriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}
