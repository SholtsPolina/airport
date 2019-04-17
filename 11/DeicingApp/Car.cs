using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DeicingApp
{
    public sealed class Car
    {
        Car() { }

        private static readonly object singleLock = new object();

        private static Car instance = null;

        public static Car Instance
        {
            get
            {
                lock (singleLock)
                {
                    if (instance == null)
                    {
                        instance = new Car();
                    }
                    return instance;
                }
            }
        }

        public void FillingAirplane()
        {
            Thread.Sleep(10000);
        }

        public void SetCarStatus(string status)
        {
            FileManager.Instance.Set(status, "../CarStatus.txt", false);
        }

        public string GetCarStatus()
        {
            return FileManager.Instance.Get(1, "../CarStatus.txt", false);
        }
    }
}
