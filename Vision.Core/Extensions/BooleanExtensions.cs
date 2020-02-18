namespace Vision.Core
{
    public static class BooleanExtensions
    {
        public static string ToYesNo(this bool value) => value ? "Yes" : "No";

        public static string ToYesNo(this bool? value) => value.GetValueOrDefault().ToYesNo();
    }
}
