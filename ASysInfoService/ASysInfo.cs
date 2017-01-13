using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LibASysInfo;

namespace ASysInfoService
{
    public partial class ASysInfo : ServiceBase
    {

        private readonly Thread _mainLoopThread;

        private volatile bool _stopMainLoop = false;

        public ASysInfo()
        {
            InitializeComponent();
            _mainLoopThread = new Thread(MainLoop);
        }

        private void MainLoop()
        {
            _stopMainLoop = false;
            var cpuCounter =
                new PerformanceCounter("Processor", "% Processor Time", "_Total", true);

            using (var device = new Device())
            {
                //TODO: реализовать чтение из конфига
                device.Connect();
                device.SendInitCommand();
                while (!_stopMainLoop)
                {
                    device.SendCommand(Convert.ToByte(cpuCounter.NextValue()), MemoryInfo.GetUsagePercentMemory());
                    Thread.Sleep(700);
                }
            }
        }

        protected override void OnStart(string[] args)
        {
            _mainLoopThread.Start();
        }

        protected override void OnStop()
        {
            if (_mainLoopThread.IsAlive)
            {
                _stopMainLoop = true;
            }
            if(!_mainLoopThread.Join(10000))
                _mainLoopThread.Abort();
        }
    }
}
