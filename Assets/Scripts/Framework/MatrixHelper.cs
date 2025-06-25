namespace Framework
{
    public static class MatrixHelper
    {
        public static bool IsValidIndex(int row, int col, int width, int height)
        {
            return row >= 0 && row < height && col >= 0 && col < width;
        }
    }
}