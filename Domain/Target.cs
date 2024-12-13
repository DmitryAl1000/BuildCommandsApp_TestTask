namespace Domain
{
    public class Target
    {
        public string TargetName { get; }
        public List<string> DependentTargetsNames { get; }

        public List<string> Actions { get; }

        public Target(string targeName, List<string> dependentTargetsNames, List<string> Actions)
        {
            this.TargetName = targeName;
            this.DependentTargetsNames = dependentTargetsNames;
            this.Actions = Actions;
        }
    }
}
