using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Nohros.Concurrent;
using Nohros.Extensions;

namespace Nohros.Metrics.Tests
{
  public class Program
  {
    public static void Main(string[] args) {
      /*var test = new Test();
      var watch = new Stopwatch();

      var measures = new List<double>();
      for (int p = 1; p < Environment.ProcessorCount + 1; ++p) {
        for (int i = 0; i < 10; i++) {
          test.Run(watch, p);
          measures.Add(watch.ElapsedMilliseconds);
        }
        Console.WriteLine("With {0} threads:{1}".Fmt(p, measures.Average()));
      }*/
      var timer = BucketTimer.Create("tests", new long[] {10, 100, 500});
      Thread thread = new Thread(Run);
      thread.Start(timer);

      var t = new System.Timers.Timer(30000);
      t.Elapsed += (sender, event_args) => Poll(timer);
      t.Start();

      Console.WriteLine("Done");
      Console.WriteLine();
      Console.ReadLine();

      t.Stop();
    }

    static void Poll(IEnumerable<IMetric> metrics) {
      foreach (var metric in metrics) {
        var composite = metric as ICompositeMetric;
        if (composite != null) {
          Poll(composite);
        } else {
          metric.GetMeasure(measure => {
            var tags =
              measure
                .MetricConfig
                .Tags
                .Select(x=>x.Name + "=" + x.Value)
                .Aggregate((t1, t2) => t1 + " " + t2);
            Console.WriteLine(tags + " value=" + measure.Value.ToString());
          });

          var step = metric as IStepMetric;
          if (step != null)
            step.OnStep();
        }
      }
    }

    static void Run(object obj) {
      BucketTimer timer = obj as BucketTimer;
      while (true) {
        timer.Time(() => Thread.Sleep(300));
      }
    }

    public class Test
    {
      Mailbox<Action> mailbox_1_;
      ConcurrentQueue<int> concurrent_;
      readonly AutoResetEvent sync_;

      public Test() {
        sync_ = new AutoResetEvent(false);
      }

      SemaphoreSlim semaphore_;
      public void Run(Stopwatch watch, int concurrency) {
        mailbox_1_ = new Mailbox<Action>(message => {});
        concurrent_ = new ConcurrentQueue<int>();
        semaphore_ = new SemaphoreSlim(concurrency);

        for (int i = 0; i < concurrency - 1; i++) {
          new BackgroundThreadFactory()
            .CreateThread(new ParameterizedThreadStart(Loop))
            .Start(concurrency);
        }
        //mailbox_1_.Send(() => Start(watch));
        Start(watch);
        Loop(concurrency);
        semaphore_.Wait();
        Stop(watch);
        //mailbox_1_.Send(() => Stop(watch));
        //sync_.WaitOne();
      }

      void Start(Stopwatch watch) {
        watch.Restart();
      }

      void Stop(Stopwatch watch) {
        watch.Stop();
        sync_.Set();
      }

      void Loop(object threads) {
        semaphore_.Wait();
        for (int i = 0; i < 10000000/(int)threads; i++) {
          UpdateAsync(i);
          //UpdateAtomic(i);
          //Update(i);
          //Interlocked.CompareExchange(ref max_, i, i-1);
          //Interlocked.Increment(ref max_);
          //Interlocked.Increment(ref max_);
          //concurrent_.Enqueue(i);
        }
        semaphore_.Release();
      }

      long max_ = 0;

      void UpdateAsync(long v) {
        mailbox_1_.Send(() => Update(v));
      }

      struct Param {
        public int method;
        public long value;
      }

      void Run(Param param) {
        switch (param.method) {
          case 0:
            Update(param.value);
            break;
        }
      }

      void Update(long v) {
        max_ += v;
      }

      void UpdateAtomic(long v) {
        long value = Interlocked.Read(ref max_);
        long new_value = value + v;
        while (Interlocked.CompareExchange(ref max_, new_value, value) != value) {
          value = Interlocked.Read(ref max_);
          new_value = value + v;
        }
      }
    }
  }
}
