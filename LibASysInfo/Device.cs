using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LibASysInfo
{
    public class Device : IDisposable
    {
        public string SerialPortName { get; set; } = "COM5";

        public int SerialPortBaudRate { get; set; } = 57600;

        public int SerialPortReadTimeout = 500;

        public int SerialPortWriteTimeout = 500;

        protected readonly string CmdTemplate = "1{0,3:000}{1,3:000}";

        protected SerialPort Port = null;
        
        public void Connect()
        {
            if(PostIsReady())
                Dispose();

            Port = new SerialPort
            {
                PortName = SerialPortName,
                BaudRate = SerialPortBaudRate,
                ReadTimeout = SerialPortReadTimeout,
                WriteTimeout = SerialPortWriteTimeout
            };
            try
            {
                Port.Open();
            }
            catch (Exception ex)
            {
                throw new ASysInfoException($"Не удалось открыть порт {SerialPortName}.", ex);
            }
        }

        public void Dispose()
        {
            if (null == Port) return;
            try
            {
                if (Port.IsOpen)
                    Port.Close();
            }
            catch (IOException ex)
            {
                throw new ASysInfoException($"Не удалось закрыть порт {SerialPortName}.", ex);
            }
            Port = null;
        }

        public void SendInitCommand(int sleepAfterSend = 10000)
        {
            SendCommand("INIT", sleepAfterSend);
        }

        public void SendCommand(byte cpuUsage, byte ramUsage)
        {
            SendCommand(string.Format(CmdTemplate, cpuUsage, ramUsage));
        }

        protected void SendCommand(string cmd, int sleepAfterSend = 0)
        {
            PostIsReady(true);

            try
            {
                Port.WriteLine(cmd);
            }
            catch (Exception ex)
            {
                throw new ASysInfoException("При отправке данных произошла ошибка.", ex);
            }

            if (0 != sleepAfterSend)
                Thread.Sleep(sleepAfterSend);
        }

        protected bool PostIsReady(bool exceptionOnNotReady = false)
        {
            var isReady = (null != Port && Port.IsOpen);

            if(!isReady && exceptionOnNotReady)
                throw new ASysInfoException("Перед отправкой данных необходимо открыть порт.");

            return isReady;
        }
    }
}
