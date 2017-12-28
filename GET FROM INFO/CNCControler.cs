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

            short ret = Focas1.cnc_allclibhndl3(_IpAddr, _port, _timeOut, out _h);
            if (ret != 0)
            {
                return "ERROR" + ret + "\n";
            }
 
            readDiagnoseExample("connect");
            return "CONNECTION OK" + "\n";
            
        }

        public void StartRecording()
        {
            //Configure periodic task
            _stop.Reset();
            _registeredWait = ThreadPool.RegisterWaitForSingleObject(_stop, new WaitOrTimerCallback(PeriodicProcess), null, _delay_ms, false);

            //Init CSV with headers
            _dataStream.WriteLine("Axe ID ; Data type ; Mac Addr ; Time ; value");

            readDiagnoseExample("start recording");
        }

        public void StopRecording()
        {
            _stop.Set();
        }

        private void PeriodicProcess(object state, bool timeout)
        {
            if (timeout)
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Reset();
                stopwatch.Start();


                readDiagnoseExample("periodic");

                // Periodic processing here
                //Read diagnose area
                var tabDiag = new Focas1.ODBDGN[100];
                short s_number = 4920;
                short e_number = 4922;
                short axis = 1;
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
               
                _dataStream.WriteLine("10;CNC Focas;" + DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond + ";" + _counter);
                
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


        private string _IpAddr;
        private ushort _port;
        private int _timeOut;
        private int _delay_ms;
        private int _counter;
        private StreamWriter _dataStream;
        private ushort _h; // LIBRARY HANDLE

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
