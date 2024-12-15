namespace Domain
{
    public class Target
    {
        public string TargetName { get; }
        public string[] DependentTargetsNames { get; }

        public string[] Actions { get; }

        public Target(string targeName, string[] dependentTargetsNames, string[] Actions)
        {
            this.TargetName = targeName;
            this.DependentTargetsNames = dependentTargetsNames;
            this.Actions = Actions;
        }
    }
}
