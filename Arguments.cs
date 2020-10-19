using System.IO;
using System.Linq;

namespace TorOrganizer
{
    public class Arguments
    {
        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        /// <value>The command to execute, or <c>null</c> if not provided.</value>
        public Commands? Command { get; set; }

        /// <summary>
        /// Gets the full path to folder that has torrent files.
        /// </summary>
        /// <value>Full path to folder containing torrent files.</value>
        public string Source { get; set; }

        /// <summary>
        /// /// Gets the full path to look for target folders.
        /// </summary>
        /// <value>Full path to folder containing target folders.</value>
        public string Destination { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if process should wait for user to read before exiting.
        /// </summary>
        /// <value>
        /// <c>true</c> if process should wait;
        /// <c>false</c> if process should terminate immediately.
        /// <c>null</c> if user has not specified.
        /// </value>
        public bool? Wait { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if new files should overwrite existings.
        /// </summary>
        /// <value>
        /// <c>true</c> if new files should overwrite old files;
        /// <c>false</c> if new files should be ignored.
        /// <c>null</c> if user has not specified.
        /// </value>
        public bool? Overwrite { get; set; }

        public Arguments()
        {
            var current = Directory.GetCurrentDirectory();

            Source = current;
            Destination = current;
        }

        public static Arguments Parse(string[] args)
        {
            var result = new Arguments();

            var cmd = ParseCommand(args);
            if (cmd != null)
            {
                result.Command = cmd;
            }

            var source = ParseSource(args);
            if (source != null)
            {
                result.Source = source;
            }

            var destination = ParseDestination(args);
            if (destination != null)
            {
                result.Destination = destination;
            }
            else if (source != null)
            {
                result.Destination = source;
            }

            if (args.Contains("-w"))
            {
                result.Wait = true;
            }

            if (args.Contains("-f"))
            {
                result.Overwrite = true;
            }
            else if (args.Contains("-skip"))
            {
                result.Overwrite = false;
            }

            return result;
        }

        private static string? ParseSource(string[] args)
        {
            return args.Length >= 2
                ? Path.GetFullPath(args[1])
                : null;
        }

        private static string? ParseDestination(string[] args)
        {
            if (args.Length < 3)
            {
                return null;
            }

            var destination = args[2];
            if (destination.StartsWith("-"))
            {
                return null;
            }

            return Path.GetFullPath(destination);
        }

        private static Commands? ParseCommand(string[] args)
        {
            return args.Length > 0
                ? ParseCommand(args[0])
                : null;
        }

        private static Commands? ParseCommand(string cmd)
        {
            switch (cmd.ToLowerInvariant())
            {
                case "c":
                case "cp":
                case "copy":
                    return Commands.Copy;

                case "m":
                case "mv":
                case "move":
                    return Commands.Move;
            }

            return null;
        }
    }
}