namespace TorOrganizer
{
    public class Pending
    {
        /// <summary>
        /// Gets the source file path.
        /// </summary>
        public string Source { get; }

        /// <summary>
        /// Gets the target file path.
        /// </summary>
        public string Target { get; }

        /// <summary>
        /// Gets a value indicating if <see cref="Target" /> exists.
        /// </summary>
        public bool TargetExists { get; }

        public Pending(string source, string target, bool targetExists)
        {
            Source = source;
            Target = target;
            TargetExists = targetExists;
        }
    }
}