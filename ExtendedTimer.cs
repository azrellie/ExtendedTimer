namespace Azrellie.Misc.ExtendedTimer
{
	public class ExtendedTimer : IDisposable
	{
		public enum TimerState
		{
			Running,
			Paused,
			Stopped
		}

		// internal stuff
		private TimerState lastState;
		private bool paused = false;
		private bool enabled = false;
		private long timeSinceLastTick = 0;

		// properties
		/// <summary>
		/// Determines whether <see cref="OnTimerTick"/> will be fired immediately upon starting the timer. This is false by default.
		/// </summary>
		public bool TickOnStart { get; set; } = false;

		/// <summary>
		/// How many times this timers <see cref="OnTimerTick"/> has been fired.
		/// </summary>
		public long TickCount { get; private set; } = 0;

		/// <summary>
		/// How long to wait to start firing <see cref="OnTimerTick"/> when <see cref="Start"/> is called (in milliseconds).
		/// </summary>
		public long StartDelay { get; set; } = 0;

		/// <summary>
		/// The amount of seconds since this <see cref="ExtendedTimer"/> has started.
		/// </summary>
		public long TimeSinceStart { get; set; } = 0;

		/// <summary>
		/// Whether this <see cref="ExtendedTimer"/> is allowed to fire <see cref="OnTimerTick"/>. Default value is true.
		/// </summary>
		public bool Enabled { get; set; } = true;

		/// <summary>
		/// The amount (in milliseconds) to wait before firing <see cref="OnTimerTick"/>. Default value is 1000.
		/// </summary>
		public int TickInterval { get; set; } = 1000;

		/// <summary>
		/// Whether <see cref="TickOnStart"/> ignores <see cref="StartDelay"/>. Default value is false.
		/// </summary>
		/// <remarks>
		/// If set to false, <see cref="OnTimerTick"/> will not be fired upon the timer being started.
		/// If set to true, <see cref="OnTimerTick"/> will be fired upon the timer being started.
		/// </remarks>
		public bool TickOnStartIgnoreDelay { get; set; } = false;

		/// <summary>
		/// The amount of times <see cref="OnTimerTick"/> can be fired before stopping the timer. Default value is -1 (will tick indefinitely).
		/// </summary>
		public int AmountToTick { get; set; } = -1;

		/// <summary>
		/// Indicates the current state of the timer.
		/// </summary>
		public TimerState State { get; set; } = TimerState.Stopped;

		// events
		public delegate void OnTimerStartEventHandler(object sender, EventArgs e);
		public delegate void OnTimerStopEventHandler(object sender, EventArgs e);
		public delegate void OnTimerTickEventHandler(object sender, EventArgs e);
		public delegate void OnTimerPausedEventHandler(object sender, EventArgs e);
		public delegate void OnTimerResumedEventHandler(object sender, EventArgs e);
		public delegate void OnTimerStateChangedEventHandler(object sender, TimerState state);

#pragma warning disable
		/// <summary>
		/// Fired whenever <see cref="Timer.Start"/> is called.
		/// </summary>
		public event OnTimerStartEventHandler OnTimerStart;

		/// <summary>
		/// Fired whenever <see cref="Timer.Stop"/> is called.
		/// </summary>
		public event OnTimerStopEventHandler OnTimerStop;

		/// <summary>
		/// Fired whenever the interval has elapsed.
		/// </summary>
		public event OnTimerTickEventHandler OnTimerTick;

		public event OnTimerPausedEventHandler OnTimerPaused;

		public event OnTimerResumedEventHandler OnTimerResumed;

		public event OnTimerStateChangedEventHandler OnTimerStateChanged;

		public void Start()
		{
			enabled = true;
			TimeSinceStart = DateTime.UtcNow.Ticks;
			InvokeStateChanged(TimerState.Running);
			OnTimerStateChanged?.Invoke(this, State);
			InternalStart();
		}

		/// <summary>
		/// Stops the <see cref="ExtendedTimer"/>.
		/// </summary>
		/// <remarks>
		/// Calling this will reset the <see cref="TickCount"/> back to 0. <see cref="TimeSinceStart"/> does not get reset.
		/// </remarks>
		public void Stop()
		{
			TickCount = 0;
			enabled = false;
			InvokeStateChanged(TimerState.Stopped);
			OnTimerStateChanged?.Invoke(this, State);
		}

		/// <summary>
		/// Stops the <see cref="ExtendedTimer"/> without resetting the <see cref="TickCount"/> back to 0.
		/// </summary>
		public void Pause()
		{
			paused = false;
			InvokeStateChanged(TimerState.Paused);
			OnTimerStateChanged?.Invoke(this, State);
		}

		public void Resume()
		{
			paused = false;
			InvokeStateChanged(TimerState.Running);
			OnTimerStateChanged?.Invoke(this, State);
		}

		private void InvokeOnTickEvent()
		{
			if (!Enabled) return;
			if (paused) return;
			if (TickCount >= AmountToTick && AmountToTick != -1)
			{
				Stop();
				return;
			}
			timeSinceLastTick = DateTime.UtcNow.Ticks;
			TickCount++;
			OnTimerTick?.Invoke(this, EventArgs.Empty);
		}

		private void InvokeStateChanged(TimerState state)
		{
			if (lastState == state) return;
			State = state;
			OnTimerStateChanged?.Invoke(this, state);
			lastState = State;
		}

		private async Task InternalStart()
		{
			if (TickOnStart)
				InvokeOnTickEvent();

			while (enabled)
			{
				if (paused)
				{
					await Task.Delay(1);
					continue;
				}
				await Task.Delay(TickInterval);
				InvokeOnTickEvent();
			}
		}

		public void Dispose()
		{
			Stop();
			Dispose();
			GC.SuppressFinalize(this);
		}
	}
}