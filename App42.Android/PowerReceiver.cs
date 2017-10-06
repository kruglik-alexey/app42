using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace App42
{
    enum PowerAction { Boot, Reboot, Shutdown }

    [BroadcastReceiver]
    [IntentFilter(new []{ Intent.ActionBootCompleted, "ACTION_LOCKED_BOOT_COMPLETED", "android.intent.action.QUICKBOOT_POWERON", Intent.ActionShutdown, Intent.ActionReboot })]
    public class PowerReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            PowerAction action;
            switch (intent.Action)
            {
                case Intent.ActionBootCompleted:
                case "android.intent.action.QUICKBOOT_POWERON":
                case "ACTION_LOCKED_BOOT_COMPLETED":
                    action = PowerAction.Boot;
                    break;
                case Intent.ActionReboot:
                    action = PowerAction.Reboot;
                    break;
                case Intent.ActionShutdown:
                    action = PowerAction.Shutdown;
                    break;
                default: 
                    Logger.E($"PowerReceiver action {intent.Action}");
                    return;
            }

            if (action == PowerAction.Boot)
            {
                var i = new Intent(context, typeof(EventsService));
                i.PutExtra(EventsService.Reason, EventsService.ReasonBoot);
                context.StartService(i);
            }
        }
    }
}