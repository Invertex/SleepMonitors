<h1>SleepMonitors</h1>
Simple app to put all your monitors to sleep when executed.
</br></br>
Helpful if you're one of those people that needs to leave their PC running but doesn't want to wait for their screens to auto-sleep or have to physically turn them off everytime.
</br></br>

<h2>Usage:</h2>

Simply launch the executable and your monitors should go to sleep.<br />
A small message will display on screen counting down to when it will do so (and may be cancelled by pressing ESC).<br />
This message can be hidden with a `-s` or `silent` launch command like so: `SleepMonitors.exe -s`
You may also set the delay before the app puts your monitors to sleep using `-sleepDelaySeconds 5` as an example to wait 5 seconds.
This helps avoid mouse movements after clicking from potentially waking the sleep. (This timer will happen whether in silent mode or not.)
