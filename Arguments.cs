using System.IO;

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
            return args.Length >= 3
                ? Path.GetFullPath(args[2])
                : null;
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