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
using NAudio.Wave;

namespace MetronomeBeat {
    public partial class MainWindow : Window {

        private System.Media.SoundPlayer player = null;
        WaveOut player2 = new WaveOut();
        WaveOut player3 = new WaveOut();
        private readonly string ClickSoundFile = "click.wav";
        private readonly string FeedbackSoundFile = "feedback.wav";
        private bool makeClickSound = false;
        private bool makeFeedbackSound = false;
        private List<DateTime>[] outputTimeList = new List<DateTime>[3];

        private DispatcherTimer _timer;
        private DateTime prevPlayedTime = DateTime.Now;
        private readonly double bpm = 120;

        private bool monitoring = false;

        public MainWindow() {
            InitializeComponent();
            InitializeTimer();
            player2.Init(new AudioFileReader(FeedbackSoundFile));
            player2.PlaybackStopped += (s, e) => {
                player2.Init(new AudioFileReader(FeedbackSoundFile));
            };
            player3.Init(new AudioFileReader(FeedbackSoundFile));
            player3.PlaybackStopped += (s, e) => {
                player3.Init(new AudioFileReader(FeedbackSoundFile));
            };
            for (int i = 0; i < 3; i++) {
                outputTimeList[i] = new List<DateTime>();
            }
        }

        private void PlayClickSound() {
            player = new System.Media.SoundPlayer(ClickSoundFile);
            player.Play();
        }
        private void PlayFeedbackSound(int a) {
            if (a == 0) player2.Play();
            else player3.Play();
        }
        private void Button_Click(object sender, RoutedEventArgs e) {
            if (monitoring) {
                monitoring = false;
                button.Content = "計測開始";
                text.Text = "状態：待機中";
                CsvOut.write(outputTimeList);
            } else {
                monitoring = true;
                button.Content = "計測停止";
                text.Text = "状態：計測中";
                outputTimeList = new List<DateTime>[3];
                for (int i = 0; i < 3; i++) {
                    outputTimeList[i] = new List<DateTime>();
                }
            }
        }
        private void TimerMethod() {
            if ((DateTime.Now - prevPlayedTime).TotalMinutes > 1 / bpm) {
                prevPlayedTime += new TimeSpan(0, 0, 0, 0, (int)(60000 / bpm));
                if (makeClickSound) PlayClickSound();
                outputTimeList[0].Add(DateTime.Now);
            }
        }

        private void InitializeTimer() {
            _timer = new DispatcherTimer(DispatcherPriority.Send) {
                Interval = new TimeSpan(0, 0, 0, 0, 10),
            };
            _timer.Tick += (e, s) => { TimerMethod(); };
            this.Closing += (e, s) => { _timer.Stop(); };
            prevPlayedTime = DateTime.Now;
            // PlaySound();
            _timer.Start();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e) {
            makeClickSound = true;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e) {
            makeClickSound = false;
        }

        private void CheckBox_Checked_1(object sender, RoutedEventArgs e) {
            makeFeedbackSound = true;
        }

        private void CheckBox_Unchecked_1(object sender, RoutedEventArgs e) {
            makeFeedbackSound = false;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key != Key.A && e.Key != Key.L) return;
            if (makeFeedbackSound) PlayFeedbackSound(e.Key == Key.A ? 1 : 0);
            if (!monitoring) return;
            if(e.Key == Key.A) {
                outputTimeList[1].Add(DateTime.Now);
            } else {
                outputTimeList[2].Add(DateTime.Now);
            }
        }
    }

}
