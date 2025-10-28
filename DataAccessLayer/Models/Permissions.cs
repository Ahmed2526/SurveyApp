using System.Reflection;

namespace DataAccessLayer.Models
{
    public class Permissions
    {
        public static string Type { get; set; } = "Permissions";
        // Permission-based claims
        public const string CanCreateSurvey = "create_survey";
        public const string CanEditSurvey = "edit_survey";
        public const string CanDeleteSurvey = "delete_survey";
        public const string CanViewSurvey = "view_survey";


        //Automatically gets all string constants declared above
        public static IEnumerable<string> All()
        {
            return typeof(Permissions)
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string))
                .Select(f => f.GetValue(null)!.ToString()!);
        }
    }
}
