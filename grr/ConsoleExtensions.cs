﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace grr
{
	internal static class ConsoleExtensions
	{
		/// <summary>
		/// A utility class to determine a process parent.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct ParentProcessUtilities
		{
			// These members must match PROCESS_BASIC_INFORMATION
			internal IntPtr Reserved1;
			internal IntPtr PebBaseAddress;
			internal IntPtr Reserved2_0;
			internal IntPtr Reserved2_1;
			internal IntPtr UniqueProcessId;
			internal IntPtr InheritedFromUniqueProcessId;

			[DllImport("ntdll.dll")]
			private static extern int NtQueryInformationProcess(IntPtr processHandle, int processInformationClass, ref ParentProcessUtilities processInformation, int processInformationLength, out int returnLength);

			/// <summary>
			/// Gets the parent process of the current process.
			/// </summary>
			/// <returns>An instance of the Process class.</returns>
			public static Process GetParentProcess()
			{
				return GetParentProcess(Process.GetCurrentProcess().Handle);
			}

			/// <summary>
			/// Gets the parent process of specified process.
			/// </summary>
			/// <param name="id">The process id.</param>
			/// <returns>An instance of the Process class.</returns>
			public static Process GetParentProcess(int id)
			{
				Process process = Process.GetProcessById(id);
				return GetParentProcess(process.Handle);
			}

			/// <summary>
			/// Gets the parent process of a specified process.
			/// </summary>
			/// <param name="handle">The process handle.</param>
			/// <returns>An instance of the Process class.</returns>
			public static Process GetParentProcess(IntPtr handle)
			{
				ParentProcessUtilities pbi = new ParentProcessUtilities();
				int status = NtQueryInformationProcess(handle, 0, ref pbi, Marshal.SizeOf(pbi), out int returnLength);
				if (status != 0)
					throw new Win32Exception(status);

				try
				{
					return Process.GetProcessById(pbi.InheritedFromUniqueProcessId.ToInt32());
				}
				catch (ArgumentException)
				{
					// not found
					return null;
				}
			}
		}

        public static void WriteConsoleInput(Process target, string value, int waitMilliseconds = 0)
        {
            // TODO
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                target = Process.GetCurrentProcess();
            }
            else
            {
                if (target.Id == Process.GetCurrentProcess().Id)
                    target = ParentProcessUtilities.GetParentProcess(target.Id);
            }

            // append ENTER key
            var arguments = (value + "{Enter}")
                .Replace("\"", "'"); // escape " with \" so that SendKeys.exe can unescape them again

            arguments = $"-pid:{target.Id} \"{arguments}\"";

            if (waitMilliseconds > 0)
                arguments += $" -wait:{waitMilliseconds}";

            var currentPath = Path.GetDirectoryName(Path.Combine(Assembly.GetExecutingAssembly().Location));
            var command = Path.Combine(currentPath, "SendKeys.exe");

            if (File.Exists(command))
            {
                // TODO
                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    arguments = "\"" + command + "\" " + arguments;
                    command = "mono";
                }

                Process.Start(new ProcessStartInfo(command, arguments) { UseShellExecute = true });
            }
            else
            {
                Console.WriteLine(command + " does not exist.");
            }
		}
	}
}
