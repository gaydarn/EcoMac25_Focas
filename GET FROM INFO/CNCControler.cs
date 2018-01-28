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

        public CNCControler(string IpAddr, ushort port, StructInfosCnc structInfo, int delay_ms = 1000, StreamWriter dataStream = null , int timeOut = 10)
        {
            _IpAddr = IpAddr;
            _port = port;
            _timeOut = timeOut;
            _structInfo = structInfo;
            _delay_ms = delay_ms;
            _dataStream = dataStream;
            _counter = 0;
        }

        private int cncGetHandle()
        {
            // CONNECTION TO NCGuide 
            return (int)Focas1.cnc_allclibhndl3(_IpAddr, _port, _timeOut, out _h);            
        }

        private int cncFreeHandle()
        {
            // DISCONNECTION TO NCGuide 
            return (int)Focas1.cnc_freelibhndl(_h);
        }

        public void StartRecording()
        {
            int ret = cncGetHandle();
            if (ret != 0)
            {
                Console.WriteLine("Unable to get the library handle" + " - ERROR : " + ret);
            }
            while (true)
            {
                //Stopwatch stopwatch = new Stopwatch();
                //stopwatch.Reset();
                //stopwatch.Start();

                for (int i = 0; i < _structInfo.structInfosCnc.Count; i++)
                {
                    StructDataCnc structInfo = _structInfo.structInfosCnc[i];                  
                    readAndLogDiagnose(ref structInfo);                   
                }


                readAndLogSeqNum();

                _dataStream.Flush();

                //stopwatch.Stop();
                //Console.WriteLine("Ticks: " + stopwatch.ElapsedTicks + " mS: " + stopwatch.ElapsedMilliseconds);
            }
            
            cncFreeHandle();

            /*
            //Configure periodic task
            _stop.Reset();
            _registeredWait = ThreadPool.RegisterWaitForSingleObject(_stop, new WaitOrTimerCallback(PeriodicProcess), null, _delay_ms, false);

            //Init CSV with headers
            _dataStream.WriteLine("Mote ID; MAC addr; Frame Abs. Time ; Delta Time ; Type ; Index ; Value");

        //Read second test diagnose
        //readDiagnoseAreaTest("2 start recording");

    */
    }

        public void StopRecording()
        {
            _stop.Set();
        }

        /*private void PeriodicProcess(object state, bool timeout)
        {
            if (timeout)
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Reset();
                stopwatch.Start();

                int ret = cncGetHandle();
                if (ret!=0)
                {
                    Console.WriteLine("Unable to get the library handle" + " - ERROR : " + ret);
                }

                for(int i = 0; i< _structInfo.structInfosCnc.Count; i++)
                {
                    StructDataCnc structInfo = _structInfo.structInfosCnc[i];
                    readDiagnose(ref structInfo);
                    _dataStream.WriteLine(  "99;" + 
                                            "CNC Focas;" +
                                            structInfo.readingTime + ";" +
                                            "0;" +
                                            structInfo._config.diagnosNumber + ";" +
                                            structInfo._config.axis + ";" +
                                            structInfo.diag.u.idata);
                }
                _dataStream.Flush();


                cncFreeHandle();

                stopwatch.Stop();
                Console.WriteLine("Ticks: " + stopwatch.ElapsedTicks +
                " mS: " + stopwatch.ElapsedMilliseconds);
                _counter++;
            }
            else
                // Stop any more events coming along
                _registeredWait.Unregister(null);
        }*/

        private ManualResetEvent _stop = new ManualResetEvent(false);
        private RegisteredWaitHandle _registeredWait;


        private string _IpAddr;
        private ushort _port;
        private int _timeOut;
        private int _delay_ms;
        private int _counter;
        private StreamWriter _dataStream;
        private ushort _h; // LIBRARY HANDLE
        private Focas1.ODBDGN diag = new Focas1.ODBDGN();
        public StructInfosCnc _structInfo;

        public int readAndLogDiagnose(ref StructDataCnc structDataCnc)
        {
            long time1 = (Int64)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
            short ret = Focas1.cnc_diagnoss(
                        _h, 
                        structDataCnc._config.diagnosNumber, 
                        structDataCnc._config.axis,
                        structDataCnc._config.length,
                        structDataCnc.diag);
            structDataCnc.readingTime = ((Int64)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds + time1) /2;

            _dataStream.WriteLine("99;" +
                                            "CNC Focas;" +
                                            structDataCnc.readingTime + ";" +
                                            "0;" +
                                            structDataCnc._config.diagnosNumber + ";" +
                                            structDataCnc._config.axis + ";" +
                                            structDataCnc.diag.u.idata);


            _counter++;

            return ret;
        }

        public int readAndLogSeqNum()
        {
            long time1 = (Int64)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
            Focas1.ODBSEQ result = new Focas1.ODBSEQ();
            int ret = Focas1.cnc_rdseqnum(_h, result);

            //Console.WriteLine("Seq num : " + result.data);

            

            long readingTime = ((Int64)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds + time1) / 2;

            _dataStream.WriteLine("98;" +
                                        "CNC Focas;" +
                                        readingTime + ";" +
                                        "0;" +
                                        0 + ";" +
                                        0 + ";" +
                                        result.data);

            _counter++;

            return ret;
        }

        public void readDiagnoseAreaTest(string testfrom)
        {
            //CNC Read Diagnose Area
            var tabDiag = new byte[100];
            short s_number = 4910;
            short e_number = 4912;
            short axis = 0;
            short lenght = (short)((e_number - s_number) * (4 + 4 * 1));

            short ret = Focas1.cnc_diagnosr(_h, ref s_number, e_number, ref axis, ref lenght, tabDiag);
            if (ret != 0)
            {
                Console.WriteLine(testfrom + " - ERROR" + ret + " - handler : " + _h);
            }
            else
            {
                Console.WriteLine(testfrom + " - Diagnose range from N° " + s_number + "to N° " + e_number + " = " + tabDiag[0] + " - handler : " + _h);
            }
        }
    }
}
