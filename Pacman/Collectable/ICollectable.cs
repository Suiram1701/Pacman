namespace Pacman.Collectable
{
    /// <summary>
    /// All possible points from points and fruits
    /// </summary>
    public enum Points
    {
        P10 = 10,
        P50 = 50,
        P100 = 100,
        P300 = 300,
        P500 = 500,
        P700 = 700,
        P1000 = 1000,
        P2000 = 2000,
        P3000 = 3000,
        P5000 = 5000
    }

    internal interface ICollectable
    {
        /// <summary>
        /// How many points the player collect by eating
        /// </summary>
        Points Point { get; }
    }
}
