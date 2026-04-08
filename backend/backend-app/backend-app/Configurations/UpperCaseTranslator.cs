using Npgsql;

namespace backend_app.Configurations
{
    public class UpperCaseTranslator : INpgsqlNameTranslator
    {
        public string TranslateMemberName(string clrName) => clrName.ToUpper();

        public string TranslateTypeName(string clrName) => clrName.ToUpper();
    }
}
