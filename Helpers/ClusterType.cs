namespace ScourgeBloom.Helpers
{
    public enum ClusterType
    {
        // Circular cluster centered around 'target'
        Radius,
        Chained,
        Cone,
        // returns a cluster of units that are between LocalPlayer location and 'target'
        PathToUnit
    }
}