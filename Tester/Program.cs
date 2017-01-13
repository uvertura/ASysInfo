using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LibASysInfo;

namespace Tester
{
    public class Program
    {

        private static readonly Device Dev = new Device();

        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Title = Logo.Title;
            Console.WriteLine(Logo.BigTextLogo);

            while (true)
            {
                SetConnectionInfo(Dev);
                TestDevice(Dev);
            }
        }

        private static void TestDevice(Device device)
        {
            try
            {
                Console.WriteLine("Trying to connect...");
                device.Connect();
                Console.WriteLine("Connect successful.");

                string cmd = null;
                while (true)
                {
                    if (null == cmd || string.Empty != cmd)
                    {
                        Console.WriteLine("\nAvailable actions:");
                        Console.WriteLine(" 1. Self system check");
                        Console.WriteLine(" 2. Send data");
                        Console.WriteLine(" 3. Send current system info");
                        Console.WriteLine(" 4. Disconnect");
                        Console.WriteLine(" 5. Quit");
                        Console.WriteLine(" 6. Help (print this message)");
                    }
                    Console.Write("\nEnter action #>");
                    cmd = Console.ReadLine()?.Trim() ?? "";

                    switch (cmd)
                    {
                        case "1":
                            Dev.SendInitCommand();
                            break;
                        case "2":
                            var cpu = ReadByteValue("CPU", 0, 100);
                            var ram = ReadByteValue("RAM", 0, 100);
                            Dev.SendCommand(cpu, ram);
                            break;
                        case "3":
                            var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total", true);
                            
                            for (var i = 0; i < 20; ++i)
                            {
                                var _cpu = Convert.ToByte(cpuCounter.NextValue());
                                var _ram = MemoryInfo.GetUsagePercentMemory();
                                Dev.SendCommand(_cpu, _ram);
                                Console.Write($"\r{_cpu} {_ram}     ");
                                Thread.Sleep(500);
                            }
                            Console.WriteLine();
                            break;
                        case "4":
                        case "d":
                        case "disconnect":
                            return;
                        case "5":
                        case "q":
                        case "quit":
                            Environment.Exit(0);
                            break;
                        case "6":
                        case "h":
                        case "help":
                            break;
                        default:
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.WriteLine($"\n\"{cmd}\" - unknown command.\n");
                            Console.ForegroundColor = ConsoleColor.Green;
                            cmd = "";
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"{ex.Message}");
                if (null != ex.InnerException)
                    Console.WriteLine($"{ex.InnerException}");
                Console.ForegroundColor = ConsoleColor.Green;
            }
            finally
            {
                device.Dispose();
            }
        }

        private static void SetConnectionInfo(Device device)
        {
            string cmd = null;
            while (true)
            {
                if (null == cmd || string.Empty != cmd)
                {
                    Console.WriteLine("\nCurrent connection settings:");
                    Console.WriteLine($" 1. Port name:          {device.SerialPortName}");
                    Console.WriteLine($" 2. Baud Rate:          {device.SerialPortBaudRate}");
                    Console.WriteLine($" 3. Read timeout (ms):  {device.SerialPortReadTimeout}");
                    Console.WriteLine($" 4. Write timeout (ms): {device.SerialPortWriteTimeout}");

                    Console.WriteLine("\nAnother actions:");
                    Console.WriteLine(" q. Quit");
                    Console.WriteLine(" h. Help (print this message)");
                    
                }
                Console.Write("\nEnter action #>");
                cmd = Console.ReadLine()?.Trim() ?? "";

                switch (cmd)
                {
                    case "1":
                        device.SerialPortName = ReadStrValue("Port name");
                        break;
                    case "2":
                        device.SerialPortBaudRate = ReadIntValue("Baud Rate", 0, 1);
                        break;
                    case "3":
                        device.SerialPortReadTimeout = ReadIntValue("Read timeout");
                        break;
                    case "4":
                        device.SerialPortWriteTimeout = ReadIntValue("Write timeout");
                        break;
                    case "h":
                    case "help":
                        break;
                    case "c":
                    case "connect":
                        return;
                    case "q":
                    case "quit":
                        Environment.Exit(0);
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine($"\n\"{cmd}\" - unknown command.\n");
                        Console.ForegroundColor = ConsoleColor.Green;
                        cmd = "";
                        break;
                }
            }
        }
        
        private static string ReadStrValue(string paramName)
        {
            Console.Write($"{paramName}: ");
            return Console.ReadLine();
        }

        private static byte ReadByteValue(string paramName, byte min = byte.MinValue, byte max = byte.MaxValue)
        {
            return (byte) ReadIntValue(paramName, min, max);
        }

        private static int ReadIntValue(string paramName, int min = int.MinValue, int max = int.MaxValue)
        {
            int value;
            string rawValue = null;
            var minMax = true;

            while(true)
            {
                if (null != rawValue)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine($"\nValue \"{rawValue}\" is not valid. Expected number.");
                }

                if (!minMax)
                {
                    Console.WriteLine($"Min: {min}, Max: {max}.");
                }
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Green;

                Console.Write($"{paramName}: ");
                rawValue = Console.ReadLine();
                if (!int.TryParse(rawValue, out value))
                {
                    minMax = true;
                    continue;
                }

                minMax = (value >= min && value <= max);
                if (minMax)
                    return value;
            }
            
        }
        
    }
    
}
