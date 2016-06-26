// SpaceFighter 
// Savan & J.B
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpaceFighter
{
    public partial class Form1 : Form
    {
        //vars
        int pX = 0; //player horizontal movement
        int playerMiddleX; //x-value of the middle of the player rect
        int pY = 0; //player vertical movement
        int playerMiddleY; //y-value of the middle of the player rect
        int[] lX; //horizontal movement of the laser
        int laserNum = 0; //keeps track of laser onscreen
        int maxHealth = 50; //max health
        int health; //health for the player
        int dmg = 2; //Laser damage
        int maxSpeed = 5; //Max speed
        int[] lvl1X; //basic enemies horizontal movement
        bool atStore = false; //keeps track whether the player is at the store or not
        int credits = 0; //keeps track of credits
        int[] upgradeLevel; //keeps track of how many upgrade are bought
        int[] upgradeCost; //keeps track of cost
        int boss1Health = 50; //health for the first boss
        //=================================//
        Rectangle player; //player rect.
        Rectangle[] playerLaser; //rect for player gun shots
        Rectangle[] lvl1Enemy; //rect for enemies
        Rectangle boss1; //rect for the first boss
        ProgressBar boss1Bar; //shows health of boss1
        Label boss1HealthDisplay; //displays boss1's health
        Button startBtn; //button to start the game
        Button storeBtn; //button to go to store
        Button[] upgrades; //used to upgrade stuff
        Button endGame; //allows players to instantly end the current game
        Label healthDisplay; //displays health
        Label creditsDisplay; //displays credits
        Label[] upgradeInfo; //displays info on upgrade
        Timer movementTmr; //timer for movement
        Timer shootTmr; //timer to auto shoot
        Timer spawnTmr; //spawns various enemies
        Image playerPic; //pic for player
        Image laserPic; //pic for the shots from the gun
        Image lvl1EnemyPic; //pic for the basic enemy
        Image boss1Pic; //pic for the first boss
        Random r; //random number generator

        public Form1()
        {
            //loading pictures
            playerPic = Image.FromFile(@"GalagaFighter.png", true);
            laserPic = Image.FromFile(@"Laser.jpg", true);
            lvl1EnemyPic = Image.FromFile(@"lvl1Enemy.png", true);
            boss1Pic = Image.FromFile(@"Yakuza5.png", true);

            //creating arrays
            playerLaser = new Rectangle[16];
            lX = new int[playerLaser.Length];
            lvl1Enemy = new Rectangle[5];
            lvl1X = new int[lvl1Enemy.Length];
            upgrades = new Button[4];
            upgradeInfo = new Label[upgrades.Length];
            upgradeLevel = new int[upgrades.Length];
            upgradeCost = new int[upgrades.Length];

            for (int i = 0; i < lX.Length; i++)
            {
                lX[i] = 12;
            }

            for (int i = 0; i < lvl1X.Length; i++)
            {
                lvl1X[i] = -5;
            }

            for (int i = 0; i < upgrades.Length; i++)
            {
                upgradeLevel[i] = 0;
                upgradeCost[i] = 1000;
            }

            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Form settings
            this.WindowState = FormWindowState.Maximized;
            this.MaximizeBox = false;
            this.Text = "Space Fighter";
            Cursor = Cursors.Cross;

            r = new Random();

            //Setting up the player
            player = new Rectangle();
            player.Size = new System.Drawing.Size(playerPic.Width, playerPic.Height);
            player.Location = new Point(5, -500);
            health = maxHealth;

            for (int i = 0; i < playerLaser.Length; i++)
            {
                playerLaser[i].Size = new Size(laserPic.Width, laserPic.Height);
                playerLaser[i].Location = new Point(-5, -500);
            }

            //Setting up the enemies
            for (int i = 0; i < lvl1Enemy.Length; i++)
            {
                lvl1Enemy[i].Size = new Size(lvl1EnemyPic.Width, lvl1EnemyPic.Height);
                lvl1Enemy[i].Location = new Point(5, -500);
            }

            boss1 = new Rectangle();
            boss1.Size = new System.Drawing.Size(boss1Pic.Width, boss1Pic.Height);
            boss1.Location = new Point(5, -500);
            boss1HealthDisplay = new Label();
            boss1HealthDisplay.Size = new System.Drawing.Size(boss1.Width, 5);
            boss1HealthDisplay.Location = new Point(5, -500);
            boss1Bar = new ProgressBar();
            boss1Bar.Size = boss1HealthDisplay.Size;
            boss1Bar.Location = new Point(0, 0);
            boss1Bar.Maximum = boss1Health;
            boss1Bar.Minimum = 0;
            boss1Bar.Value = boss1Health;
            boss1HealthDisplay.Controls.Add(boss1Bar);

            //Game window stuff
            healthDisplay = new Label();
            healthDisplay.Size = new System.Drawing.Size(100, 40);
            healthDisplay.Location = new Point(100, (this.ClientSize.Height - 50));

            creditsDisplay = new Label();
            creditsDisplay.Size = new System.Drawing.Size(100, 40);

            //Button stuff
            startBtn = new Button();
            startBtn.Font = new Font("Ar Essence", 12, FontStyle.Italic);
            startBtn.Text = "Start!";
            startBtn.Size = new Size(100, 30);
            startBtn.Location = new Point((this.ClientSize.Width / 2) - (startBtn.Width / 2), (this.Height / 3));
            startBtn.Click += startBtn_Click;

            storeBtn = new Button();
            storeBtn.Font = new Font("Ar Essence", 12, FontStyle.Italic);
            storeBtn.Text = "Store";
            storeBtn.Size = new Size(100, 30);
            storeBtn.Location = new Point((this.ClientSize.Width / 2) - (storeBtn.Width / 2), (startBtn.Bottom + 50));
            storeBtn.Click += storeBtn_Click;

            endGame = new Button();
            endGame.Font = new Font("Ar Essence", 12);
            endGame.Text = "Self destruct";
            endGame.Size = new Size(100, 30);
            endGame.Location = new Point((this.ClientSize.Width) - (this.ClientSize.Width / 4) - (endGame.Width / 2), this.ClientSize.Height - 50);
            endGame.Visible = true;
            endGame.Click += endGame_Click;

            for (int i = 0; i < upgrades.Length; i++)
            {
                upgrades[i] = new Button();
                upgrades[i].FlatStyle = FlatStyle.Popup;
                upgrades[i].BackColor = Color.Purple;
                upgrades[i].ForeColor = Color.Yellow;
                upgrades[i].Size = new Size(200, 60);
                upgrades[i].Location = new Point(200, (i * 100) + 100);
                upgrades[i].Visible = true;

                upgradeInfo[i] = new Label();
                upgradeInfo[i].ForeColor = Color.Black;
                upgradeInfo[i].TextAlign = ContentAlignment.MiddleCenter;
                upgradeInfo[i].Size = upgrades[i].Size;
                upgradeInfo[i].Location = new Point(upgrades[i].Right + 50, upgrades[i].Top);
                upgradeInfo[i].Visible = true;
            }
            upgrades[0].Text = "Health \nLv: " + upgradeLevel[0];
            upgrades[0].Click += healthUpgrade_Click;
            upgradeInfo[0].Text = "Increases health by 10 per level \nCost: " + upgradeCost[0];
            upgrades[1].Text = "Speed \nLv: " + upgradeLevel[1];
            upgrades[1].Click += speedUpgrade_Click;
            upgradeInfo[1].Text = "Increases max speed by 1 per level \nCost: " + upgradeCost[1];
            upgrades[2].Text = "Rate Of Fire \nLv: " + upgradeLevel[2];
            upgrades[2].Click += ROFUpgrade_Click;
            upgradeInfo[2].Text = "Decreases fire delay by 300ms per level \nCost: " + upgradeCost[2];
            upgrades[3].Text = "Damage \nLv: " + upgradeLevel[3];
            upgrades[3].Click += damageUpgrade_Click;
            upgradeInfo[3].Text = "Increases damage by 2 per level \nCost: " + upgradeCost[3];

            //Timers
            movementTmr = new Timer();
            movementTmr.Interval = 10;
            movementTmr.Tick += movementTmr_Tick;

            shootTmr = new Timer();
            shootTmr.Interval = 2000;
            shootTmr.Tick += shootTmr_Tick;

            spawnTmr = new Timer();
            spawnTmr.Interval = 1000;
            spawnTmr.Tick += spawnTmr_Tick;

            //Event for player movement
            this.MouseMove += Form1_MouseMove;

            //Form paint stuff
            this.DoubleBuffered = true;
            this.Paint += Form1_Paint;

            menu();
        }

        void endGame_Click(object sender, EventArgs e)
        {
            gameOver();
        }

        void damageUpgrade_Click(object sender, EventArgs e)
        {
            if (upgradeLevel[3] < 5 && credits >= upgradeCost[3])
            {
                dmg += 2;
                credits -= upgradeCost[3];
                creditsDisplay.Text = "Credits: " + credits;
                upgradeLevel[3]++;
                upgrades[3].Text = "Damage \nLv: " + upgradeLevel[3];
                upgradeCost[3] *= 2;
                upgradeInfo[3].Text = "Increases damage by 2 per level \nCost: " + upgradeCost[3];

                boss1Bar.Step = (-1 * dmg);
            }
        }

        void ROFUpgrade_Click(object sender, EventArgs e)
        {
            if (upgradeLevel[2] < 5 && credits >= upgradeCost[2])
            {
                shootTmr.Interval -= 300;
                credits -= upgradeCost[2];
                creditsDisplay.Text = "Credits: " + credits;
                upgradeLevel[2]++;
                upgrades[2].Text = "Rate Of Fire \nLv: " + upgradeLevel[2];
                upgradeCost[2] *= 2;
                upgradeInfo[2].Text = "Decreases fire delay by 300ms per level \nCost: " + upgradeCost[2];
            }
        }

        void speedUpgrade_Click(object sender, EventArgs e)
        {
            if (upgradeLevel[1] < 5 && credits >= upgradeCost[1])
            {
                maxSpeed++;
                credits -= upgradeCost[1];
                creditsDisplay.Text = "Credits: " + credits;
                upgradeLevel[1]++;
                upgrades[1].Text = "Speed \nLv: " + upgradeLevel[1];
                upgradeCost[1] *= 2;
                upgradeInfo[1].Text = "Increases max speed by 1 per level \nCost: " + upgradeCost[1];
            }
        }

        void healthUpgrade_Click(object sender, EventArgs e)
        {
            if (upgradeLevel[0] < 5 && credits >= upgradeCost[0])
            {
                maxHealth += 10;
                credits -= upgradeCost[0];
                creditsDisplay.Text = "Credits: " + credits;
                upgradeLevel[0]++;
                upgrades[0].Text = "Health \nLv: " + upgradeLevel[0];
                upgradeCost[0] *= 2;
                upgradeInfo[0].Text = "Increases health by 10 per level \nCost: " + upgradeCost[0];
            }
        }

        void storeBtn_Click(object sender, EventArgs e)
        {
            storeMenu();
        }

        void spawnTmr_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < lvl1Enemy.Length; i++)
            {
                if (r.Next(100) < 15)
                {
                    if (lvl1Enemy[i].Y < 0)
                    {
                        lvl1Enemy[i].Y = r.Next((this.ClientSize.Height - 60) - lvl1Enemy[i].Height);
                        lvl1Enemy[i].X = this.ClientSize.Width;
                    }
                }
            }

            if (r.Next(100) < 5 && boss1.Y < 0)
            {
                boss1.X = this.ClientSize.Width;
                boss1.Y = r.Next(this.ClientSize.Height - boss1.Height - 60);
                boss1HealthDisplay.Location = new Point(boss1.Left, boss1.Bottom);
            }
        }

        void shootTmr_Tick(object sender, EventArgs e)
        {
            if (playerLaser[laserNum].Y < 0 && laserNum < playerLaser.Length)
            {
                playerLaser[laserNum].X = player.Right;
                playerLaser[laserNum].Y = playerMiddleY - (playerLaser[0].Height / 2) - 25;
                laserNum++;
            }

            if (laserNum >= playerLaser.Length)
            {
                laserNum = 0;
            }
        }

        void movementTmr_Tick(object sender, EventArgs e)
        {
            playerAcceleration();

            playerMovement();

            boss1Movement();

            laserMovement();

            enemyMovement();

            intersection();
            creditsDisplay.Text = "Credits: " + credits;

            if (health <= 0)
            {
                gameOver();
            }

            this.Refresh();
        }

        void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (player.Y > -50 && !movementTmr.Enabled)
            {
                movementTmr.Start();
            }
        }

        void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(playerPic, player);

            for (int i = 0; i < playerLaser.Length; i++)
            {
                e.Graphics.DrawImage(laserPic, playerLaser[i]);
            }

            for (int i = 0; i < lvl1Enemy.Length; i++)
            {
                e.Graphics.DrawImage(lvl1EnemyPic, lvl1Enemy[i]);
            }

            e.Graphics.DrawImage(boss1Pic, boss1);
        }

        void startBtn_Click(object sender, EventArgs e)
        {
            //Getting rid of the main menu
            this.Controls.Remove(startBtn);
            this.Controls.Remove(storeBtn);
            this.Controls.Add(endGame);

            //Adding necessary hud stuff
            this.Controls.Add(healthDisplay);
            creditsDisplay.Location = new Point(300, (this.ClientSize.Height - 50));
            this.Controls.Add(creditsDisplay);
            this.Controls.Add(boss1HealthDisplay);

            health = maxHealth;
            healthDisplay.Text = "Health: " + health;
            creditsDisplay.Text = "Credits: " + credits;

            //Repositioning the player
            player.Location = new Point((this.ClientSize.Width / 2) - (player.Width / 2), this.ClientSize.Height - (this.ClientSize.Height / 3));
            playerMiddleX = player.X + (player.Width / 2);
            playerMiddleY = player.Y + (player.Height / 2);

            //Starts any necessary timers
            shootTmr.Start();
            spawnTmr.Start();
        }

        private void menu()
        { //creates the menu
            this.Refresh();
            this.Controls.Add(startBtn);
            this.Controls.Add(storeBtn);
        }

        private void playerMovement()
        {
            if (player.Left > 0 && pX < 0 || player.Right < this.ClientSize.Width && pX > 0)
            { //keeps player onscreen
                player.X += pX;
            }
            if (player.Top > 0 && pY < 0 || player.Bottom < (this.ClientSize.Height - 60) && pY > 0)
            {
                player.Y += pY;
            }
            playerMiddleX = player.Left + (player.Width / 2); //gets new middle of the player
            playerMiddleY = player.Top + (player.Height / 2) + 25; //+25 offsets the mouse a bit

            if (MousePosition.X < playerMiddleX && pX > 0)
            { //these stop the player after it passes the mouse
                pX = 0;
            }
            else if (MousePosition.X > playerMiddleX && pX < 0)
            {
                pX = 0;
            }

            if (MousePosition.Y < playerMiddleY && pY > 0)
            {
                pY = 0;
            }
            else if (MousePosition.Y > playerMiddleY && pY < 0)
            {
                pY = 0;
            }
        }

        private void boss1Movement()
        {
            if (boss1.Y >= 0)
            {
                boss1.X -= 12;

                if (boss1.Right <= 0)
                {
                    boss1.X = this.ClientSize.Width;
                    boss1.Y = r.Next(this.ClientSize.Height - boss1.Height - 60);
                }
            }
            boss1HealthDisplay.Location = new Point(boss1.Left, boss1.Bottom);
        }

        private void laserMovement()
        {
            for (int i = 0; i < playerLaser.Length; i++)
            {
                if (playerLaser[i].Y > 0)
                {
                    playerLaser[i].X += lX[i];

                    if (playerLaser[i].X > this.ClientSize.Width)
                    {
                        playerLaser[i].Location = new Point(5, -500);
                    }
                }
            }
        }

        private void enemyMovement()
        {
            for (int i = 0; i < lvl1Enemy.Length; i++)
            {
                if (lvl1Enemy[i].Y >= 0)
                {
                    lvl1Enemy[i].X += lvl1X[i];
                }

                if (lvl1Enemy[i].Right < 0)
                {
                    lvl1Enemy[i].Location = new Point(5, -500);
                }
            }
        }

        private void intersection()
        {
            //checking for intersection between player and enemy
            for (int i = 0; i < lvl1Enemy.Length; i++)
            {
                if (player.IntersectsWith(lvl1Enemy[i]))
                {
                    lvl1Enemy[i].Location = new Point(5, -500);

                    health -= 5;
                    healthDisplay.Text = "Health: " + health;
                }
            }

            if (player.IntersectsWith(boss1))
            {
                health -= 10;
                healthDisplay.Text = "Health: " + health;
                boss1.X = this.ClientSize.Width;
                boss1.Y = r.Next(this.ClientSize.Height - boss1.Height - 60);
            }

            //checking for intersection between laser and enemy
            for (int i = 0; i < playerLaser.Length; i++)
            {
                if (playerLaser[i].Y > 0 && playerLaser[i].IntersectsWith(lvl1Enemy[0]))
                {
                    lvl1Enemy[0].Location = new Point(5, -500);
                    playerLaser[i].Location = new Point(5, -500);
                    credits += 100;
                }
                else if (playerLaser[i].Y > 0 && playerLaser[i].IntersectsWith(lvl1Enemy[1]))
                {
                    lvl1Enemy[1].Location = new Point(5, -500);
                    playerLaser[i].Location = new Point(5, -500);
                    credits += 100;
                }
                else if (playerLaser[i].Y > 0 && playerLaser[i].IntersectsWith(lvl1Enemy[2]))
                {
                    lvl1Enemy[2].Location = new Point(5, -500);
                    playerLaser[i].Location = new Point(5, -500);
                    credits += 100;
                }
                else if (playerLaser[i].Y > 0 && playerLaser[i].IntersectsWith(lvl1Enemy[3]))
                {
                    lvl1Enemy[3].Location = new Point(5, -500);
                    playerLaser[i].Location = new Point(5, -500);
                    credits += 100;
                }
                else if (playerLaser[i].Y > 0 && playerLaser[i].IntersectsWith(lvl1Enemy[4]))
                {
                    lvl1Enemy[4].Location = new Point(5, -500);
                    playerLaser[i].Location = new Point(5, -500);
                    credits += 100;
                }
                else if (playerLaser[i].Y > 0 && playerLaser[i].IntersectsWith(boss1))
                {
                    playerLaser[i].Location = new Point(5, -500);
                    boss1Health -= dmg;
                    boss1Bar.PerformStep();

                    if (boss1Health <= 0)
                    {
                        credits += 5000;
                        boss1.Location = new Point(5, -500);
                        boss1HealthDisplay.Location = new Point(boss1.Left, boss1.Bottom);
                        boss1Health = 50;
                        boss1Bar.Value = boss1Health;
                    }
                }
            }
        }

        private void gameOver()
        {
            player.Location = new Point(5, -550);

            for (int i = 0; i < playerLaser.Length; i++)
            {
                playerLaser[i].Location = new Point(5, -500);
            }

            for (int i = 0; i < lvl1Enemy.Length; i++)
            {
                lvl1Enemy[i].Location = new Point(5, -500);
            }

            boss1.Location = new Point(5, -500);

            this.Controls.Remove(healthDisplay);
            this.Controls.Remove(creditsDisplay);
            this.Controls.Remove(endGame);
            this.Controls.Remove(boss1HealthDisplay);

            movementTmr.Stop();
            shootTmr.Stop();
            spawnTmr.Stop();
            menu();
        }

        private void playerAcceleration()
        {
            for (int i = 0; i < maxSpeed; i++)
            {
                if ((playerMiddleX) - MousePosition.X > Math.Pow(i + 1, 2.8))
                {
                    pX = -1 * i;
                }
                else if ((playerMiddleX) - MousePosition.X < (-1 * Math.Pow(i + 1, 2.8)))
                {
                    pX = i;
                }

                if ((playerMiddleY) - MousePosition.Y > Math.Pow(i + 1, 2.8))
                {
                    pY = -1 * i;
                }
                else if ((playerMiddleY) - MousePosition.Y < (-1 * Math.Pow(i + 1, 2.8)))
                {
                    pY = i;
                }
            }
        }

        private void storeMenu()
        {
            if (!atStore)
            {
                this.Controls.Remove(startBtn);
                storeBtn.Top = 500;
                storeBtn.Text = "Menu";

                creditsDisplay.Location = new Point((this.ClientSize.Width / 2) - (creditsDisplay.Width / 2), 50);
                this.Controls.Add(creditsDisplay);
                creditsDisplay.Text = "Credits: " + credits;

                //creates store
                for (int i = 0; i < upgrades.Length; i++)
                {
                    this.Controls.Add(upgrades[i]);
                    this.Controls.Add(upgradeInfo[i]);
                }

                atStore = true;
            }
            else
            {
                for (int i = 0; i < upgrades.Length; i++)
                {
                    this.Controls.Remove(upgrades[i]);
                    this.Controls.Remove(upgradeInfo[i]);
                }

                this.Controls.Remove(creditsDisplay);
                this.Controls.Add(startBtn);
                storeBtn.Top = (startBtn.Bottom + 50);
                storeBtn.Text = "Store";

                atStore = false;
            }
        }
    }
}