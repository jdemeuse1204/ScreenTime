﻿using ScreenTime.Managers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace ScreenTime
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Processor Processor;
        private readonly System.Timers.Timer Timer;

        public MainWindow()
        {
            Processor = new Processor();
            InitializeComponent();

            // need to run immediated when the window opens
            Process();

            Timer = new System.Timers.Timer
            {
                Interval = 5000
            };
            Timer.Elapsed += TimerElapsed;
            Timer.Start();            
        }

        private void TimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Task.Run(() =>
            {
                Process();
            });
        }

        private void Process()
        {
            Processor.Run();

            this.Dispatcher.Invoke(() =>
            {
                this.ProcessesGrid.ItemsSource = null;
                this.ProcessesGrid.ItemsSource = Processor.WindowsProcesses;
                this.ProcessesGrid.Items.Refresh();
            });
        }
        
        protected override void OnClosing(CancelEventArgs e)
        { 
            Timer.Dispose();
            Processor.Dispose();
            base.OnClosing(e);
        }
    }
}
