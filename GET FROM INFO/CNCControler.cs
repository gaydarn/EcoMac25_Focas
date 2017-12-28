using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CNC_WRMACRO
{
    class CNCControler
    {

        public CNCControler(string IpAddr, ushort port, int delay_ms = 1000, StreamWriter dataStream = null , int timeOut = 10)
        {
            _IpAddr = IpAddr;
            _port = port;
            _timeOut = timeOut;
            _delay_ms = delay_ms;
            _dataStream = dataStream;
            _counter = 0;
        }

        public string Connect()
        {
            // CONNECTION TO NCGuide 

            short ret = Focas1.cnc_allclibhndl3("172.16.199.10", 8193, 10, out _h);
            if (ret != 0)
            {
                Console.WriteLine("ERROR" + ret + "\n");
            }
            else
            {
                Console.WriteLine("CONNECTION OK" + "\n");
            }


            readDiagnoseExample("connect");

            return "done";
        }

        public void StartRecording()
        {
            _stop.Reset();
            _registeredWait = ThreadPool.RegisterWaitForSingleObject(_stop,
                new WaitOrTimerCallback(PeriodicProcess), null, _delay_ms, false);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Reset();
            stopwatch.Start();
            //Write header in csv
            _dataStream.WriteLine("Axe ID ; Data type ; Mac Addr ; Time ; value");
            
            readDiagnoseExample("start recording");
            readDiagnoseExample("start recording");
            readDiagnoseExample("start recording");
            readDiagnoseExample("start recording");
            readDiagnoseExample("start recording");
            readDiagnoseExample("start recording");
            stopwatch.Stop();
            Console.WriteLine("Ticks: " + stopwatch.ElapsedTicks +
            " mS: " + stopwatch.ElapsedMilliseconds);
        }

        public void StopRecording()
        {
            _stop.Set();
        }

        private void PeriodicProcess(object state, bool timeout)
        {
            Stopwatch stopwatch = new Stopwatch();
            if (timeout)
            {
                stopwatch.Reset();
                stopwatch.Start();


                readDiagnoseExample("periodic");

                // Periodic processing here
                /*
                var tabDiag = new Focas1.ODBDGN[100];
                short s_number = 4920;
                short e_number = 4922;
                short axis = -1;
                short lenght = 100;
                for (int i= tabDiag.Length; i>0; i--)
                {
                    tabDiag[i-1] = new Focas1.ODBDGN();
                }
                short ret = Focas1.cnc_diagnosr(_h, ref s_number, e_number, ref axis, ref lenght, tabDiag);
                if (ret != 0)
                {
                    Console.WriteLine("ERROR" + ret);
                }
                else
                {
                    Console.Write("Diagnose N° " + s_number + " = " + tabDiag[0].u.idata + "\n\n");
                }
                */



                _dataStream.WriteLine("2;ff-00-54-AB-00;" + DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond + ";" + _counter);
                
                stopwatch.Stop();
                Console.WriteLine("Ticks: " + stopwatch.ElapsedTicks +
                " mS: " + stopwatch.ElapsedMilliseconds);
                _counter++;
            }
            else
                // Stop any more events coming along
                _registeredWait.Unregister(null);
        }

        private ManualResetEvent _stop = new ManualResetEvent(false);
        private RegisteredWaitHandle _registeredWait;


        string _IpAddr;
        ushort _port;
        int _timeOut;
        int _delay_ms;
        int _counter;
        StreamWriter _dataStream;
        ushort _h; // LIBRARY HANDLE

        public void readDiagnoseExample(string from)
        {
            // CNC Read Diagnose
            short number = 1002; //Number of diagnose
            short axis = 0;   // 0: assigns no axis
            short length;
            Focas1.ODBDGN diag = new Focas1.ODBDGN();

            length = 4 + 8 * 1;

            short ret = Focas1.cnc_diagnoss(_h, number, axis, length, diag);

            if (ret != 0)
                Console.WriteLine("ERROR" + ret);
            else
                Console.Write(from + " - Diagnose N° " + number + " = " + diag.u.idata + "\n\n");
        }
    }
}
