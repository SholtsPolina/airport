using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Threading;

namespace DeicingApp
{
    class Program
    {
        class WorkingLoop
        {
            private static int controllerCounter = 1;
            private static bool invoke = false;
            public void StartLoop()
            {
                Console.WriteLine("Init loop file manager");
                
                Console.WriteLine(Car.Instance.GetCarStatus());
                Console.WriteLine("Starting working loop");

                while (true)
                {
                    string s = FileManager.Instance.Get(controllerCounter, "../../../../controllerStatus.txt", false);
                    Console.WriteLine("Got controller input");
                    Console.WriteLine(s);

                    if (s.Equals("0"))
                    {
                        Console.WriteLine("no input");
                        Thread.Sleep(5000);
                    }
                    else if (s.Equals("error"))
                    {
                        Console.WriteLine("bad input");
                        Thread.Sleep(5000);
                    }
                    else if (s.Equals("3"))
                    {
                        Console.WriteLine("invoke initiated");
                        invoke = true;
                    }
                    else
                    {
                        Console.WriteLine("got input, no queue");
                        string[] splitResult = s.Split(' ');
                        string planeID = splitResult[1];
                        Console.WriteLine("starting thread");

                        ThreadPool.QueueUserWorkItem(delegate { ExecuteRefuelling(planeID); });

                        controllerCounter++;
                        Thread.Sleep(5000);
                    }
                }
            }

            public void ExecuteRefuelling(string planeID)
            {
                Console.WriteLine("Queue started");
                Console.WriteLine("Thread started");
                Console.WriteLine("planeid is {0}", planeID);
                var time = 10000;

                string planePos = MakeGetGateRequestCall("https://groundcontrol.v2.vapor.cloud/getTFInformation", planeID);    //get plane position (gate)
                if (!planePos.Equals("error"))
                {
                    MoveRequest mvReq = new MoveRequest("Garage", planePos, "Deicing Service", "Deicing1");
                    var jsonmvReq = JsonConvert.SerializeObject(mvReq);
                    string moveToGatePermission = MakeMoveRequestCall("https://groundcontrol.v2.vapor.cloud/askForPermission", jsonmvReq);//request permission to move to gate

                    if (moveToGatePermission.Equals("Obtained"))
                    {
                        Car.Instance.SetCarStatus("2");
                        Thread.Sleep(10000);
                        Car.Instance.SetCarStatus("3");
                        Thread.Sleep(time);
                        MoveRequest mvBackReq = new MoveRequest(planePos, "Garage", "Deicing Service", "Deicing1"); //request permission to move to garage
                        var jsonmvbReq = JsonConvert.SerializeObject(mvBackReq);
                        string moveToGaragePermission = MakeMoveRequestCall("https://groundcontrol.v2.vapor.cloud/askForPermission", jsonmvbReq);

                        if (moveToGaragePermission.Equals("Obtained"))
                        {
                            Car.Instance.SetCarStatus("2");
                            Thread.Sleep(10000);
                            Car.Instance.SetCarStatus("0");
                        }

                        else if (moveToGatePermission.Equals("Queued"))
                        {
                            Car.Instance.SetCarStatus("4");
                            while (!invoke)
                            {
                                Thread.Sleep(1000);
                            }
                            invoke = false;
                            Car.Instance.SetCarStatus("2");
                            Thread.Sleep(10000);
                            Car.Instance.SetCarStatus("0");
                        }
                        else if (moveToGatePermission.Equals("Denied"))
                        {

                        }
                    }
                    else if (moveToGatePermission.Equals("Queued"))
                    {
                        Car.Instance.SetCarStatus("1");
                        while (!invoke)
                        {
                            Thread.Sleep(1000);
                        }
                        invoke = false;
                        Car.Instance.SetCarStatus("2");
                        Thread.Sleep(10000);
                        Car.Instance.SetCarStatus("0");

                    }
                    else if (moveToGatePermission.Equals("Denied"))
                    {

                    }
                    else
                    {
                        //something
                    }
                }
                else
                {
                    Console.WriteLine("error in gate call");
                }

            }

            private string MakeMoveRequestCall(string host, string jsonMessage)
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(host);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
                
                streamWriter.Write(jsonMessage);
                streamWriter.Flush();
                streamWriter.Close();

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                Console.WriteLine("awaiting response");

                var streamReader = new StreamReader(httpResponse.GetResponseStream());
                var result = JsonConvert.DeserializeObject<MoveRequestResult>(streamReader.ReadToEnd());

                Console.WriteLine("Post call (move request) to {0} returns {1}", host, result.permission);
                return result.permission;
            }

            private string MakeGetGateRequestCall(string host, string planeID)
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(host);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream());

                GetGateRequest ggr = new GetGateRequest("Air Facility", planeID);
                string jsonMessage = JsonConvert.SerializeObject(ggr);
                streamWriter.Write(jsonMessage);
                streamWriter.Flush();
                streamWriter.Close();

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                Console.WriteLine("awaiting response");

                var streamReader = new StreamReader(httpResponse.GetResponseStream());
                var result = JsonConvert.DeserializeObject<GetGateRequestResult>(streamReader.ReadToEnd());

                Console.WriteLine("Post call (get gate request) to {0} returns {1}", host, result.locationCode);
                if (result.status.Equals("Unknown"))
                {
                    return "error";
                }
                else
                {
                    return result.locationCode;
                }
            }
        }

        //public static int GetTemp(string cityName)
        //{
        //    string str;
        //    using (StreamReader strr = new StreamReader(HttpWebRequest.Create($"http://api.apixu.com/v1/current.json?key=f23f1e67a5b94765988182759191604&q={cityName}").GetResponse().GetResponseStream()))
        //        str = strr.ReadToEnd();
        //    var temp = JObject.Parse(str)["current"]["temp_c"].ToString();
        //    return Convert.ToInt32(temp);
        //}

        static void Main(string[] args)
        {
            Console.WriteLine("Backend Started");
                WorkingLoop wl = new WorkingLoop();
                wl.StartLoop();
        }
    }
}
