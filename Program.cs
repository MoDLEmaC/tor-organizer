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

            var pendings = new List<Pending>();
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
                var exists = File.Exists(target);
                if (exists && inputs.Overwrite != true)
                {
                    switch (inputs.Overwrite)
                    {
                        case false:
                            WriteStatus(file, false, $"Tập tin ở đích đến đã tồn tại!");
                            continue;

                        case null:
                            pendings.Add(new Pending(file, target, exists));
                            continue;
                    }
                }

                switch (inputs.Command)
                {
                    case Commands.Copy:
                        File.Copy(file, target, exists);
                        WriteStatus(file, true, $"Tập tin đã được sao chép đến {target}.");
                        break;

                    case Commands.Move:
                        if (exists)
                        {
                            File.Delete(target);
                        }

                        File.Move(file, target);
                        WriteStatus(file, true, $"Tập tin đã được di chuyển đến {target}.");
                        break;
                }
            }

            if (pendings.Any())
            {
                Confirm(inputs.Command, pendings);
            }

            if (inputs.Wait == true)
            {
                Console.WriteLine("Bấm phím bất kỳ để thoát...");
                Console.ReadKey();
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

        private static OverwriteAnswers PromptOverwrite(string target)
        {
            while (true)
            {
                Console.WriteLine($"Tập tin {target} đã tồn tại. Bạn có muốn ghi đè không?");
                Console.Write($"Bấm Enter hoặc n hoặc no để bỏ qua. Bấm y hoặc yes để ghi đè. Bấm a hoặc all để ghi đề mọi tập tin. Bấm s hoặc skip để bỏ qua mọi tập tin.");

                switch (Console.ReadLine().Trim())
                {
                    case "y":
                    case "yes":
                        return OverwriteAnswers.Overwrite;

                    case "":
                    case "n":
                    case "no":
                        return OverwriteAnswers.SkipOne;

                    case "a":
                    case "all":
                        return OverwriteAnswers.OverwriteAll;

                    case "s":
                    case "skip":
                        return OverwriteAnswers.SkipAll;
                }
            }
        }

        private static void Confirm(Commands? command, IEnumerable<Pending> pendings)
        {
            if (command == null)
            {
                return;
            }

            var prompt = true;
            foreach (var pair in pendings)
            {
                if (prompt)
                {
                    var answer = PromptOverwrite(pair.Target);
                    switch (answer)
                    {
                        case OverwriteAnswers.Overwrite:
                            break;

                        case OverwriteAnswers.OverwriteAll:
                            prompt = false;
                            break;

                        case OverwriteAnswers.SkipOne:
                            continue;

                        case OverwriteAnswers.SkipAll:
                            return;
                    }
                }

                switch (command)
                {
                    case Commands.Copy:
                        File.Copy(pair.Source, pair.Target, true);
                        WriteStatus(pair.Source, true, $"Tập tin đã được sao chép đến {pair.Target}.");
                        break;

                    case Commands.Move:
                        if (pair.TargetExists)
                        {
                            File.Delete(pair.Target);
                        }

                        File.Move(pair.Source, pair.Target);
                        WriteStatus(pair.Source, true, $"Tập tin đã được di chuyển đến {pair.Target}.");
                        break;
                }
            }
        }
    }
}
