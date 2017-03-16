namespace Yttrium.Core
{
    public class CommandParseResult<T> where T : class
    {
        public bool IsValid { get; set; }

        public T Command { get; set; }
    }
}
