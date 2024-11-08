# ExtendedTimer
A C# class that has extra functionality over System.Timer.Timers. This class does not use System.Timer.Timers internally, and instead relies on asynchronous programming.

## Basic usage
```cs
ExtendedTimer timer = new();
timer.TickInterval = 4000; // every 4 seconds
timer.StartDelay = 2000; // wait 2 seconds before starting the tick interval
timer.TickOnStart = true; // immediately tick upon calling Start()
timer.TickOnStartIgnoreDelay = true; // ignore the start delay if TickOnStart is true

// listening for tick events
timer.OnTimerTick += (sender, e) =>
{
	Console.WriteLine($"timer has ticked {timer.TickCount} times");
	if (timer.TickCount > 10)
	{
		Console.WriteLine("pausing timer because it has ticked over 10 times");
		timer.Pause();
	}
};

// listening for state changes
timer.OnTimerStateChanged += (sender, state) =>
{
	Console.WriteLine("timer state has changed. new state: " + state);
	bool exampleCondition = true;
	if (state == ExtendedTimer.TimerState.Paused && exampleCondition)
	{
		Console.WriteLine("resuming timer because a condition was met.");
		timer.Resume();
	}
};

timer.OnTimerStop += (sender, e) =>
{
	Console.WriteLine("timer has stopped. i wonder why...");
	Console.WriteLine("time since timer has been started: " + new DateTime(timer.TimeSinceStart));
};

timer.Start();
```

Any improvements/features to add or bugs to fix are greatly appreciated.
