﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using RepoZ.Ipc;

namespace grr.Messages
{
    [System.Diagnostics.DebuggerDisplay("{GetRemoteCommand()}")]
    public class ChangeToDirectoryMessage : DirectoryMessage
    {
        public ChangeToDirectoryMessage(RepositoryFilterOptions filter)
            : base(filter)
        {
        }

        protected override void ExecuteExistingDirectory(string directory)
        {
            var command = $"cd \"{directory}\"";

            // type the path into the console which is hosting grr.exe to change to the directory
            ConsoleExtensions.WriteConsoleInput(Process.GetCurrentProcess(), command);
        }

        protected override void ExecuteRepositoryQuery(Repository[] repositories)
        {
            if (repositories?.Length > 1)
            {
                // only use the first repository when multiple repositories came in
                // cd makes no sense with multiple repositories
                System.Console.WriteLine("");
                System.Console.WriteLine($"Found multiple repositories, using {repositories[0].Name}.");
                System.Console.WriteLine("You can access the others by index now, like:\n  grr cd :2");
                base.ExecuteRepositoryQuery(new Repository[] { repositories[0] });
            }
            else
            {
                base.ExecuteRepositoryQuery(repositories);
            }
        }
    }
}
