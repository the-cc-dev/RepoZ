using System;
using System.Linq;
using AppKit;
using CoreGraphics;

namespace SKConsole
{
    public static class SendKeys
    {
        public static void Send(string value)
        {
            using (var eventSource = new CGEventSource(CGEventSourceStateID.HidSystem))
            {
                var keys = value.Select(GetKey)
                                .Select(c => (ushort)c)
                                .ToArray();

                foreach (var key in keys)
                {
                    using (var keyEvent = new CGEvent(eventSource, key, keyDown: true))
                    {
                        CGEvent.Post(keyEvent, CGEventTapLocation.HID);
                    }
                }
            }
        }

        private static NSKey GetKey(char c) => (NSKey)Enum.Parse(typeof(NSKey), c.ToString().ToUpper());
    }
}
