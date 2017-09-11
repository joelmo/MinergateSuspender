using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace MinergateSuspender
{
    public partial class MinergateSuspenderService : ServiceBase
    {
        public MinergateSuspenderService()
        {
            InitializeComponent();
            timer.Interval = 5000; // 5 seconds  
            timer.Elapsed += OnTimer;
        }

        private Timer timer = new Timer();

        protected override void OnStart(string[] args)
        {

            timer.Start();
            foreach (var m in Process.GetProcessesByName("minergate"))
            {
                m.Resume();
            }
            base.OnStart(args);
        }

        private static void OnTimer(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            var exclusives = new[] { "TslGame", "csgo", "dota2" };
            var exclusiveActives = Process.GetProcesses().FirstOrDefault(x => exclusives.Contains(x.ProcessName));
            if (exclusiveActives != null)
            {
                var minergates = Process.GetProcessesByName("minergate");
                foreach (var minergate in minergates)
                {
                    minergate.Suspend();
                }
                exclusiveActives.WaitForExit();
                foreach (var minergate in minergates)
                {
                    minergate.Resume();
                }
            }
        }

        protected override void OnStop()
        {
            timer.Stop();
        }
    }
}
