namespace MineSweeper.Extensions;

/// <summary>
/// Contains the RadarPaternWitRadarLikeOpacityAnimation method implementation.
/// </summary>
public static class RadarPaternWitRadarLikeOpacityAnimationExtension
{
    /// <summary>
    ///     Performs a radar-like sweeping animation with varying opacity.
    ///     Tiles become visible briefly during first two sweeps, then stay visible after third sweep.
    /// </summary>
    public static async Task RadarPaternWitRadarLikeOpacityAnimation(Image image, int row, int col, int totalRows, int totalColumns)
    {
        // Initial state: invisible
        image.Opacity = 0;
        image.Scale = 1.0;
        
        // Calculate center coordinates
        var centerRow = totalRows / 2.0;
        var centerCol = totalColumns / 2.0;
        
        // Calculate distance and angle from center
        var rowDistance = row - centerRow;
        var colDistance = col - centerCol;
        var angle = Math.Atan2(rowDistance, colDistance);
        var angleDegrees = angle * (180 / Math.PI);
        var distance = Math.Sqrt(rowDistance * rowDistance + colDistance * colDistance);
        
        // Normalize angle to 0-360 degrees
        var normalizedAngle = (angleDegrees + 360) % 360;
        
        // Define animation parameters
        const int sweeps = 3; // Exactly three radar sweeps
        const int totalDuration = 4500; // Total duration for all sweeps
        const int singleSweepDuration = totalDuration / sweeps;
        
        // Calculate the time at which this cell should be "hit" by the radar in each sweep
        // based on its angle from the center
        var sweepHitTime = normalizedAngle / 360.0;
        
        // For each of the three sweeps
        for (int sweep = 0; sweep < sweeps; sweep++)
        {
            // Calculate when this cell should be hit in this sweep
            var sweepStartTime = sweep * singleSweepDuration;
            var cellHitTime = sweepStartTime + (int)(sweepHitTime * singleSweepDuration);
            
            // Wait until it's time for this cell to be hit by the radar
            var waitTime = cellHitTime - (sweep > 0 ? 0 : 0); // No initial delay for subsequent sweeps
            if (waitTime > 0)
            {
                await Task.Delay(waitTime);
            }
            
            if (sweep < 2)
            {
                // For first two sweeps, briefly show the cell then hide it
                await image.FadeTo(0.8, 150, Easing.CubicOut);
                await image.FadeTo(0, 350, Easing.CubicIn);
            }
            else
            {
                // For the third sweep, make the cell fully visible and keep it that way
                await image.FadeTo(1.0, 200, Easing.CubicOut);
            }
        }
    }
}
