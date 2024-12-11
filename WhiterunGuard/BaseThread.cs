//***********************************************************
//
// Copyright © 2017-2024 AMI Marine Ltd.  All rights reserved
//
//***********************************************************

using System;
using System.Reflection;
using System.Threading;

namespace WhiterunGuard
{
   public class BaseThread : IDisposable
   {
      #region Public Properties

      public bool RunTaskNow { get; set; }

      #endregion

      #region Protected Fields

      protected ManualResetEvent ResetEvent = new ManualResetEvent(false);

      protected DateTime NextTime = DateTime.UtcNow.TimeAccurateToMinutes().AddMinutes(1);

      protected object SyncLock = new object();

      protected int Offset = 0;
      protected int SleepPeriod = 100;

      protected bool Stopping;

      #endregion

      #region Private Fields

      private Thread _thread = null!;

      #endregion
         

      #region IDisposable Implementation

      public void Dispose()
      {

            ResetEvent?.Dispose();
         
      }

      #endregion

      #region Public Methods

      public virtual void Start()
      {
            Stopping = false;

            if (ResetEvent.SafeWaitHandle.IsClosed)
            {
               ResetEvent = new ManualResetEvent(false);
            }

            if (ResetEvent.Set())
            {
               _thread = new Thread(BackgroundThread)
               {
                  IsBackground = true
               };
               _thread.Start();
            }
         
      }

      public virtual void Stop()
      {
         
            Stopping = true;

            if (!ResetEvent.SafeWaitHandle.IsClosed
                && ResetEvent.Reset())
            {
               ResetEvent.Close();
            }
         
      }

      #endregion

      #region Protected Methods

      protected virtual void PerformTask()
      {
      }

      #endregion

      #region Private Methods

      private void BackgroundThread()
      {


            while (!Stopping
                   && !ResetEvent.SafeWaitHandle.IsClosed
                   && ResetEvent.WaitOne())
            {

                  PerformTask();
               Thread.Sleep(SleepPeriod);
            }
         
         }
      }

      #endregion
   }
