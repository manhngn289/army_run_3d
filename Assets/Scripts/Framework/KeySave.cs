namespace Framework
{
    public static class KeySave
    {
        // Tags
        public const string PoolParent = "PoolParent";
        public static readonly string EnemyTag = "Enemy";
        
        // Spatial Grid Settings
        public const int MaxEnemyPerCell = 32;
        public const int GridWidth = 10;
        public const int GridHeight = 150;
        public const float HalfGridWidth = GridWidth / 2f;
        public const float HalfGridHeight = GridHeight / 2f;
        public const float CellSize = 1f;
    }
}