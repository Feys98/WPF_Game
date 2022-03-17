using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
namespace WpfGame
{
    public partial class MainWindow : Window
    {
        DispatcherTimer secondsTimer_Debug = new DispatcherTimer();
        DispatcherTimer animationTimer = new DispatcherTimer();
        int framesCounter_Debug = 0;
        int counter_Debug = 0;
        DispatcherTimer gameFramerate = new DispatcherTimer();

        Rect playerHitBox;
        Rect playerFootHitBox;
        Rect platformHitBox;
        Rect obstacleHitBox;
        Rect pointHitBox;


        bool playerJumping = false, playerLeft = false, playerRight = false, playerIsOnTheGround = false, playerCanJump = false, playerFlying = false;

        int score = 0;

        const int gForce = 16;
        bool playerDirectionIsLeft = false;
        int force = gForce;
        int speed = 5;
        int flying = 21;

        int playerJumpStrengthCounter = 0;
        bool playerJumpStrengthSwitch = false;

        List<double> platformsStartingLocationTop = new List<double>();
        List<double> platformsStartingLocationLeft = new List<double>();
        List<double> obstaclesStartingLocationTop = new List<double>();
        List<double> obstaclesStartingLocationLeft = new List<double>();
        List<double> pointsStartingLocationLeft = new List<double>();
        List<double> pointsStartingLocationTop = new List<double>();


        Random rng = new Random();

        bool gameIsOver = false;

        ImageBrush backgrounSprite = new ImageBrush();
        ImageBrush playerAnimationLeft1 = new ImageBrush();
        ImageBrush playerAnimationLeft2 = new ImageBrush();
        ImageBrush playerAnimationRight1 = new ImageBrush();
        ImageBrush playerAnimationRight2 = new ImageBrush();
        ImageBrush playerAnimationJump1 = new ImageBrush();
        ImageBrush playerAnimationJump2 = new ImageBrush();
        ImageBrush obstacle1 = new ImageBrush();
        ImageBrush brick = new ImageBrush();
        bool playerAnimation = false;

        int platformPositionTop;
        int platformPositionLeft;
        int previousPlatformPosition = 0;


        private void keyIsDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && gameIsOver == true)
            {
                StartGame();
            }
            if (e.Key == Key.A && gameIsOver == false)
            {
                playerLeft = true;

                playerDirectionIsLeft = true;

                animationTimer.Start();
            }
            if (e.Key == Key.D && gameIsOver == false)
            {
                playerRight = true;

                playerDirectionIsLeft = false;

                animationTimer.Start();
            }
            if ((e.Key == Key.W || e.Key == Key.Space) && gameIsOver == false && playerJumping == false && playerIsOnTheGround == true && playerCanJump == true)
            {
                playerJumping = true;

                playerJumpStrengthSwitch = true;

            }
            if (e.Key == Key.LeftShift && gameIsOver == false)
            {
                speed = 10;
            }

        }

        private void keyIsUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.A && gameIsOver == false)
            {
                playerLeft = false;

                animationTimer.Stop();
                player.Fill = playerAnimationLeft1;
            }
            if (e.Key == Key.D && gameIsOver == false)
            {
                playerRight = false;

                animationTimer.Stop();
                player.Fill = playerAnimationRight1;
            }
            if (e.Key == Key.LeftShift && gameIsOver == false)
            {
                speed = 5;
            }
            if ((e.Key == Key.W || e.Key == Key.Space))
            {
                playerJumpStrengthSwitch = false;
                playerJumpStrengthCounter = 1;
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            myCanvas.Focus();

            gameFramerate.Tick += GameEngine;
            gameFramerate.Interval = TimeSpan.FromMilliseconds(15);

            secondsTimer_Debug.Tick += seconds_Debug_Tick;
            secondsTimer_Debug.Interval = TimeSpan.FromMilliseconds(250);

            animationTimer.Tick += AnimationRate_Tick;
            animationTimer.Interval = TimeSpan.FromMilliseconds(100);

            backgrounSprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Img/backgroundImg.png"));

            background1.Fill = backgrounSprite;
            background2.Fill = backgrounSprite;

            foreach (var x in myCanvas.Children.OfType<Rectangle>())
            {
                if ((string)x.Tag == "platform")
                {
                    platformsStartingLocationLeft.Add(Canvas.GetLeft(x));
                    platformsStartingLocationTop.Add(Canvas.GetTop(x));
                }

                foreach (var y in myCanvas.Children.OfType<Rectangle>())
                {
                    if ((string)y.Tag == (string)x.Name
                            &&
                                y.Name != "point1" &&
                                y.Name != "point2" &&
                                y.Name != "point3" &&
                                y.Name != "point4" &&
                                y.Name != "point5" &&
                                y.Name != "point6" &&
                                y.Name != "point7" &&
                                y.Name != "point8" &&
                                y.Name != "point9" &&
                                y.Name != "point10" &&
                                y.Name != "point11"
                            )
                    {
                        obstaclesStartingLocationLeft.Add(Canvas.GetLeft(y));
                        obstaclesStartingLocationTop.Add(Canvas.GetTop(y));
                    }
                }

                foreach (var y in myCanvas.Children.OfType<Rectangle>())
                {
                    if ((string)y.Tag == (string)x.Name
                    &&
                    (
                        y.Name == "point1" ||
                        y.Name == "point2" ||
                        y.Name == "point3" ||
                        y.Name == "point4" ||
                        y.Name == "point5" ||
                        y.Name == "point6" ||
                        y.Name == "point7" ||
                        y.Name == "point8" ||
                        y.Name == "point9" ||
                        y.Name == "point10" ||
                        y.Name == "point11"
                    ))
                    {
                        pointsStartingLocationLeft.Add(Canvas.GetLeft(y));
                        pointsStartingLocationTop.Add(Canvas.GetTop(y));
                    }
                }

            }





            StartGame();
        }

        private void AnimationRate_Tick(object sender, EventArgs e)
        {
            counter_Debug++;

            if (playerDirectionIsLeft == true)
            {
                if (playerAnimation == true)
                {
                    player.Fill = playerAnimationLeft1;
                    playerAnimation = false;
                }
                else
                {
                    player.Fill = playerAnimationLeft2;
                    playerAnimation = true;
                }
            }
            else
            {
                if (playerAnimation == true)
                {
                    player.Fill = playerAnimationRight1;
                    playerAnimation = false;
                }
                else
                {
                    player.Fill = playerAnimationRight2;
                    playerAnimation = true;
                }
            }
        }

        private void seconds_Debug_Tick(object sender, EventArgs e)
        {
            interfaceFramerate.Content = $"FPS: {framesCounter_Debug * 4}";
            framesCounter_Debug = 0;
        }

        private void StartGame()
        {
            playerJumping = false;
            playerLeft = false;
            playerRight = false;
            playerIsOnTheGround = false;
            playerCanJump = false;
            playerFlying = false;
            score = 0;
            playerDirectionIsLeft = false;
            force = gForce;
            speed = 5;
            playerJumpStrengthCounter = 0;
            playerJumpStrengthSwitch = false;
            gameIsOver = false;
            playerAnimation = false;
            previousPlatformPosition = 0;

            interface_GAMEOVER.Content = "";

            playerAnimationLeft1.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Img/Left1.png"));
            playerAnimationLeft2.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Img/Left2.png"));
            playerAnimationRight1.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Img/Right1.png"));
            playerAnimationRight2.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Img/Right2.png"));
            playerAnimationJump1.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Img/JumpLeft.png"));
            playerAnimationJump2.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Img/JumpRight.png"));
            obstacle1.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Img/Spikes.png"));
            brick.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Img/Brick.png"));

            Canvas.SetLeft(player, 45);
            Canvas.SetTop(player, 650);
            player.Fill = playerAnimationRight1;
      
            int count1 = 0;
            int count2 = 0;
            int count3 = 0;

            foreach (var x in myCanvas.Children.OfType<Rectangle>())
            {
                if ((string)x.Tag == "platform")
                {
                    Canvas.SetLeft(x, platformsStartingLocationLeft.ElementAt(count1));
                    Canvas.SetTop(x, platformsStartingLocationTop.ElementAt(count1));
                    count1++;
                }

                foreach (var y in myCanvas.Children.OfType<Rectangle>())
                {
                    if ((string)y.Tag == (string)x.Name
                            &&
                                y.Name != "point1" &&
                                y.Name != "point2" &&
                                y.Name != "point3" &&
                                y.Name != "point4" &&
                                y.Name != "point5" &&
                                y.Name != "point6" &&
                                y.Name != "point7" &&
                                y.Name != "point8" &&
                                y.Name != "point9" &&
                                y.Name != "point10" &&
                                y.Name != "point11"
                            )
                    {
                        Canvas.SetLeft(y, obstaclesStartingLocationLeft.ElementAt(count2));
                        Canvas.SetTop(y, obstaclesStartingLocationTop.ElementAt(count2));
                        count2++;
                    }
                }

                foreach (var y in myCanvas.Children.OfType<Rectangle>())
                {
                    if ((string)y.Tag == (string)x.Name
                    &&
                    (
                        y.Name == "point1" ||
                        y.Name == "point2" ||
                        y.Name == "point3" ||
                        y.Name == "point4" ||
                        y.Name == "point5" ||
                        y.Name == "point6" ||
                        y.Name == "point7" ||
                        y.Name == "point8" ||
                        y.Name == "point9" ||
                        y.Name == "point10" ||
                        y.Name == "point11"
                    ))
                    {
                        Canvas.SetLeft(y, pointsStartingLocationLeft.ElementAt(count3));
                        Canvas.SetTop(y, pointsStartingLocationTop.ElementAt(count3));
                        y.Visibility = Visibility.Visible;
                        count3++;
                    }
                }
            }


            gameFramerate.Start();
            secondsTimer_Debug.Start();


            foreach (var x in myCanvas.Children.OfType<Rectangle>())
            {
                if ((string)x.Tag == "platform")
                {
                    x.Fill = brick;
                    x.Stretch = Stretch.Fill;

                }
                foreach (var y in myCanvas.Children.OfType<Rectangle>())
                {
                    if ((string)y.Tag == (string)x.Name
                            &&
                                y.Name != "point1" &&
                                y.Name != "point2" &&
                                y.Name != "point3" &&
                                y.Name != "point4" &&
                                y.Name != "point5" &&
                                y.Name != "point6" &&
                                y.Name != "point7" &&
                                y.Name != "point8" &&
                                y.Name != "point9" &&
                                y.Name != "point10" &&
                                y.Name != "point11"
                            )
                    {
                        y.Fill = obstacle1;
                        y.Stretch = Stretch.Fill;
                    }
                }
            }

        }



        //=================================



        private void GameEngine(object sender, EventArgs e)
        {
            playerHitBox = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width, player.Height);
            playerFootHitBox = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player) + player.Height - ((player.Height / 10)), player.Width, ((player.Height / 10) * 9));

            if (playerJumping == true)
            {
                if (playerDirectionIsLeft == true)
                {
                    player.Fill = playerAnimationJump1;
                }
                else player.Fill = playerAnimationJump2;
            }


            interface_score.Content = $"Score: {score}";

            framesCounter_Debug++;

            foreach (var x in myCanvas.Children.OfType<Rectangle>())
            {
                if ((string)x.Tag == "platform")
                {
                    platformHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    if (platformHitBox.IntersectsWith(playerFootHitBox) && playerJumping == false)
                    {
                        playerIsOnTheGround = true;
                        playerCanJump = true;
                        Canvas.SetTop(player, platformHitBox.Top - player.Height);
                    }
                    else playerIsOnTheGround = false;

                    foreach (var y in myCanvas.Children.OfType<Rectangle>())
                    {
                        if ((string)y.Tag == (string)x.Name
                            &&
                                y.Name != "point1" &&
                                y.Name != "point2" &&
                                y.Name != "point3" &&
                                y.Name != "point4" &&
                                y.Name != "point5" &&
                                y.Name != "point6" &&
                                y.Name != "point7" &&
                                y.Name != "point8" &&
                                y.Name != "point9" &&
                                y.Name != "point10" &&
                                y.Name != "point11"
                            )
                        {
                            obstacleHitBox = new Rect(Canvas.GetLeft(y), Canvas.GetTop(y), y.Width, y.Height);
                            if (playerHitBox.IntersectsWith(obstacleHitBox) && y.Visibility == Visibility.Visible)
                            {
                                gameIsOver = true;
                                interface_GAMEOVER.Content = "Press Enter To Restart";
                                gameFramerate.Stop();
                                animationTimer.Stop();
                            }
                        }
                        if ((string)y.Tag == (string)x.Name
                            &&
                            (
                                y.Name == "point1" ||
                                y.Name == "point2" ||
                                y.Name == "point3" ||
                                y.Name == "point4" ||
                                y.Name == "point5" ||
                                y.Name == "point6" ||
                                y.Name == "point7" ||
                                y.Name == "point8" ||
                                y.Name == "point9" ||
                                y.Name == "point10" ||
                                y.Name == "point11"
                            ))
                        {
                            pointHitBox = new Rect(Canvas.GetLeft(y), Canvas.GetTop(y), y.Width, y.Height);
                            if (playerHitBox.IntersectsWith(pointHitBox) && y.Visibility == Visibility.Visible)
                            {
                                score++;
                                y.Visibility = Visibility.Hidden;
                            }
                        }
                    }
                }
            }



            if (playerLeft == true)
            {
                Canvas.SetLeft(player, Canvas.GetLeft(player) - speed);
            }

            if (playerRight == true)
            {
                Canvas.SetLeft(player, Canvas.GetLeft(player) + speed);
            }

            if (playerIsOnTheGround == false && playerJumping == false)
            {
                Canvas.SetTop(player, Canvas.GetTop(player) + force);
            }

            if (playerJumping == true && playerCanJump == true && playerFlying == false)
            {
                playerFlying = true;
                Canvas.SetTop(player, Canvas.GetTop(player) - flying);
                flying = 12;
            }



            if (playerJumpStrengthSwitch == true)
            {
                if (playerJumpStrengthCounter < 25)
                {
                    playerJumpStrengthCounter++;
                    switch (playerJumpStrengthCounter)
                    {
                        case 6:
                            flying += 3;
                            break;
                        case 12:
                            flying += 3;
                            break;
                        case 18:
                            flying += 3;
                            break;
                        case 24:
                            flying += 3;
                            break;
                    }
                }

            }
            if (playerFlying == true)
            {
                if ((playerJumpStrengthCounter > 24 || playerJumpStrengthSwitch == false) || flying > 15)
                {
                    playerJumpStrengthSwitch = false;
                    flying--;
                }
                Canvas.SetTop(player, Canvas.GetTop(player) - flying);
                if (flying == 0)
                {
                    playerFlying = false;
                    playerJumping = false;
                    if (playerDirectionIsLeft == true)
                    {
                        player.Fill = playerAnimationLeft1;
                    }
                    else player.Fill = playerAnimationRight1;
                }
            }
            foreach (var x in myCanvas.Children.OfType<Rectangle>())
            {
                if ((string)x.Tag == "platform")
                {
                    platformHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    if (platformHitBox.IntersectsWith(playerFootHitBox) && playerJumping == false)
                    {
                        playerIsOnTheGround = true;
                        playerCanJump = true;
                        Canvas.SetTop(player, platformHitBox.Top - player.Height);
                    }
                }
            }


            if (Canvas.GetLeft(player) < 0)
            {
                Canvas.SetLeft(player, speed + 1);
            }

            if (Canvas.GetLeft(background1) < -background1.Width - 1)
            {
                Canvas.SetLeft(background1, 2780);
            }

            if (Canvas.GetLeft(background2) < -background2.Width - 1)
            {
                Canvas.SetLeft(background2, 2780);
            }

            if (Canvas.GetLeft(player) > 800)
            {
                Canvas.SetLeft(background1, Canvas.GetLeft(background1) - 5);
                Canvas.SetLeft(background2, Canvas.GetLeft(background2) - 5);
                foreach (var x in myCanvas.Children.OfType<Rectangle>())
                {
                    if ((string)x.Tag == "platform")
                    {
                        Canvas.SetLeft(x, Canvas.GetLeft(x) - speed);
                        foreach (var y in myCanvas.Children.OfType<Rectangle>())
                        {
                            if ((string)y.Tag == (string)x.Name)
                            {
                                Canvas.SetLeft(y, Canvas.GetLeft(y) - speed);
                            }
                        }
                    }
                }
                Canvas.SetLeft(player, Canvas.GetLeft(player) - speed);
            }


            foreach (var x in myCanvas.Children.OfType<Rectangle>())
            {
                if (Canvas.GetLeft(x) + x.Width <= 0 && (string)x.Tag == "platform" && (string)x.Name != "g1" && (string)x.Name != "g2")
                {

                    switch (rng.Next(0, 2) + 1)
                    {
                        case 1:
                            if (previousPlatformPosition == 1)
                            {
                                if (rng.Next(0, 2) == 0)
                                {
                                    previousPlatformPosition = 3;
                                    platformPositionTop = 600;
                                }
                                else
                                {
                                    previousPlatformPosition = 2;
                                    platformPositionTop = 400;
                                }
                            }
                            else
                            {
                                previousPlatformPosition = 1;
                                platformPositionTop = 200;
                            }
                            break;

                        case 2:
                            if (previousPlatformPosition == 2)
                            {
                                if (rng.Next(0, 2) == 0)
                                {
                                    previousPlatformPosition = 1;
                                    platformPositionTop = 200;
                                }
                                else
                                {
                                    previousPlatformPosition = 3;
                                    platformPositionTop = 600;
                                }
                            }
                            else
                            {
                                previousPlatformPosition = 2;
                                platformPositionTop = 400;
                            }
                            break;
                        case 3:
                            if (previousPlatformPosition == 3)
                            {
                                if (rng.Next(0, 2) == 0)
                                {
                                    previousPlatformPosition = 1;
                                    platformPositionTop = 200;
                                }
                                else
                                {
                                    previousPlatformPosition = 2;
                                    platformPositionTop = 400;
                                }
                            }
                            else
                            {
                                previousPlatformPosition = 3;
                                platformPositionTop = 600;
                            }
                            break;
                    }

                    switch (rng.Next(0, 9) + 1)
                    {
                        case 1:
                            platformPositionLeft = 0;
                            break;
                        case 2:
                            platformPositionLeft = 20;
                            break;
                        case 3:
                            platformPositionLeft = 40;
                            break;
                        case 4:
                            platformPositionLeft = 60;
                            break;
                        case 5:
                            platformPositionLeft = 80;
                            break;
                        case 6:
                            platformPositionLeft = -20;
                            break;
                        case 7:
                            platformPositionLeft = -40;
                            break;
                        case 8:
                            platformPositionLeft = -60;
                            break;
                        case 9:
                            platformPositionLeft = -80;
                            break;
                    }
                    Canvas.SetLeft(x, (Canvas.GetLeft(x) + x.Width + 1700 + platformPositionLeft));
                    Canvas.SetTop(x, platformPositionTop);

                    foreach (var y in myCanvas.Children.OfType<Rectangle>())
                    {
                        if ((string)y.Tag == (string)x.Name
                            &&
                                y.Name != "point1" &&
                                y.Name != "point2" &&
                                y.Name != "point3" &&
                                y.Name != "point4" &&
                                y.Name != "point5" &&
                                y.Name != "point6" &&
                                y.Name != "point7" &&
                                y.Name != "point8" &&
                                y.Name != "point9" &&
                                y.Name != "point10" &&
                                y.Name != "point11"
                            )
                        {
                            Canvas.SetLeft(y, (Canvas.GetLeft(y) + x.Width + 1700 + platformPositionLeft));
                            Canvas.SetTop(y, Canvas.GetTop(x) - y.Height);
                        }


                        if ((string)y.Tag == (string)x.Name
                                &&
                                (
                                    y.Name == "point1" ||
                                    y.Name == "point2" ||
                                    y.Name == "point3" ||
                                    y.Name == "point4" ||
                                    y.Name == "point5" ||
                                    y.Name == "point6" ||
                                    y.Name == "point7" ||
                                    y.Name == "point8" ||
                                    y.Name == "point9" ||
                                    y.Name == "point10" ||
                                    y.Name == "point11" ||
                                    y.Name == "point12"
                                ))
                        {
                            y.Visibility = Visibility.Visible;
                            Canvas.SetLeft(y, (Canvas.GetLeft(y) + x.Width + 1700 + platformPositionLeft));
                            Canvas.SetTop(y, Canvas.GetTop(x) - y.Height - 80);
                        }

                    }
                }
                if (Canvas.GetLeft(x) + x.Width <= 0 && (string)x.Tag == "platform" && (string)x.Name == "g1")
                {
                    Canvas.SetLeft(x, 1735);
                    foreach (var y in myCanvas.Children.OfType<Rectangle>())
                    {
                        if ((string)y.Tag == (string)x.Name)
                        {
                            if (rng.Next(0, 2) == 0)
                            {
                                y.Visibility = Visibility.Hidden;
                            }
                            else
                            {
                                y.Visibility = Visibility.Visible;
                            }
                            Canvas.SetLeft(y, (Canvas.GetLeft(y) + x.Width + 1700 + platformPositionLeft));
                            Canvas.SetTop(y, Canvas.GetTop(x) - y.Height);
                        }

                    }
                }
                if (Canvas.GetLeft(x) + x.Width <= 0 && (string)x.Tag == "platform" && (string)x.Name == "g2")
                {
                    Canvas.SetLeft(x, 1735);
                    foreach (var y in myCanvas.Children.OfType<Rectangle>())
                    {
                        if ((string)y.Tag == (string)x.Name)
                        {
                            if (rng.Next(0, 2) == 0)
                            {
                                y.Visibility = Visibility.Hidden;
                            }
                            else
                            {
                                y.Visibility = Visibility.Visible;
                            }
                            Canvas.SetLeft(y, (Canvas.GetLeft(y) + x.Width + 1700 + platformPositionLeft));
                            Canvas.SetTop(y, Canvas.GetTop(x) - y.Height);
                        }

                    }
                }
            }
        }
    }
}
