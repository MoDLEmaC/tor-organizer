using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace TorOrganizer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var inputs = InputPrompt.EnsureCommand(args);
            if (inputs == null)
            {
                return;
            }

            var regex = new Regex("\\[[^\\]]+]");
            var files = Directory.GetFiles(inputs.Source, "*.torrent");
            var missings = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
            var knowns = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);


            foreach (var file in files)
            {
                var match = regex.Match(Path.GetFileNameWithoutExtension(file));
                if (!match.Success)
                {
                    continue;
                }

                var tag = match.Value;
                if (missings.Contains(tag))
                {
                    WriteStatus(file, false, $"Không tìm thấy thư mục có tag {tag}!");
                    continue;
                }

                if (!knowns.TryGetValue(tag, out var tracker))
                {
                    var lowerTag = tag.ToLowerInvariant();
                    tracker = Directory
                        .GetDirectories(inputs.Destination)
                        .FirstOrDefault(x => Path.GetFileName(x).ToLowerInvariant().Contains(lowerTag));

                    if (tracker == null)
                    {
                        missings.Add(tag);
                        WriteStatus(file, false, $"Không tìm thấy thư mục có tag {tag}!");
                        continue;
                    }

                    knowns.Add(tag, tracker);
                }

                var target = Path.Combine(tracker, Path.GetFileName(file));
                if (File.Exists(target))
                {
                    WriteStatus(file, false, $"Tập tin ở đích đến đã tồn tại!");
                    continue;
                }

                switch (inputs.Command)
                {
                    case Commands.Copy:
                        File.Copy(file, target);
                        WriteStatus(file, true, $"Tập tin đã được sao chép đến {target}.");
                        break;

                    case Commands.Move:
                        File.Move(file, target);
                        WriteStatus(file, true, $"Tập tin đã được di chuyển đến {target}.");
                        break;
                }
            }
        }

        private static void WriteStatus(string path, bool ok, string status)
        {
            var foreground = Console.ForegroundColor;
            var highlight = ok
                ? ConsoleColor.Green
                : ConsoleColor.Red;

            Console.ForegroundColor = highlight;
            Console.Write(ok ? "Ok  " : "XXX ");

            Console.ForegroundColor = foreground;
            Console.Write(path);
            Console.Write(": ");

            Console.ForegroundColor = highlight;
            Console.WriteLine(status);

            Console.ForegroundColor = foreground;
        }
    }
}
