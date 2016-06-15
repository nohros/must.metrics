using System;
using System.Configuration;
using Nohros.Concurrent;

namespace Nohros.Metrics
{
  /// <summary>
  /// Defines a container for shared <see cref="Mailbox{T}"/> and
  /// <see cref="Clock"/>.
  /// </summary>
  public class MetricContext
  {
    readonly Mailbox<Action> mailbox_;
    readonly Clock clock_;

    /// <summary>
    /// Initializes a new instance of the <see cref="MetricContext"/>
    /// </summary>
    public MetricContext()
      : this(new Mailbox<Action>(x => x()), new StopwatchClock()) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MetricContext"/> by
    /// using the given <paramref name="clock"/>.
    /// </summary>
    /// <param name="clock">
    /// A <see cref="Clock"/> that can be used to measure the passage of time.
    /// </param>
    public MetricContext(Clock clock)
      : this(new Mailbox<Action>(x => x()), clock) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MetricContext"/> by
    /// using the given <paramref name="mailbox"/>.
    /// </summary>
    /// <param name="mailbox">
    /// A <see cref="Mailbox{T}"/> that can be used execute a
    /// <see cref="Action"/> asynchronously.
    /// </param>
    public MetricContext(Mailbox<Action> mailbox)
      : this(mailbox, new StopwatchClock()) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MetricContext"/> by
    /// using the given <paramref name="mailbox"/>.
    /// </summary>
    /// <param name="mailbox">
    /// A <see cref="Mailbox{T}"/> that can be used execute a
    /// <see cref="Action"/> asynchronously.
    /// </param>
    /// <param name="clock">
    /// A <see cref="Clock"/> that can be used to measure the passage of time.
    /// </param>
    public MetricContext(Mailbox<Action> mailbox, Clock clock) {
      mailbox_ = mailbox;
      clock_ = clock;
    }

    static MetricContext() {
      ForCurrentProcess = new MetricContext();
    }

    /// <summary>
    /// Gets the current time tick from the underlying <see cref="Clock"/>.
    /// </summary>
    public long Tick {
      get { return clock_.Tick; }
    }

    /// <summary>
    /// Executed the given <paramref name="runnable"/> asynchronously thorough
    /// the underlying <see cref="Mailbox{T}"/>.
    /// </summary>
    /// <param name="runnable">
    /// The action to be executed.
    /// </param>
    public void Send(Action runnable) {
      mailbox_.Send(runnable);
    }

    /// <summary>
    /// Gets the default <see cref="MetricContext"/>.
    /// </summary>
    public static MetricContext ForCurrentProcess { get; private set; }
  }
}
