/**
 * A timed game that prompts the user to answer addition problems.
 * 
 * @author Ernesto Martinez
 * @since 7-14-2017
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace WPFTestApp
{

    public partial class MainWindow : Window
    {
        int answer, problemCounter, numberPlaces = 10, totalQuestions = 3;
        string totalTimeTemp, tempScoreLoadString = null;
        List<double> numList;
        List<string> scoreList;
        MediaPlayer player = new MediaPlayer();
        Stopwatch timer = new Stopwatch();
        

        public MainWindow()
        {
            InitializeComponent();
        }

        /**
         * Sets the size of the numbers that will be used in the addition problems
         * @param sender Unused
         * @param e Unused
         */
        private void NumberSizeClick(object sender, RoutedEventArgs e)
        {
            player.Open(new Uri("sound_ding.mp3", UriKind.RelativeOrAbsolute));
            player.Play();

            if (numberPlaces1.IsChecked == true)
            {
                numberPlaces = 10;
            }
            else if (numberPlaces2.IsChecked == true)
            {
                numberPlaces = 100;
            }
            else
            {
                numberPlaces = 1000;
            }
        }

        /**
         * Sets the total number of questions that will be presented in the game
         * @param sender Unused
         * @param e Unused
         */
        private void QuestionsClick(object sender, RoutedEventArgs e)
        {
            player.Open(new Uri("sound_ding.mp3", UriKind.RelativeOrAbsolute));
            player.Play();

            if (questionsOption1.IsChecked == true)
            {
                totalQuestions = 3;
            }
            else if (questionsOption2.IsChecked == true)
            {
                totalQuestions = 5;
            }
            else
            {
                totalQuestions = 10;
            }
        }

        /**
         * Calls for the start of the game (StartGame)
         * @param sender Unused
         * @param e Unused
         */
        private void StartClick(object sender, RoutedEventArgs e)
        {
            buttonStartGame.Visibility = Visibility.Collapsed;
            StartGame();
            
        }

        /**
         * This method controls the flow of a new game.
         *  -The answer box is given focus.
         *  -The timer is started.
         *  -A new addition problem is procured.
         */
        public void StartGame()
        {
            //Default values reasserted for consecutive plays
            labelElapsedTime.Visibility = Visibility.Collapsed;
            textAnswerBox.Visibility = Visibility.Visible;
            buttonPlayAgain.Visibility = Visibility.Collapsed;
            LabelTopScoresTitle.Visibility = Visibility.Collapsed;
            labelTopScores.Visibility = Visibility.Collapsed;
            buttonResetScores.Visibility = Visibility.Collapsed;
            problemCounter = 1;
            //

            labelNumberSize.Visibility = Visibility.Collapsed;
            labelQuestionsOption.Visibility = Visibility.Collapsed;
            stackNumberSizeGroup.Visibility = Visibility.Collapsed;
            questions_group.Visibility = Visibility.Collapsed;
            labelQuestion.Visibility = Visibility.Visible;
            textAnswerBox.Visibility = Visibility.Visible;

            textAnswerBox.Focus();
            timer.Restart();
            GetRandomMathProblem();
        }

        /**
         * Generates a new addition problem
         */
        public void GetRandomMathProblem()
        {
            Random rand = new Random();
            int num1 = rand.Next((numberPlaces/10), numberPlaces-1);
            int num2 = rand.Next((numberPlaces/10), numberPlaces-1);

            labelQuestion.Content = num1 + " + " + num2;

            answer = num1 + num2;

            //textAnswerBox.Text = answer.ToString();
        }

        /**
         * This method is called when the player enters an answer.
         * @param sender Unused
         * @param e Contains information about the key event
         */
        private void GameAnswerBoxKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Return)
            {
                if(Int32.TryParse(textAnswerBox.Text, out int x)) {
                    if (answer == Int32.Parse(textAnswerBox.Text))
                    {
                        player.Open(new Uri("sound_ding.mp3", UriKind.RelativeOrAbsolute));
                        player.Play();
                        labelIsCorrect.Content = "CORRECT";
                        textAnswerBox.Text = "";

                        if(problemCounter < totalQuestions) {
                            GetRandomMathProblem();
                        }
                        else
                        {
                            player.Open(new Uri("sound_applause.wav", UriKind.RelativeOrAbsolute));
                            player.Play();

                            labelIsCorrect.Content = "Your Time";
                            TimeSpan timeSpan = new TimeSpan();
                            timeSpan = timer.Elapsed;
                            totalTimeTemp = String.Format("{0}.{1}", ((timeSpan.Minutes * 60) + timeSpan.Seconds)
                                ,timeSpan.Milliseconds);
                            double totalTime = Double.Parse(totalTimeTemp);
                            labelElapsedTime.Content = totalTime + " seconds!";

                            SaveScore();

                            LabelTopScoresTitle.Visibility = Visibility.Visible;
                            labelTopScores.Visibility = Visibility.Visible;
                            buttonResetScores.Visibility = Visibility.Visible;
                            labelElapsedTime.Visibility = Visibility.Visible;
                            textAnswerBox.Visibility = Visibility.Collapsed;
                            buttonPlayAgain.Visibility = Visibility.Visible;
                        }

                        problemCounter++;
                    }
                    else
                    {
                        player.Open(new Uri("sound_buzz.mp3", UriKind.RelativeOrAbsolute));
                        player.Play();
                        labelIsCorrect.Content = "INCORRECT";
                        textAnswerBox.Text = "";
                    }
                }
                else
                {
                    player.Open(new Uri("sound_buzz.mp3", UriKind.RelativeOrAbsolute));
                    player.Play();
                    labelIsCorrect.Content = "Not A Number";
                }
            }
        }

        /**
         * Save function control:
         *   -Calls for sorting of scores
         *   -Writes to file
         *   -Calls for scores to be displayed
         */
        public void SaveScore()
        {
            try
            {
                if (!File.Exists("scores.txt"))
                {
                    //Do nothing if file doesn't exists. File is created by textWriter.
                }
                else
                {
                    tempScoreLoadString = System.IO.File.ReadAllText("scores.txt");
                }
            }
            catch (IOException)
            {
                MessageBox.Show("Could not create scores.txt file");
            }

            try{
                using (TextWriter textWriter = new StreamWriter("scores.txt", true))
                {
                    textWriter.WriteLine(totalTimeTemp);
                }

                using (TextReader textReader = new StreamReader("scores.txt"))
                {
                    tempScoreLoadString = textReader.ReadToEnd();
                    scoreList = new List<string>(tempScoreLoadString.Split('\n'));
                }

                SortScores();

                File.Delete("scores.txt");
                using(TextWriter textWriter = new StreamWriter("scores.txt", true))
                {
                    numList.ForEach(delegate (double score)
                    {
                            textWriter.WriteLine(score.ToString());
                    });
                }

                LoadScores();
            }
            catch (IOException)
            {
                MessageBox.Show("IOException");
            }
        }

        /**
         * Sorts the scores
         */
        public void SortScores()
        {
            double temp;
            numList = new List<double>();

            scoreList.ForEach(delegate(string score){
                if(Double.TryParse(score, out double x))
                {
                    numList.Add(Double.Parse(score));
                }
            });

            for (int i = 0; i < numList.Count; i++)
            {
                for(int j = 0; j < numList.Count; j++)
                {
                    if(numList[i] > numList[j] &&  i < j){
                        temp = numList[i];
                        numList[i] = numList[j];
                        numList[j] = temp;
                    }
                }
            }

            if(numList.Count > 5)
            {
                numList.RemoveAt(5);
            }
        }

        /**
         * Displays scores to screen
         */
        public void LoadScores()
        {
            tempScoreLoadString = null;
            using (TextReader textReader = new StreamReader("scores.txt"))
            {
                tempScoreLoadString = textReader.ReadToEnd();
                scoreList = new List<string>(tempScoreLoadString.Split('\n'));
            }

            labelTopScores.Content = tempScoreLoadString;
        }

        /**
          * Resets scores
          */
        private void ResetScoresBtnClick(object sender, RoutedEventArgs e)
        {
            File.Delete("scores.txt");
            labelTopScores.Content = "";
        }

    }
}
