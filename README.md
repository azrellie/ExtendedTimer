# ExtendedTimer
A C# class that has extra functionality over System.Timer.Timers (or any timer class). This class does not use System.Timer.Timers internally, and instead relies on asynchronous programming.

## Basic usage
```cs
ExtendedTimer timer = new(); // this constructor also accepts a bool parameter to start automatically
timer.TickInterval = 4000; // every 4 seconds
timer.StartDelay = 2000; // wait 2 seconds before starting the tick interval
timer.TickOnStart = true; // immediately tick upon calling Start()
timer.AmountToTick = 50; // tick 50 times, then stop
timer.AmountToSpinWait = 200; // 200 spin wait iterations. reduces load on the processor while keeping delta time precise (too high can make it less precise)
timer.TickOnStartIgnoreDelay = true; // ignore the start delay if TickOnStart is true
timer.TickingMethod = TickMethod.StopwatchLoop; // use stopwatch ticking over Task.Delay ticking (more precise. more intensive on processor)
// TickMethod.StopwatchLoop is not necessary if your tick intervals are long, more than a second

// listening for tick events
timer.OnTimerTick += (sender, e) =>
{
	Console.WriteLine($"timer has ticked {timer.TickCount} times");
	if (timer.TickCount >= 10 && timer.TickCount <= 20)
	{
		Console.WriteLine("pausing timer because it has ticked 10-20 times");
		timer.Pause();
	}
	else if (timer.TickCount > 20)
	{
		Console.WriteLine("pausing timer because it has ticked over 20 times");
		timer.Pause(4000); // you can also pause the timer for a specified amount of milliseconds and it will resume automatically. 4 seconds in this case
	}
};

// listening for state changes
timer.OnTimerStateChanged += (oldState, newState) =>
{
	Console.WriteLine("timer state has changed. new state: " + newState + " | old state: " + oldState);
	bool exampleCondition = true;
	if (newState == ExtendedTimer.TimerState.Paused && exampleCondition)
	{
		Console.WriteLine("resuming timer because a condition was met.");
		timer.Resume();
	}
};

// all events except "OnTimerTick" provide the previous and newest state
timer.OnTimerStop += (oldState, newState) =>
{
	Console.WriteLine("timer has stopped. i wonder why...");
	Console.WriteLine("time since timer has been started: " + timer.TimeAtStart); // TimeAtStart is a DateTimeOffset object
};

timer.Start(); // start. no need to call if constructor was provided with a "true" bool parameter
```

Any improvements/features to add or bugs to fix are greatly appreciated.
