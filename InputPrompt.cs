using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace TorOrganizer
{
    public static class InputPrompt
    {
        public static Arguments? EnsureCommand(string[] args)
        {
            var inputs = Arguments.Parse(args);

            if (!Directory.Exists(inputs.Source))
            {
                Console.WriteLine($"Thư mục [{inputs.Source}] không tồn tại!");
                return null;
            }

            if (!Directory.Exists(inputs.Destination))
            {
                Console.WriteLine($"Thư mục [{inputs.Source}] không tồn tại!");
                return null;
            }

            if (inputs.Command != null)
            {
                return inputs;
            }

            var cmd = PromptCommand();
            if (cmd == null)
            {
                return null;
            }

            inputs.Command = cmd;
            inputs.Source = PromptSource(inputs.Source);
            inputs.Destination = PromptDestination(inputs.Source);

            ShowCommand(inputs);

            return inputs;
        }

        private static void ShowCommand(Arguments args)
        {
            var sb = new StringBuilder();

            var executable = Assembly.GetEntryAssembly()!.Location;
            sb.AppendFormat("\"{0}\"", executable);

            switch (args.Command)
            {
                case Commands.Copy:
                    sb.Append(" copy ");
                    break;

                case Commands.Move:
                    sb.Append(" move ");
                    break;
            }

            sb.AppendFormat("\"{0}\"", args.Source);

            if (args.Destination != args.Source)
            {
                sb.AppendFormat(" \"{0}\"", args.Destination);
            }

            var foreground = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.WriteLine("Để thực hiện cùng thao tác, bạn có thể dùng lệnh:");
            Console.WriteLine(sb.ToString());
            Console.WriteLine();
            Console.ForegroundColor = foreground;
        }

        private static Commands? PromptCommand()
        {
            while (true)
            {
                Console.WriteLine("Bạn muốn sao chép hay di chuyển các tập tin?");
                Console.WriteLine("Bấm Enter hoặc nhập copy hoặc cp hoặc c để sao chép.");
                Console.WriteLine("Nhập move hoặc mv hoặc m để di chuyển.");
                Console.WriteLine("Nhập quit hoặc q để thoát.");

                var input = Console.ReadLine();
                switch (input.Trim().ToLowerInvariant())
                {
                    case "":
                    case "c":
                    case "cp":
                    case "copy":
                        return Commands.Copy;

                    case "m":
                    case "mv":
                    case "move":
                        return Commands.Move;

                    case "q":
                    case "quit":
                        return null;
                }
            }
        }

        private static string PromptSource(string current)
        {
            while (true)
            {
                Console.WriteLine("Nhập đường dẫn đến thư mục chứa các tập tin torrent.");
                Console.WriteLine($"Bấm Enter để dùng thư mục [{current}].");

                var input = Console.ReadLine().Trim();
                if (string.IsNullOrEmpty(input))
                {
                    return current;
                }

                var full = Path.GetFullPath(input);
                if (Directory.Exists(full))
                {
                    return full;
                }

                Console.WriteLine($"Thư mục [{full}] không tồn tại!");
            }
        }

        private static string PromptDestination(string current)
        {
            while (true)
            {
                Console.WriteLine("Nhập đường dẫn đến thư mục chứa các thư mục cho mỗi tracker.");
                Console.WriteLine($"Bấm Enter để dùng thư mục [{current}].");

                var input = Console.ReadLine().Trim();
                if (string.IsNullOrEmpty(input))
                {
                    return current;
                }

                var full = Path.GetFullPath(input);
                if (Directory.Exists(full))
                {
                    return full;
                }

                Console.WriteLine($"Thư mục [{full}] không tồn tại!");
            }
        }
    }
}