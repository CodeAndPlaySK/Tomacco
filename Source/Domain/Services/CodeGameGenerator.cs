namespace Application.Services
{
    public interface ICodeGameGenerator
    {
        string GenerateNewCode(Func<string, bool> isAlreadyExisting);
    }

    public class CodeGameGenerator : ICodeGameGenerator
    {

        public CodeGameGenerator()
        {
        }

        // Func to avoid circular dependency
        public string GenerateNewCode(Func<string, bool> isAlreadyExisting)
        {
            do
            {
                var code = new char[5];
                for (int idxCode = 0; idxCode < 5; idxCode++)
                {
                    code[idxCode] = (char)(new Random().Next(26) + 65);
                }

                var codeAsString = new string(code);
                if (!isAlreadyExisting(codeAsString))
                {
                    return codeAsString;
                }
            } while (true);
        }
    }
}
