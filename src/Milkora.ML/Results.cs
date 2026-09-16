namespace Milkora.ML;

/// <summary>One predicted future step (1-based) with a confidence band.</summary>
public sealed record ForecastPoint(int Step, double Value, double LowerBound, double UpperBound);

/// <summary>A point in the series flagged as an abnormal drop.</summary>
public sealed record AnomalyResult(int Index, double Value, double Expected, double DropPercent, double Severity);
